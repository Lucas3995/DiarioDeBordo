import { Injectable } from '@angular/core';

const STORAGE_KEY_URL = 'diariodebordo_api_url';
const STORAGE_KEY_TOKEN = 'diariodebordo_api_token';

/**
 * Serviço singleton de configuração global da aplicação.
 *
 * Responsabilidade: persistir e recuperar a URL base da API (backend) e o
 * token JWT, ambos configurados em tela pelo usuário via localStorage.
 *
 * Os valores são usados por ListaObrasHttp (e futuras implementações HTTP) para
 * montar as requisições à API com o cabeçalho Authorization correto.
 *
 * Evolução futura: quando houver fluxo de login completo, o token passará a ser
 * gerenciado por um serviço de autenticação; esta classe ficará apenas com a URL.
 */
@Injectable({ providedIn: 'root' })
export class ApiConfigService {
  getApiUrl(): string {
    if (typeof window === 'undefined' || !window.localStorage) return '';
    return window.localStorage.getItem(STORAGE_KEY_URL) ?? '';
  }

  setApiUrl(url: string): void {
    if (typeof window === 'undefined' || !window.localStorage) return;
    if (url.trim()) {
      window.localStorage.setItem(STORAGE_KEY_URL, url.trim());
    } else {
      window.localStorage.removeItem(STORAGE_KEY_URL);
    }
  }

  getToken(): string {
    if (typeof window === 'undefined' || !window.localStorage) return '';
    return window.localStorage.getItem(STORAGE_KEY_TOKEN) ?? '';
  }

  setToken(token: string): void {
    if (typeof window === 'undefined' || !window.localStorage) return;
    if (token.trim()) {
      window.localStorage.setItem(STORAGE_KEY_TOKEN, token.trim());
    } else {
      window.localStorage.removeItem(STORAGE_KEY_TOKEN);
    }
  }
}
