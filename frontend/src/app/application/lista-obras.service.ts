import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  IListaObrasPort,
  ListaObrasParams,
  ListaObrasResult,
  ObraBuscaItem,
} from '../domain/lista-obras.port';

/**
 * Caso de uso: Listar Obras (paginado).
 *
 * Responsabilidade exclusiva: delegar a query de listagem paginada à porta,
 * mantendo isolamento de outras ações (detalhes, comandos de escrita).
 * Não mistura lógica de leitura de listagem com outros casos de uso (CQRS).
 */
@Injectable()
export class ListaObrasService {
  constructor(private readonly porta: IListaObrasPort) {}

  listarPagina(params: ListaObrasParams): Observable<ListaObrasResult> {
    return this.porta.listarPagina(params);
  }

  /** Busca obras por termo no nome para autocomplete (Demanda 4). */
  buscarPorNome(termo: string, limit = 10): Observable<ObraBuscaItem[]> {
    return this.porta.buscarPorNome(termo, limit);
  }
}
