import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ApiConfigService } from '../core/api-config.service';
import {
  IAtualizarPosicaoPort,
  ObraDetalhe,
  AtualizarPosicaoRequest,
  AtualizarPosicaoResponse,
} from '../domain/atualizar-posicao.port';

@Injectable()
export class AtualizarPosicaoHttp extends IAtualizarPosicaoPort {
  constructor(
    private readonly http: HttpClient,
    private readonly apiConfig: ApiConfigService,
  ) {
    super();
  }

  obterPorId(id: string): Observable<ObraDetalhe> {
    return this.getObra(`${this.baseUrl()}/api/obras/${encodeURIComponent(id)}`);
  }

  obterPorNome(nome: string): Observable<ObraDetalhe> {
    const params = new HttpParams().set('nome', nome);
    return this.getObra(`${this.baseUrl()}/api/obras/por-nome`, params);
  }

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
