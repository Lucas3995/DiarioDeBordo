import { Component, ChangeDetectionStrategy, signal } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl } from '@angular/forms';
import { ApiConfigService } from '../../core/api-config.service';
import { LoginFormComponent } from '../auth/login-form.component';

@Component({
  selector: 'app-config',
  standalone: true,
  imports: [ReactiveFormsModule, LoginFormComponent],
  templateUrl: './config.component.html',
  styleUrl: './config.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfigComponent {
  readonly form = new FormGroup({
    apiUrl: new FormControl('', { nonNullable: true }),
    jwtToken: new FormControl('', { nonNullable: true }),
  });
  readonly message = signal('');

  constructor(private readonly apiConfig: ApiConfigService) {
    this.form.patchValue({
      apiUrl: this.apiConfig.getApiUrl(),
      jwtToken: this.apiConfig.getToken(),
    });
  }

  save(): void {
    this.apiConfig.setApiUrl(this.form.controls.apiUrl.value);
    this.apiConfig.setToken(this.form.controls.jwtToken.value);
    this.message.set('Configurações salvas.');
    setTimeout(() => this.message.set(''), 3000);
  }
}
