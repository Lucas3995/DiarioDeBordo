import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ListaObrasService } from '../../application/lista-obras.service';
import { ObraListItem } from '../../domain/obra-list-item';
import { formatarPosicao } from '../../domain/obra.types';

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
    const hoje = new Date();
    const diff = hoje.getTime() - data.getTime();
    const dias = Math.floor(diff / (1000 * 60 * 60 * 24));

    if (dias === 0) return 'hoje';
    if (dias === 1) return 'há 1 dia';
    if (dias < 30) return `há ${dias} dias`;
    if (dias < 60) return 'há 1 mês';
    if (dias < 365) return `há ${Math.floor(dias / 30)} meses`;
    if (dias < 730) return 'há 1 ano';
    return `há ${Math.floor(dias / 365)} anos`;
  }

  formatarDataTooltip(data: Date): string {
    const d = data.getDate().toString().padStart(2, '0');
    const m = (data.getMonth() + 1).toString().padStart(2, '0');
    const y = data.getFullYear();
    return `${d}/${m}/${y}`;
  }
}
