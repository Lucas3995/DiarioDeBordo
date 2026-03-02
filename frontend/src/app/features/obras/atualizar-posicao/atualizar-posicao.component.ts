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
import { IListaObrasPort, ObraBuscaItem } from '../../../domain/lista-obras.port';
import { TipoObra, ROTULO_POSICAO } from '../../../domain/obra.types';
import { formatarDataRelativa, formatarDataTooltip as formatarDataTooltipDomain } from '../../../domain/datas';
import { DialogRef } from '../../../shared/dialog/dialog-ref';
import { DialogService } from '../../../shared/dialog/dialog.service';
import {
  SAIDA_APOS_SUCESSO,
  SaidaAposSucesso,
  DELAY_FECHAMENTO_APOS_SUCESSO_MS,
} from '../../../shared/dialog/saida-apos-sucesso';
import {
  AutocompleteItem,
  InputAutocompleteComponent,
} from '../../../shared/input-autocomplete/input-autocomplete.component';
import { PromptObraNovaComponent, PromptObraNovaResult } from './prompt-obra-nova.component';

const LIMITE_SUGESTOES = 10;

@Component({
  selector: 'app-atualizar-posicao',
  standalone: true,
  imports: [CommonModule, FormsModule, InputAutocompleteComponent],
  templateUrl: './atualizar-posicao.component.html',
  styleUrl: './atualizar-posicao.component.scss',
})
export class AtualizarPosicaoComponent {
  valorIdentificador = '';
  /** Obra selecionada no autocomplete (id + nome); null quando o usuário digitou texto livre. */
  obraSelecionada: ObraBuscaItem | null = null;
  novaPosicao = 0;
  dataUltimaAtualizacao: string | null = null; // ISO date string ou vazio = hoje
  /** Estado preenchido pelo resultado do prompt "obra nova"; null quando não há criação pendente. */
  dadosCriacaoPendentes: { nome: string; tipo: TipoObra; ordemPreferencia: number } | null = null;

  sugestoes: ObraBuscaItem[] = [];

  /** Funções para o componente de autocomplete (tipadas como AutocompleteItem para o binding; uso interno é ObraBuscaItem). */
  displayWithObra = (item: AutocompleteItem): string => (item as ObraBuscaItem).nome;
  getItemIdObra = (item: AutocompleteItem): string => (item as ObraBuscaItem).id;

  preview: ObraDetalhe | null = null;
  erro: string | null = null;
  sucesso: string | null = null;
  carregando = false;

  constructor(
    private readonly atualizarPosicaoService: AtualizarPosicaoService,
    @Optional() private readonly listaObrasPort: IListaObrasPort | null,
    private readonly dialogService: DialogService,
    @Optional() @Inject(SAIDA_APOS_SUCESSO) private readonly saidaAposSucesso: SaidaAposSucesso | null,
    @Optional() private readonly dialogRef: DialogRef<{ salvou: boolean }> | null,
    @Optional() private readonly router: Router | null,
  ) {}

  get dataParaEnvio(): string | undefined {
    if (this.dataUltimaAtualizacao) return this.dataUltimaAtualizacao;
    return new Date().toISOString().slice(0, 10);
  }

  onNomeInput(valor: string): void {
    this.obraSelecionada = null;
    this.valorIdentificador = valor;
  }

  onSearchTerm(termo: string): void {
    if (!this.listaObrasPort) return;
    this.listaObrasPort
      .buscarPorNome(termo, LIMITE_SUGESTOES)
      .subscribe((items) => (this.sugestoes = items));
  }

  selecionarSugestao(item: AutocompleteItem): void {
    const obra = item as ObraBuscaItem;
    this.obraSelecionada = obra;
    this.valorIdentificador = obra.nome;
    this.sugestoes = [];
  }

