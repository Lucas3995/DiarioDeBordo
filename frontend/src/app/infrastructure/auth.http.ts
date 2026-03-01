import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { ApiConfigService } from '../core/api-config.service';
import { IAuthPort } from '../domain/auth.port';
import { LoginParams, LoginResult } from '../domain/auth.types';

@Injectable()
export class AuthHttp implements IAuthPort {
  constructor(
    private readonly http: HttpClient,
    private readonly apiConfig: ApiConfigService,
  ) {}

  login(params: LoginParams): Observable<LoginResult> {
    const apiUrl = this.apiConfig.getApiUrl();
    if (!apiUrl) {
      return throwError(() => new Error('URL da API não configurada. Acesse Configurações para definir a URL.'));
    }

    return this.http.post<LoginResult>(`${apiUrl}/api/auth/login`, params);
  }
}
