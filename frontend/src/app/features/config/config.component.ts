import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiConfigService } from '../../core/api-config.service';

@Component({
  selector: 'app-config',
  standalone: true,
  imports: [FormsModule],
  template: `
    <h2>Configurações (Admin)</h2>
    <p>A URL da API é configurada aqui e armazenada localmente no navegador.</p>
    <label for="apiUrl">URL da API (backend):</label>
    <input
      id="apiUrl"
      type="url"
      [(ngModel)]="apiUrl"
      placeholder="https://api.exemplo.com"
      (blur)="save()"
    />
    <button type="button" (click)="save()">Salvar</button>
    @if (message) {
      <p class="message">{{ message }}</p>
    }
  `,
  styles: [`
    label { display: block; margin-top: 0.5rem; }
    input { width: 100%; max-width: 24rem; padding: 0.25rem; margin: 0.25rem 0; }
    button { margin-left: 0.25rem; padding: 0.25rem 0.5rem; }
    .message { margin-top: 0.5rem; color: green; }
  `],
})
export class ConfigComponent {
  apiUrl = '';
  message = '';

  constructor(private readonly apiConfig: ApiConfigService) {
    this.apiUrl = this.apiConfig.getApiUrl();
  }

  save(): void {
    this.apiConfig.setApiUrl(this.apiUrl);
    this.message = 'URL salva.';
    setTimeout(() => (this.message = ''), 3000);
  }
}