  verPreview(): void {
    this.erro = null;
    this.preview = null;
    this.sucesso = null;
    const termoIdentificador = this.valorIdentificador.trim();
    if (!termoIdentificador) {
      this.erro = 'Informe o nome da obra.';
      return;
    }
    this.carregando = true;
    const obs = this.obraSelecionada
      ? this.atualizarPosicaoService.obterPorId(this.obraSelecionada.id)
      : this.atualizarPosicaoService.obterPorNome(termoIdentificador);
    obs.subscribe({
      next: (obra) => {
        this.preview = obra;
        this.carregando = false;
      },
      error: (err) => {
        this.carregando = false;
        if (err?.status === 404) {
          this.abrirPromptObraNovaParaPreview(termoIdentificador);
        } else {
          this.erro = err?.message ?? 'Erro ao buscar obra.';
        }
      },
    });
  }

  /**
   * Abre o prompt "obra nova" e, se o utilizador prosseguir, aplica o resultado e chama o callback.
   * Caso contrário, define erro "Obra não encontrada.".
   */
  private abrirPromptObraNova(termoIdentificador: string, onProsseguir: (termo: string) => void): void {
    const ref = this.dialogService.open<PromptObraNovaComponent, PromptObraNovaResult | undefined>(
      PromptObraNovaComponent,
      { data: { nomeDefault: termoIdentificador } }
    );
    ref.afterClosed.pipe(take(1)).subscribe((result) => {
      if (result?.prosseguir) {
        this.aplicarResultadoPromptObraNova(result);
        onProsseguir(termoIdentificador);
      } else {
        this.erro = 'Obra não encontrada.';
      }
    });
  }

  private abrirPromptObraNovaParaPreview(termoIdentificador: string): void {
    this.abrirPromptObraNova(termoIdentificador, (termo) => {
      this.preview = this.construirPreviaSinteticaObraNova(termo);
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
    const termoIdentificador = this.valorIdentificador.trim();
    if (!termoIdentificador) {
      this.erro = 'Informe o nome da obra.';
      return;
    }
    const temDadosCriacaoPendentes = this.dadosCriacaoPendentes != null;
    const request = this.montarRequest(termoIdentificador, temDadosCriacaoPendentes);
    this.carregando = true;
    this.atualizarPosicaoService.atualizarPosicao(request).subscribe({
      next: (res) => this.tratarSucessoSalvamento(res),
      error: (err) => {
        this.carregando = false;
        if (err?.status === 404) {
          this.abrirPromptObraNovaParaSalvar(termoIdentificador);
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

  private montarRequest(termoIdentificador: string, criarSeNaoExistir: boolean): AtualizarPosicaoRequest {
    const request: AtualizarPosicaoRequest = {
      novaPosicao: this.novaPosicao,
      criarSeNaoExistir,
    };
    if (this.obraSelecionada) request.idObra = this.obraSelecionada.id;
    else request.nomeObra = termoIdentificador;
    if (this.dataParaEnvio) request.dataUltimaAtualizacao = this.dataParaEnvio;
    if (criarSeNaoExistir && this.dadosCriacaoPendentes) {
      request.nomeParaCriar = this.dadosCriacaoPendentes.nome.trim() || termoIdentificador;
      request.tipoParaCriar = this.dadosCriacaoPendentes.tipo;
      request.ordemPreferenciaParaCriar = this.dadosCriacaoPendentes.ordemPreferencia;
    }
    return request;
  }

  private abrirPromptObraNovaParaSalvar(termoIdentificador: string): void {
    this.abrirPromptObraNova(termoIdentificador, (termo) =>
      this.executarSalvamentoObraNova(termo)
    );
  }

  private executarSalvamentoObraNova(termoIdentificador: string): void {
    const request = this.montarRequest(termoIdentificador, true);
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
  private construirPreviaSinteticaObraNova(termoIdentificador: string): ObraDetalhe {
    const d = this.dadosCriacaoPendentes;
    const dataEnvio = this.dataParaEnvio ?? new Date().toISOString().slice(0, 10);
    return {
      id: '',
      nome: d?.nome.trim() || termoIdentificador,
      tipo: d?.tipo ?? TipoObra.Manga,
      posicaoAtual: 0,
      dataUltimaAtualizacaoPosicao: dataEnvio,
      ordemPreferencia: d?.ordemPreferencia ?? 0,
      obraNova: true,
    };
  }
}
