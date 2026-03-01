import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ListaObrasService } from '../../application/lista-obras.service';
import { formatarDataRelativa, formatarDataTooltip } from '../../domain/datas';
import { ObraListItem } from '../../domain/obra-list-item';
import { formatarPosicao } from '../../domain/obra.types';

/** Opções disponíveis para pageSize. */
export const PAGE_SIZE_OPTIONS = [10, 25, 50, 100] as const;

@Component({
  selector: 'app-obra-lista',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './obra-lista.component.html',
  styleUrl: './obra-lista.component.scss',
})
export class ObraListaComponent implements OnInit {
  obras: ObraListItem[] = [];
  totalCount = 0;
  pageIndex = 0;
  pageSize = 10;
  readonly pageSizeOptions = PAGE_SIZE_OPTIONS;

  constructor(private readonly listaObrasService: ListaObrasService) {}

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

  formatarPosicaoObra(obra: ObraListItem): string {
    return formatarPosicao(obra.posicaoAtual, obra.tipo);
  }

  formatarDataRelativa(data: Date): string {
    return formatarDataRelativa(data);
  }

  formatarDataTooltip(data: Date): string {
    return formatarDataTooltip(data);
  }
}
