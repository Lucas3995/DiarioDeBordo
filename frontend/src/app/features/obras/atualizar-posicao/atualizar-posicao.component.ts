import { Component, Optional, Inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl } from '@angular/forms';
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
  imports: [ReactiveFormsModule, InputAutocompleteComponent],
  templateUrl: './atualizar-posicao.component.html',
  styleUrl: './atualizar-posicao.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AtualizarPosicaoComponent {
  readonly valorIdentificador = signal('');
  /** Obra selecionada no autocomplete (id + nome); null quando o usuário digitou texto livre. */
  readonly obraSelecionada = signal<ObraBuscaItem | null>(null);
  readonly form = new FormGroup({
    novaPosicao: new FormControl(0, { nonNullable: true }),
    dataUltimaAtualizacao: new FormControl<string | null>(null),
  });
  /** Estado preenchido pelo resultado do prompt "obra nova"; null quando não há criação pendente. */
  readonly dadosCriacaoPendentes = signal<{ nome: string; tipo: TipoObra; ordemPreferencia: number } | null>(null);

  readonly sugestoes = signal<ObraBuscaItem[]>([]);

  /** Funções para o componente de autocomplete (tipadas como AutocompleteItem para o binding; uso interno é ObraBuscaItem). */
  readonly displayWithObra = (item: AutocompleteItem): string => (item as ObraBuscaItem).nome;
  readonly getItemIdObra = (item: AutocompleteItem): string => (item as ObraBuscaItem).id;

  readonly preview = signal<ObraDetalhe | null>(null);
  readonly erro = signal<string | null>(null);
  readonly sucesso = signal<string | null>(null);
  readonly carregando = signal(false);

  constructor(
    private readonly atualizarPosicaoService: AtualizarPosicaoService,
    @Optional() private readonly listaObrasPort: IListaObrasPort | null,
    private readonly dialogService: DialogService,
    @Optional() @Inject(SAIDA_APOS_SUCESSO) private readonly saidaAposSucesso: SaidaAposSucesso | null,
    @Optional() private readonly dialogRef: DialogRef<{ salvou: boolean }> | null,
    @Optional() private readonly router: Router | null,
  ) {}

  get dataParaEnvio(): string | undefined {
    const data = this.form.controls.dataUltimaAtualizacao.value;
    if (data) return data;
    return new Date().toISOString().slice(0, 10);
  }

  onNomeInput(valor: string): void {
    this.obraSelecionada.set(null);
    this.valorIdentificador.set(valor);
  }

  onSearchTerm(termo: string): void {
    if (!this.listaObrasPort) return;
    this.listaObrasPort
      .buscarPorNome(termo, LIMITE_SUGESTOES)
      .subscribe((items) => this.sugestoes.set(items));
  }

  selecionarSugestao(item: AutocompleteItem): void {
    const obra = item as ObraBuscaItem;
    this.obraSelecionada.set(obra);
    this.valorIdentificador.set(obra.nome);
    this.sugestoes.set([]);
  }

  verPreview(): void {
    this.erro.set(null);
    this.preview.set(null);
    this.sucesso.set(null);
    const termoIdentificador = this.valorIdentificador().trim();
    if (!termoIdentificador) {
      this.erro.set('Informe o nome da obra.');
      return;
    }
    this.carregando.set(true);
    const obra = this.obraSelecionada();
    const obs = obra
      ? this.atualizarPosicaoService.obterPorId(obra.id)
      : this.atualizarPosicaoService.obterPorNome(termoIdentificador);
    obs.subscribe({
      next: (obraDetalhe) => {
        this.preview.set(obraDetalhe);
        this.carregando.set(false);
      },
      error: (err) => {
        this.carregando.set(false);
        if (err?.status === 404) {
          this.abrirPromptObraNovaParaPreview(termoIdentificador);
        } else {
          this.erro.set(err?.message ?? 'Erro ao buscar obra.');
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
        this.erro.set('Obra não encontrada.');
      }
    });
  }

  private abrirPromptObraNovaParaPreview(termoIdentificador: string): void {
    this.abrirPromptObraNova(termoIdentificador, (termo) => {
      this.preview.set(this.construirPreviaSinteticaObraNova(termo));
    });
  }

  private aplicarResultadoPromptObraNova(result: PromptObraNovaResult): void {
    this.dadosCriacaoPendentes.set({
      nome: result.nome,
      tipo: result.tipo,
      ordemPreferencia: result.ordemPreferencia,
    });
  }

  salvar(): void {
    this.erro.set(null);
    this.sucesso.set(null);
    const termoIdentificador = this.valorIdentificador().trim();
    if (!termoIdentificador) {
      this.erro.set('Informe o nome da obra.');
      return;
    }
    const temDadosCriacaoPendentes = this.dadosCriacaoPendentes() != null;
    const request = this.montarRequest(termoIdentificador, temDadosCriacaoPendentes);
    this.carregando.set(true);
    this.atualizarPosicaoService.atualizarPosicao(request).subscribe({
      next: (res) => this.tratarSucessoSalvamento(res),
      error: (err) => {
        this.carregando.set(false);
        if (err?.status === 404) {
          this.abrirPromptObraNovaParaSalvar(termoIdentificador);
        } else {
          this.erro.set(err?.error?.message ?? err?.message ?? 'Erro ao atualizar.');
        }
      },
    });
  }

  private tratarSucessoSalvamento(res: AtualizarPosicaoResponse): void {
    this.carregando.set(false);
    this.sucesso.set(res.criada ? 'Obra criada e posição registrada.' : 'Posição atualizada.');
    this.preview.set(null);
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
      novaPosicao: this.form.controls.novaPosicao.value,
      criarSeNaoExistir,
    };
    const obra = this.obraSelecionada();
    if (obra) request.idObra = obra.id;
    else request.nomeObra = termoIdentificador;
    if (this.dataParaEnvio) request.dataUltimaAtualizacao = this.dataParaEnvio;
    const dados = this.dadosCriacaoPendentes();
    if (criarSeNaoExistir && dados) {
      request.nomeParaCriar = dados.nome.trim() || termoIdentificador;
      request.tipoParaCriar = dados.tipo;
      request.ordemPreferenciaParaCriar = dados.ordemPreferencia;
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
    this.carregando.set(true);
    this.atualizarPosicaoService.atualizarPosicao(request).subscribe({
      next: (res) => this.tratarSucessoSalvamento(res),
      error: (err) => {
        this.carregando.set(false);
        this.erro.set(err?.error?.message ?? err?.message ?? 'Erro ao atualizar.');
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
    const d = this.dadosCriacaoPendentes();
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
