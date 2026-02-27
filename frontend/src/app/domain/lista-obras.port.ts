import { Observable } from 'rxjs';
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

/**
 * Porta (interface de saída) para a query de listagem paginada de obras.
 * Segue o princípio CQRS: esta porta é exclusiva para a query de listagem;
 * detalhes de uma obra e operações de escrita usam outras portas.
 *
 * Implementações:
 * - `ListaObrasMock` (infrastructure/): dados fixos para desenvolvimento/testes.
 * - `ListaObrasHttp` (infrastructure/): chamada à API real quando o backend estiver pronto.
 */
export abstract class IListaObrasPort {
  abstract listarPagina(params: ListaObrasParams): Observable<ListaObrasResult>;
}
