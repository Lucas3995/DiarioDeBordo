import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { ApiConfigService } from '../core/api-config.service';
import { IAuthPort } from '../domain/auth.port';
import { LoginParams, LoginResult } from '../domain/auth.types';

/**
 * Caso de uso: Login.
 *
 * Orquestra a chamada à porta de auth e armazena o token via ApiConfigService
 * em caso de sucesso. Segue CQRS: este é um comando, não retorna dados de negócio
 * além do resultado da autenticação.
 */
@Injectable()
export class AuthService {
  constructor(
    private readonly authPort: IAuthPort,
    private readonly apiConfig: ApiConfigService,
  ) {}

  login(params: LoginParams): Observable<LoginResult> {
    return this.authPort.login(params).pipe(
      tap((result) => {
        if (result.sucesso && result.token) {
          this.apiConfig.setToken(result.token);
        }
      }),
    );
  }
}
