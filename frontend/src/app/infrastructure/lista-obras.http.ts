import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { IListaObrasPort, ListaObrasParams, ListaObrasResult, ObraBuscaItem } from '../domain/lista-obras.port';
import { ObraListItem } from '../domain/obra-list-item';
import { ProximaInfo, TipoObra } from '../domain/obra.types';
import { ApiConfigService } from '../core/api-config.service';

/**
 * DTO de item de obra recebido do backend (contrato da API).
 * Reflete exatamente o shape do ObraAcompanhamentoListItemDto do .NET.
 */
interface ObraListItemApiDto {
  id: string;
  nome: string;
  tipo: string;
  ultimaAtualizacaoPosicao: string; // ISO 8601 UTC — convertido para Date no mapeamento
  posicaoAtual: number;
  proximaInfo?: {
    tipo: string;
    dias?: number;
    quantidade?: number;
  };
  ordemPreferencia: number;
}

/** DTO da resposta paginada recebida do backend. */
interface ObrasAcompanhamentoListResponseDto {
  items: ObraListItemApiDto[];
  totalCount: number;
}

/**
 * Implementação HTTP de IListaObrasPort.
 * Consome o endpoint GET /api/obras do backend real, enviando o cabeçalho
 * Authorization com o token JWT configurado em tela pelo usuário (Configurações).
 *
 * Substitui ListaObrasMock quando a configuração de apiUrl e jwtToken estiver presente.
 */
@Injectable()
export class ListaObrasHttp extends IListaObrasPort {
  constructor(
    private readonly http: HttpClient,
    private readonly apiConfig: ApiConfigService,
  ) {
    super();
  }

  listarPagina(params: ListaObrasParams): Observable<ListaObrasResult> {
    const apiUrl = this.apiConfig.getApiUrl();
    const token = this.apiConfig.getToken();

    if (!apiUrl) {
      return throwError(() => new Error('URL da API não configurada. Acesse Configurações e informe a URL do backend.'));
    }

    const url = `${apiUrl.replace(/\/$/, '')}/api/obras`;

    const headers = new HttpHeaders({
      Authorization: token ? `Bearer ${token}` : '',
    });

    const queryParams = new HttpParams()
      .set('pageIndex', params.pageIndex.toString())
      .set('pageSize', params.pageSize.toString());

    return this.http
      .get<ObrasAcompanhamentoListResponseDto>(url, { headers, params: queryParams })
      .pipe(
        map((response) => ({
          items: response.items.map(this.mapearItem),
          totalCount: response.totalCount,
        })),
        catchError((err) => throwError(() => err)),
      );
  }

  buscarPorNome(termo: string, limit = 10): Observable<ObraBuscaItem[]> {
    const apiUrl = this.apiConfig.getApiUrl();
    const token = this.apiConfig.getToken();

    if (!apiUrl) {
      return throwError(() => new Error('URL da API não configurada. Acesse Configurações e informe a URL do backend.'));
    }

    const url = `${apiUrl.replace(/\/$/, '')}/api/obras/buscar`;
    const headers = new HttpHeaders({
      Authorization: token ? `Bearer ${token}` : '',
    });
    const params = new HttpParams()
      .set('q', termo.trim())
      .set('limit', Math.min(Math.max(1, limit), 50).toString());

    return this.http
      .get<ObraBuscaItem[]>(url, { headers, params })
      .pipe(catchError((err) => throwError(() => err)));
  }

  private mapearItem(dto: ObraListItemApiDto): ObraListItem {
    const proximaInfo = ListaObrasHttp.mapearProximaInfo(dto.proximaInfo);

    return {
      id: dto.id,
      nome: dto.nome,
      tipo: dto.tipo as TipoObra,
      // Backend envia em UTC (ISO 8601). O Date do JS interpreta como UTC.
      // A exibição em horário de Brasília (UTC-3) é responsabilidade dos componentes de UI.
      ultimaAtualizacaoPosicao: new Date(dto.ultimaAtualizacaoPosicao),
      posicaoAtual: dto.posicaoAtual,
      proximaInfo,
      ordemPreferencia: dto.ordemPreferencia,
    };
  }

  private static mapearProximaInfo(
    dto?: { tipo: string; dias?: number; quantidade?: number },
  ): ProximaInfo | undefined {
    if (!dto) return undefined;

    if (dto.tipo === 'dias_ate_proxima' && dto.dias !== undefined) {
      return { tipo: 'dias_ate_proxima', dias: dto.dias };
    }

    if (dto.tipo === 'partes_ja_publicadas' && dto.quantidade !== undefined) {
      return { tipo: 'partes_ja_publicadas', quantidade: dto.quantidade };
    }

    return undefined;
  }
}
