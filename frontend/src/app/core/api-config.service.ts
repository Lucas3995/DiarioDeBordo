import { Injectable } from '@angular/core';

const STORAGE_KEY = 'diariodebordo_api_url';

/**
 * Serviço singleton de configuração global da aplicação.
 *
 * Responsabilidade: persistir e recuperar a URL base da API (backend)
 * configurada em tela pelo usuário administrador via localStorage.
 *
 * Evolução futura: caso a configuração cresça (múltiplas chaves, perfis de
 * ambiente, validações de URL), mover lógica de negócio para um serviço em
 * application/ e manter aqui apenas o acesso ao localStorage (infrastructure/).
 */
@Injectable({ providedIn: 'root' })
export class ApiConfigService {
  getApiUrl(): string {
    if (typeof window === 'undefined' || !window.localStorage) return '';
    return window.localStorage.getItem(STORAGE_KEY) ?? '';
  }

  setApiUrl(url: string): void {
    if (typeof window === 'undefined' || !window.localStorage) return;
    if (url.trim()) {
      window.localStorage.setItem(STORAGE_KEY, url.trim());
    } else {
      window.localStorage.removeItem(STORAGE_KEY);
    }
  }
}
