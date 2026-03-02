import type { Observable } from 'rxjs';
import { ObraListItem } from './obra-list-item';

/** Parâmetros de paginação para a listagem de obras. */
export interface ListaObrasParams {
  /** Índice da página (base 0). */
  pageIndex: number;
  /** Número de itens por página. Valores válidos: 10, 25, 50, 100. */
  pageSize: number;
}

/** Resultado paginado da listagem de obras. */
export interface ListaObrasResult {
  /** Itens da página atual, já ordenados por `ordemPreferencia`. */
  items: ObraListItem[];
  /** Total de obras no sistema (para calcular número de páginas). */
  totalCount: number;
}

/** Item reduzido para autocomplete (buscar por nome). */
export interface ObraBuscaItem {
  id: string;
  nome: string;
}

/**
 * Porta (interface de saída) para a query de listagem paginada de obras e busca por nome.
 * Segue o princípio CQRS: esta porta é exclusiva para as queries de listagem e busca;
 * detalhes de uma obra e operações de escrita usam outras portas.
 *
 * Implementações:
 * - `ListaObrasMock` (infrastructure/): dados fixos para desenvolvimento/testes.
 * - `ListaObrasHttp` (infrastructure/): chamada à API real quando o backend estiver pronto.
 */
export abstract class IListaObrasPort {
  abstract listarPagina(params: ListaObrasParams): Observable<ListaObrasResult>;

  /**
   * Busca obras por termo no nome para autocomplete.
   * Retorna lista reduzida (id, nome) com até limit itens.
   */
  abstract buscarPorNome(termo: string, limit?: number): Observable<ObraBuscaItem[]>;
}
