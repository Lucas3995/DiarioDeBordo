import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiConfigService } from '../core/api-config.service';
import { TipoObra } from '../domain/obra.types';

/** Dados atuais da obra retornados pela API (prévia). */
export interface ObraDetalhe {
  id: string;
  nome: string;
  tipo: TipoObra;
  posicaoAtual: number;
  dataUltimaAtualizacaoPosicao: string; // ISO 8601
  ordemPreferencia: number;
}

/** Payload para PATCH /api/obras/posicao. */
export interface AtualizarPosicaoRequest {
  idObra?: string;
  nomeObra?: string;
  novaPosicao: number;
  dataUltimaAtualizacao?: string; // ISO 8601
  criarSeNaoExistir: boolean;
  nomeParaCriar?: string;
  tipoParaCriar?: TipoObra;
  ordemPreferenciaParaCriar?: number;
}

/** Resposta do PATCH /api/obras/posicao. */
export interface AtualizarPosicaoResponse {
  id: string;
  criada: boolean;
}

/**
 * Serviço para obter prévia de uma obra e atualizar sua posição.
 * Consome GET /api/obras/:id, GET /api/obras/por-nome e PATCH /api/obras/posicao.
 */
@Injectable()
export class AtualizarPosicaoService {
  constructor(
    private readonly http: HttpClient,
    private readonly apiConfig: ApiConfigService,
  ) {}

  /** Obtém os dados atuais da obra por id (para prévia). */
  obterPorId(id: string): Observable<ObraDetalhe> {
    return this.getObra(`${this.baseUrl()}/api/obras/${encodeURIComponent(id)}`);
  }

  /** Obtém os dados atuais da obra por nome (para prévia). */
  obterPorNome(nome: string): Observable<ObraDetalhe> {
    const params = new HttpParams().set('nome', nome);
    return this.getObra(`${this.baseUrl()}/api/obras/por-nome`, params);
  }

  /** Atualiza a posição (ou cria obra se não existir). */
  atualizarPosicao(request: AtualizarPosicaoRequest): Observable<AtualizarPosicaoResponse> {
    const apiUrl = this.apiConfig.getApiUrl();
    const token = this.apiConfig.getToken();
    if (!apiUrl) {
      return throwError(() => new Error('URL da API não configurada.'));
    }
    const url = `${apiUrl.replace(/\/$/, '')}/api/obras/posicao`;
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: token ? `Bearer ${token}` : '',
    });
    return this.http
      .patch<AtualizarPosicaoResponse>(url, request, { headers })
      .pipe(catchError((err) => throwError(() => err)));
  }

  private baseUrl(): string {
    const url = this.apiConfig.getApiUrl();
    if (!url) throw new Error('URL da API não configurada.');
    return url.replace(/\/$/, '');
  }

  private getObra(url: string, params?: HttpParams): Observable<ObraDetalhe> {
    const token = this.apiConfig.getToken();
    const headers = new HttpHeaders({
      Authorization: token ? `Bearer ${token}` : '',
    });
    return this.http
      .get<ObraDetalhe>(url, { headers, params: params ?? undefined })
      .pipe(catchError((err) => throwError(() => err)));
  }
}
