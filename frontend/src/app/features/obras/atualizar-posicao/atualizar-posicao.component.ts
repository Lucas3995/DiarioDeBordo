import { Component, Optional, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { take } from 'rxjs';
import {
  AtualizarPosicaoService,
  ObraDetalhe,
  AtualizarPosicaoRequest,
  AtualizarPosicaoResponse,
} from '../../../application/atualizar-posicao.service';
import { TipoObra, ROTULO_POSICAO } from '../../../domain/obra.types';
import { formatarDataRelativa, formatarDataTooltip as formatarDataTooltipDomain } from '../../../domain/datas';
import { DialogRef } from '../../../shared/dialog/dialog-ref';
import { DialogService } from '../../../shared/dialog/dialog.service';
import {
  SAIDA_APOS_SUCESSO,
  SaidaAposSucesso,
  DELAY_FECHAMENTO_APOS_SUCESSO_MS,
} from '../../../shared/dialog/saida-apos-sucesso';
import { PromptObraNovaComponent, PromptObraNovaResult } from './prompt-obra-nova.component';

@Component({
  selector: 'app-atualizar-posicao',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './atualizar-posicao.component.html',
  styleUrl: './atualizar-posicao.component.scss',
})
export class AtualizarPosicaoComponent {
  /** Identificador: 'id' ou 'nome'. */
  tipoIdentificador: 'id' | 'nome' = 'nome';
  valorIdentificador = '';
  novaPosicao = 0;
  dataUltimaAtualizacao: string | null = null; // ISO date string ou vazio = hoje
  /** Estado preenchido pelo resultado do prompt "obra nova"; null quando não há criação pendente. */
  dadosCriacaoPendentes: { nome: string; tipo: TipoObra; ordemPreferencia: number } | null = null;

  /** Compatibilidade para testes que definem estado de criação diretamente (evita alterar specs). */
  get nomeParaCriar(): string {
    return this.dadosCriacaoPendentes?.nome ?? '';
  }
  set nomeParaCriar(v: string) {
    if (!this.dadosCriacaoPendentes)
      this.dadosCriacaoPendentes = { nome: '', tipo: TipoObra.Manga, ordemPreferencia: 0 };
    this.dadosCriacaoPendentes.nome = v;
  }
  get tipoParaCriar(): TipoObra {
    return this.dadosCriacaoPendentes?.tipo ?? TipoObra.Manga;
  }
  set tipoParaCriar(v: TipoObra) {
    if (!this.dadosCriacaoPendentes)
      this.dadosCriacaoPendentes = { nome: '', tipo: TipoObra.Manga, ordemPreferencia: 0 };
    this.dadosCriacaoPendentes.tipo = v;
  }
  get ordemPreferenciaParaCriar(): number {
    return this.dadosCriacaoPendentes?.ordemPreferencia ?? 0;
  }
  set ordemPreferenciaParaCriar(v: number) {
    if (!this.dadosCriacaoPendentes)
      this.dadosCriacaoPendentes = { nome: '', tipo: TipoObra.Manga, ordemPreferencia: 0 };
    this.dadosCriacaoPendentes.ordemPreferencia = v;
  }

  preview: ObraDetalhe | null = null;
  erro: string | null = null;
  sucesso: string | null = null;
  carregando = false;

  constructor(
    private readonly atualizarPosicaoService: AtualizarPosicaoService,
    private readonly dialogService: DialogService,
    @Optional() @Inject(SAIDA_APOS_SUCESSO) private readonly saidaAposSucesso: SaidaAposSucesso | null,
    @Optional() private readonly dialogRef: DialogRef<{ salvou: boolean }> | null,
    @Optional() private readonly router: Router | null,
  ) {}

  get dataParaEnvio(): string | undefined {
    if (this.dataUltimaAtualizacao) return this.dataUltimaAtualizacao;
    return new Date().toISOString().slice(0, 10);
  }

  verPreview(): void {
    this.erro = null;
    this.preview = null;
    this.sucesso = null;
    const id = this.valorIdentificador.trim();
    if (!id) {
      this.erro = 'Informe o código (id) ou o nome da obra.';
      return;
    }
    this.carregando = true;
    const obs =
      this.tipoIdentificador === 'id'
        ? this.atualizarPosicaoService.obterPorId(id)
        : this.atualizarPosicaoService.obterPorNome(id);
    obs.subscribe({
      next: (obra) => {
        this.preview = obra;
        this.carregando = false;
      },
      error: (err) => {
        this.carregando = false;
        if (err?.status === 404) {
          this.abrirPromptObraNovaParaPreview(id);
        } else {
          this.erro = err?.message ?? 'Erro ao buscar obra.';
        }
      },
    });
  }

  private abrirPromptObraNovaParaPreview(id: string): void {
    const ref = this.dialogService.open<PromptObraNovaComponent, PromptObraNovaResult | undefined>(
      PromptObraNovaComponent,
      { data: { nomeDefault: id } }
    );
    ref.afterClosed.pipe(take(1)).subscribe((result) => {
      if (result?.prosseguir) {
        this.aplicarResultadoPromptObraNova(result);
        this.preview = this.construirPreviaSinteticaObraNova(id);
      } else {
        this.erro = 'Obra não encontrada.';
      }
    });
  }

  private aplicarResultadoPromptObraNova(result: PromptObraNovaResult): void {
    this.dadosCriacaoPendentes = {
      nome: result.nome,
      tipo: result.tipo,
      ordemPreferencia: result.ordemPreferencia,
    };
  }

  salvar(): void {
    this.erro = null;
    this.sucesso = null;
    const id = this.valorIdentificador.trim();
    if (!id) {
      this.erro = 'Informe o código (id) ou o nome da obra.';
      return;
    }
    const temDadosCriacaoPendentes = this.dadosCriacaoPendentes != null;
    const request = this.montarRequest(id, temDadosCriacaoPendentes);
    this.carregando = true;
    this.atualizarPosicaoService.atualizarPosicao(request).subscribe({
      next: (res) => this.tratarSucessoSalvamento(res),
      error: (err) => {
        this.carregando = false;
        if (err?.status === 404) {
          this.abrirPromptObraNovaParaSalvar(id);
        } else {
          this.erro = err?.error?.message ?? err?.message ?? 'Erro ao atualizar.';
        }
      },
    });
  }

  private tratarSucessoSalvamento(res: AtualizarPosicaoResponse): void {
    this.carregando = false;
    this.sucesso = res.criada ? 'Obra criada e posição registrada.' : 'Posição atualizada.';
    this.preview = null;
    if (this.saidaAposSucesso) {
      this.saidaAposSucesso.fecharComSucesso();
    } else if (this.dialogRef) {
      const dialogRef = this.dialogRef;
      setTimeout(() => dialogRef.close({ salvou: true }), DELAY_FECHAMENTO_APOS_SUCESSO_MS);
    } else if (this.router) {
      const router = this.router;
      setTimeout(() => router.navigate(['/obras']), DELAY_FECHAMENTO_APOS_SUCESSO_MS);
    }
  }

  private montarRequest(id: string, criarSeNaoExistir: boolean): AtualizarPosicaoRequest {
    const request: AtualizarPosicaoRequest = {
      novaPosicao: this.novaPosicao,
      criarSeNaoExistir,
    };
    if (this.tipoIdentificador === 'id') request.idObra = id;
    else request.nomeObra = id;
    if (this.dataParaEnvio) request.dataUltimaAtualizacao = this.dataParaEnvio;
    if (criarSeNaoExistir && this.dadosCriacaoPendentes) {
      request.nomeParaCriar = this.dadosCriacaoPendentes.nome.trim() || id;
      request.tipoParaCriar = this.dadosCriacaoPendentes.tipo;
      request.ordemPreferenciaParaCriar = this.dadosCriacaoPendentes.ordemPreferencia;
    }
    return request;
  }

  private abrirPromptObraNovaParaSalvar(id: string): void {
    const ref = this.dialogService.open<PromptObraNovaComponent, PromptObraNovaResult | undefined>(
      PromptObraNovaComponent,
      { data: { nomeDefault: id } }
    );
    ref.afterClosed.pipe(take(1)).subscribe((result) => {
      if (result?.prosseguir) {
        this.aplicarResultadoPromptObraNova(result);
        this.executarSalvamentoObraNova(id);
      } else {
        this.erro = 'Obra não encontrada.';
      }
    });
  }

  private executarSalvamentoObraNova(id: string): void {
    const request = this.montarRequest(id, true);
    this.carregando = true;
    this.atualizarPosicaoService.atualizarPosicao(request).subscribe({
      next: (res) => this.tratarSucessoSalvamento(res),
      error: (err) => {
        this.carregando = false;
        this.erro = err?.error?.message ?? err?.message ?? 'Erro ao atualizar.';
      },
    });
  }

  formatarData(s: string): string {
    return formatarDataRelativa(new Date(s));
  }

  formatarDataTooltip(s: string): string {
    return formatarDataTooltipDomain(new Date(s));
  }

  rotuloPosicao(tipo: string): string {
    return ROTULO_POSICAO[tipo as TipoObra] ?? 'parte';
  }

  /** Fecha o dialog (quando usado em popup). */
  fechar(): void {
    this.dialogRef?.close(undefined);
  }

  /** Constrói prévia sintética para obra que ainda não existe (404 + dados de criação pendentes). */
  private construirPreviaSinteticaObraNova(id: string): ObraDetalhe {
    const d = this.dadosCriacaoPendentes;
    const dataEnvio = this.dataParaEnvio ?? new Date().toISOString().slice(0, 10);
    return {
      id: '',
      nome: d?.nome.trim() || id,
      tipo: d?.tipo ?? TipoObra.Manga,
      posicaoAtual: 0,
      dataUltimaAtualizacaoPosicao: dataEnvio,
      ordemPreferencia: d?.ordemPreferencia ?? 0,
      obraNova: true,
    };
  }
}
