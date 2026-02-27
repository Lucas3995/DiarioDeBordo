import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApiConfigService } from '../../core/api-config.service';

@Component({
  selector: 'app-config',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './config.component.html',
  styleUrl: './config.component.scss',
})
export class ConfigComponent {
  apiUrl = '';
  jwtToken = '';
  message = '';

  constructor(private readonly apiConfig: ApiConfigService) {
    this.apiUrl = this.apiConfig.getApiUrl();
    this.jwtToken = this.apiConfig.getToken();
  }

  save(): void {
    this.apiConfig.setApiUrl(this.apiUrl);
    this.apiConfig.setToken(this.jwtToken);
    this.message = 'Configurações salvas.';
    setTimeout(() => (this.message = ''), 3000);
  }
}
