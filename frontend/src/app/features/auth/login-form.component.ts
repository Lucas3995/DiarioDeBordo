import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../application/auth.service';

/**
 * Formulário de login standalone.
 * Exibe campos de login e senha, botão de envio,
 * e mensagem de sucesso ou erro após a tentativa.
 *
 * Integrado na tela de Configurações para que o usuário
 * possa se autenticar e obter o token automaticamente.
 */
@Component({
  selector: 'app-login-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login-form.component.html',
  styleUrl: './login-form.component.scss',
})
export class LoginFormComponent {
  login = '';
  senha = '';
  carregando = false;
  sucesso = false;
  erro: string | null = null;

  constructor(private readonly authService: AuthService) {}

  submeter(): void {
    if (!this.login.trim() || !this.senha.trim()) {
      return;
    }

    this.carregando = true;
    this.sucesso = false;
    this.erro = null;

    this.authService.login({ login: this.login, senha: this.senha }).subscribe({
      next: (result) => {
        this.carregando = false;
        if (result.sucesso) {
          this.sucesso = true;
        } else if (result.requer2FA) {
          this.erro = 'Segundo fator de autenticação necessário (2FA não implementado ainda).';
        } else {
          this.erro = result.erro ?? 'Falha na autenticação.';
        }
      },
      error: (err) => {
        this.carregando = false;
        this.erro = err?.message ?? 'Erro ao conectar com o servidor.';
      },
    });
  }
}
