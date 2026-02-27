import { Injectable } from '@angular/core';

const STORAGE_KEY = 'diariodebordo_api_url';

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
