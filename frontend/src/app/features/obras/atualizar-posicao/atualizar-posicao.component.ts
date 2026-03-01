import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AtualizarPosicaoService, ObraDetalhe, AtualizarPosicaoRequest } from '../../../application/atualizar-posicao.service';
import { TipoObra, ROTULO_POSICAO } from '../../../domain/obra.types';
import { formatarDataRelativa, formatarDataTooltip as formatarDataTooltipDomain } from '../../../domain/datas';

@Component({
  selector: 'app-atualizar-posicao',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './atualizar-posicao.component.html',
  styleUrl: './atualizar-posicao.component.scss',
})
export class AtualizarPosicaoComponent {
  /** Identificador: 'id' ou 'nome'. */
  tipoIdentificador: 'id' | 'nome' = 'nome';
  valorIdentificador = '';
  novaPosicao = 0;
  dataUltimaAtualizacao: string | null = null; // ISO date string ou vazio = hoje
  criarSeNaoExistir = false;
  nomeParaCriar = '';
  tipoParaCriar: TipoObra = TipoObra.Manga;
  ordemPreferenciaParaCriar = 0;

  preview: ObraDetalhe | null = null;
  erro: string | null = null;
  sucesso: string | null = null;
  carregando = false;

  readonly tiposObra = Object.values(TipoObra);

  constructor(
    private readonly atualizarPosicaoService: AtualizarPosicaoService,
    private readonly router: Router,
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
        if (err?.status === 404 && this.criarSeNaoExistir) {
          this.preview = this.construirPreviaSinteticaObraNova(id);
        } else if (err?.status === 404) {
          this.erro = 'Obra não encontrada. Marque "Criar se não existir" para cadastrar.';
        } else {
          this.erro = err?.message ?? 'Erro ao buscar obra.';
        }
      },
    });
  }

  salvar(): void {
    this.erro = null;
    this.sucesso = null;
    const id = this.valorIdentificador.trim();
    if (!id) {
      this.erro = 'Informe o código (id) ou o nome da obra.';
      return;
    }
    const request: AtualizarPosicaoRequest = {
      novaPosicao: this.novaPosicao,
      criarSeNaoExistir: this.criarSeNaoExistir,
    };
    if (this.tipoIdentificador === 'id') request.idObra = id;
    else request.nomeObra = id;
    if (this.dataParaEnvio) request.dataUltimaAtualizacao = this.dataParaEnvio;
    if (this.criarSeNaoExistir) {
      request.nomeParaCriar = this.nomeParaCriar.trim() || id;
      request.tipoParaCriar = this.tipoParaCriar;
      request.ordemPreferenciaParaCriar = this.ordemPreferenciaParaCriar;
    }
    this.carregando = true;
    this.atualizarPosicaoService.atualizarPosicao(request).subscribe({
      next: (res) => {
        this.carregando = false;
        this.sucesso = res.criada ? 'Obra criada e posição registrada.' : 'Posição atualizada.';
        this.preview = null;
        setTimeout(() => this.router.navigate(['/obras']), 1500);
      },
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

  /** Constrói prévia sintética para obra que ainda não existe (404 + criarSeNaoExistir). */
  private construirPreviaSinteticaObraNova(id: string): ObraDetalhe {
    return {
      id: '',
      nome: this.nomeParaCriar.trim() || id,
      tipo: this.tipoParaCriar,
      posicaoAtual: 0,
      dataUltimaAtualizacaoPosicao: this.dataParaEnvio!,
      ordemPreferencia: this.ordemPreferenciaParaCriar,
      obraNova: true,
    };
  }
}
