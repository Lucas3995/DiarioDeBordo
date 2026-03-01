import { Component, OnInit, DestroyRef, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
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
  imports: [CommonModule, FormsModule],
  templateUrl: './obra-lista.component.html',
  styleUrl: './obra-lista.component.scss',
})
export class ObraListaComponent implements OnInit {
  obras: ObraListItem[] = [];
  totalCount = 0;
  pageIndex = 0;
  pageSize = 10;
  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;

  /** Funções de domínio expostas ao template (evita middle man). */
  readonly formatarPosicao = formatarPosicao;
  readonly formatarDataRelativa = formatarDataRelativa;
  readonly formatarDataTooltip = formatarDataTooltip;

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
      .listarPagina({ pageIndex: this.pageIndex, pageSize: this.pageSize })
      .subscribe((result) => {
        this.obras = result.items;
        this.totalCount = result.totalCount;
      });
  }

  irParaProximaPagina(): void {
    if (!this.temProximaPagina) return;
    this.pageIndex++;
    this.carregarPagina();
  }

  irParaPaginaAnterior(): void {
    if (this.pageIndex <= 0) return;
    this.pageIndex--;
    this.carregarPagina();
  }

  mudarPageSize(novoPageSize: number): void {
    this.pageSize = novoPageSize;
    this.pageIndex = 0;
    this.carregarPagina();
  }

  get temProximaPagina(): boolean {
    return (this.pageIndex + 1) * this.pageSize < this.totalCount;
  }

  get primeiroItemDaPagina(): number {
    return this.totalCount === 0 ? 0 : this.pageIndex * this.pageSize + 1;
  }

  get ultimoItemDaPagina(): number {
    return Math.min((this.pageIndex + 1) * this.pageSize, this.totalCount);
  }
}
