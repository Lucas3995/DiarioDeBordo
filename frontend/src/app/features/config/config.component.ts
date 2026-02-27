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
