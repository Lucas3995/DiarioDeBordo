import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { IListaObrasPort, ListaObrasParams, ListaObrasResult } from '../domain/lista-obras.port';
import { ObraListItem } from '../domain/obra-list-item';
import { TipoObra } from '../domain/obra.types';

/**
 * Implementação mock de IListaObrasPort para desenvolvimento e testes.
 * Retorna dados fixos paginados com exemplos de todos os tipos de obra
 * e variações de proximaInfo.
 *
 * Substitua por ListaObrasHttp quando o backend estiver pronto.
 */
@Injectable()
export class ListaObrasMock extends IListaObrasPort {
  private static readonly LISTA_FIXA: ObraListItem[] = [
    {
      id: '1',
      nome: 'One Piece',
      tipo: TipoObra.Manga,
      ultimaAtualizacaoPosicao: new Date('2026-02-20'),
      posicaoAtual: 1110,
      ordemPreferencia: 1,
      proximaInfo: { tipo: 'dias_ate_proxima', dias: 5 },
    },
    {
      id: '2',
      nome: 'Solo Leveling',
      tipo: TipoObra.Manhwa,
      ultimaAtualizacaoPosicao: new Date('2026-02-15'),
      posicaoAtual: 179,
      ordemPreferencia: 2,
      proximaInfo: { tipo: 'dias_ate_proxima', dias: 7 },
    },
    {
      id: '3',
      nome: 'Fullmetal Alchemist',
      tipo: TipoObra.Anime,
      ultimaAtualizacaoPosicao: new Date('2026-01-30'),
      posicaoAtual: 64,
      ordemPreferencia: 3,
    },
    {
      id: '4',
      nome: 'The Beginning After the End',
      tipo: TipoObra.Webnovel,
      ultimaAtualizacaoPosicao: new Date('2026-02-10'),
      posicaoAtual: 320,
      ordemPreferencia: 4,
      proximaInfo: { tipo: 'partes_ja_publicadas', quantidade: 3 },
    },
    {
      id: '5',
      nome: 'Harry Potter e o Cálice de Fogo',
      tipo: TipoObra.Livro,
      ultimaAtualizacaoPosicao: new Date('2026-01-15'),
      posicaoAtual: 37,
      ordemPreferencia: 5,
    },
    {
      id: '6',
      nome: 'Breaking Bad',
      tipo: TipoObra.Serie,
      ultimaAtualizacaoPosicao: new Date('2026-02-01'),
      posicaoAtual: 47,
      ordemPreferencia: 6,
      proximaInfo: { tipo: 'partes_ja_publicadas', quantidade: 1 },
    },
    {
      id: '7',
      nome: 'Interstellar',
      tipo: TipoObra.Filme,
      ultimaAtualizacaoPosicao: new Date('2025-12-20'),
      posicaoAtual: 95,
      ordemPreferencia: 7,
    },
    {
      id: '8',
      nome: 'Demon Slayer',
      tipo: TipoObra.Anime,
      ultimaAtualizacaoPosicao: new Date('2026-02-25'),
      posicaoAtual: 44,
      ordemPreferencia: 8,
      proximaInfo: { tipo: 'dias_ate_proxima', dias: 14 },
    },
    {
      id: '9',
      nome: 'Tower of God',
      tipo: TipoObra.Manhwa,
      ultimaAtualizacaoPosicao: new Date('2026-02-18'),
      posicaoAtual: 605,
      ordemPreferencia: 9,
      proximaInfo: { tipo: 'dias_ate_proxima', dias: 7 },
    },
    {
      id: '10',
      nome: 'O Nome do Vento',
      tipo: TipoObra.Livro,
      ultimaAtualizacaoPosicao: new Date('2026-01-05'),
      posicaoAtual: 22,
      ordemPreferencia: 10,
    },
    {
      id: '11',
      nome: 'Attack on Titan',
      tipo: TipoObra.Manga,
      ultimaAtualizacaoPosicao: new Date('2026-01-20'),
      posicaoAtual: 139,
      ordemPreferencia: 11,
    },
    {
      id: '12',
      nome: 'Vinland Saga',
      tipo: TipoObra.Manga,
      ultimaAtualizacaoPosicao: new Date('2026-02-05'),
      posicaoAtual: 198,
      ordemPreferencia: 12,
      proximaInfo: { tipo: 'partes_ja_publicadas', quantidade: 2 },
    },
  ];

  listarPagina(params: ListaObrasParams): Observable<ListaObrasResult> {
    const lista = ListaObrasMock.LISTA_FIXA;
    const inicio = params.pageIndex * params.pageSize;
    const fim = inicio + params.pageSize;
    const items = lista.slice(inicio, fim);

    return of({
      items,
      totalCount: lista.length,
    });
  }
}
