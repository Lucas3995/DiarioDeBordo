import { Component, ChangeDetectionStrategy, signal } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl } from '@angular/forms';
import { AuthService } from '../../application/auth.service';

@Component({
  selector: 'app-login-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginFormComponent {
  readonly form = new FormGroup({
    login: new FormControl('', { nonNullable: true }),
    senha: new FormControl('', { nonNullable: true }),
  });
  readonly carregando = signal(false);
  readonly sucesso = signal(false);
  readonly erro = signal<string | null>(null);

  constructor(private readonly authService: AuthService) {}

  submeter(): void {
    const login = this.form.controls.login.value.trim();
    const senha = this.form.controls.senha.value.trim();
    if (!login || !senha) return;

    this.carregando.set(true);
    this.sucesso.set(false);
    this.erro.set(null);

    this.authService.login({ login, senha }).subscribe({
      next: (result) => {
        this.carregando.set(false);
        if (result.sucesso) {
          this.sucesso.set(true);
        } else if (result.requer2FA) {
          this.erro.set('Segundo fator de autenticação necessário (2FA não implementado ainda).');
        } else {
          this.erro.set(result.erro ?? 'Falha na autenticação.');
        }
      },
      error: (err) => {
        this.carregando.set(false);
        this.erro.set(err?.message ?? 'Erro ao conectar com o servidor.');
      },
    });
  }
}
