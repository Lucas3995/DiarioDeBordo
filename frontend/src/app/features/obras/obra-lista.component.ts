import { Component, OnInit, DestroyRef, inject, signal, computed, ChangeDetectionStrategy } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { SlicePipe } from '@angular/common';
import { ListaObrasService } from '../../application/lista-obras.service';
import { formatarDataRelativa, formatarDataTooltip } from '../../domain/datas';
import { ObraListItem } from '../../domain/obra-list-item';
import { formatarPosicao } from '../../domain/obra.types';
import { DialogService } from '../../shared/dialog/dialog.service';
import {
  SAIDA_APOS_SUCESSO,
  DELAY_FECHAMENTO_APOS_SUCESSO_MS,
} from '../../shared/dialog/saida-apos-sucesso';
import { AtualizarPosicaoComponent } from './atualizar-posicao/atualizar-posicao.component';

/** Opções disponíveis para pageSize. */
export const PAGE_SIZE_OPTIONS = [10, 25, 50, 100] as const;

@Component({
  selector: 'app-obra-lista',
  standalone: true,
  imports: [SlicePipe],
  templateUrl: './obra-lista.component.html',
  styleUrl: './obra-lista.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ObraListaComponent implements OnInit {
  readonly obras = signal<ObraListItem[]>([]);
  readonly totalCount = signal(0);
  readonly pageIndex = signal(0);
  readonly pageSize = signal(10);
  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;

  /** Funções de domínio expostas ao template (evita middle man). */
  readonly formatarPosicao = formatarPosicao;
  readonly formatarDataRelativa = formatarDataRelativa;
  readonly formatarDataTooltip = formatarDataTooltip;

  readonly temProximaPagina = computed(() => (this.pageIndex() + 1) * this.pageSize() < this.totalCount());
  readonly primeiroItemDaPagina = computed(() => this.totalCount() === 0 ? 0 : this.pageIndex() * this.pageSize() + 1);
  readonly ultimoItemDaPagina = computed(() => Math.min((this.pageIndex() + 1) * this.pageSize(), this.totalCount()));

  private readonly destroyRef = inject(DestroyRef);

  constructor(
    private readonly listaObrasService: ListaObrasService,
    private readonly dialogService: DialogService,
  ) {}

  abrirDialogAtualizarPosicao(): void {
    const ref = this.dialogService.open<
      AtualizarPosicaoComponent,
      { salvou?: boolean }
    >(AtualizarPosicaoComponent, {
      getProviders: (dialogRef) => [
        {
          provide: SAIDA_APOS_SUCESSO,
          useValue: {
            fecharComSucesso: () =>
              setTimeout(
                () => dialogRef.close({ salvou: true }),
                DELAY_FECHAMENTO_APOS_SUCESSO_MS
              ),
          },
        },
      ],
    });
    ref.afterClosed
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((result) => {
        if (result?.salvou) {
          this.carregarPagina();
        }
      });
  }

  ngOnInit(): void {
    this.carregarPagina();
  }

  carregarPagina(): void {
    this.listaObrasService
      .listarPagina({ pageIndex: this.pageIndex(), pageSize: this.pageSize() })
      .subscribe((result) => {
        this.obras.set(result.items);
        this.totalCount.set(result.totalCount);
      });
  }

  irParaProximaPagina(): void {
    if (!this.temProximaPagina()) return;
    this.pageIndex.update(i => i + 1);
    this.carregarPagina();
  }

  irParaPaginaAnterior(): void {
    if (this.pageIndex() <= 0) return;
    this.pageIndex.update(i => i - 1);
    this.carregarPagina();
  }

  mudarPageSize(novoPageSize: number): void {
    this.pageSize.set(novoPageSize);
    this.pageIndex.set(0);
    this.carregarPagina();
  }
}
