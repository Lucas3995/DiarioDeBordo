import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { AuthService } from '../../application/auth.service';
import { LoginResult } from '../../domain/auth.types';
import { LoginFormComponent } from './login-form.component';

describe('LoginFormComponent', () => {
  let component: LoginFormComponent;
  let fixture: ComponentFixture<LoginFormComponent>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    authServiceSpy = jasmine.createSpyObj<AuthService>('AuthService', ['login']);

    await TestBed.configureTestingModule({
      imports: [LoginFormComponent],
      providers: [{ provide: AuthService, useValue: authServiceSpy }],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  // ────────────────── Criação ──────────────────

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  // ────────────────── Campos vazios ──────────────────

  it('submeter com login vazio não deve chamar authService.login', () => {
    component.login = '';
    component.senha = 'camaradinha@123';
    component.submeter();

    expect(authServiceSpy.login).not.toHaveBeenCalled();
  });

  it('submeter com senha vazia não deve chamar authService.login', () => {
    component.login = 'admin';
    component.senha = '';
    component.submeter();

    expect(authServiceSpy.login).not.toHaveBeenCalled();
  });

  it('submeter com campos só de espaços não deve chamar authService.login', () => {
    component.login = '   ';
    component.senha = '   ';
    component.submeter();

    expect(authServiceSpy.login).not.toHaveBeenCalled();
  });

  // ────────────────── Credenciais válidas ──────────────────

  it('submeter com credenciais válidas deve chamar authService.login com os valores corretos', () => {
    const mockResult: LoginResult = { sucesso: true, token: 'tok', expiresAt: null, requer2FA: false, erro: null };
    authServiceSpy.login.and.returnValue(of(mockResult));

    component.login = 'admin';
    component.senha = 'camaradinha@123';
    component.submeter();

    expect(authServiceSpy.login).toHaveBeenCalledWith({ login: 'admin', senha: 'camaradinha@123' });
  });

  // ────────────────── Sucesso → DOM ──────────────────

  it('em sucesso deve exibir mensagem de confirmação no DOM', () => {
    const mockResult: LoginResult = { sucesso: true, token: 'tok', expiresAt: null, requer2FA: false, erro: null };
    authServiceSpy.login.and.returnValue(of(mockResult));

    component.login = 'admin';
    component.senha = 'camaradinha@123';
    component.submeter();
    fixture.detectChanges();

    const msgEl: HTMLElement = fixture.nativeElement.querySelector('.login-form__msg--sucesso');
    expect(msgEl).not.toBeNull();
    expect(msgEl.textContent).toContain('sucesso');
  });

  // ────────────────── Erro → DOM ──────────────────

  it('em erro deve exibir mensagem de erro no DOM', () => {
    authServiceSpy.login.and.returnValue(throwError(() => new Error('Credenciais inválidas.')));

    component.login = 'admin';
    component.senha = 'errada';
    component.submeter();
    fixture.detectChanges();

    const msgEl: HTMLElement = fixture.nativeElement.querySelector('.login-form__msg--erro');
    expect(msgEl).not.toBeNull();
    expect(msgEl.textContent).toContain('Credenciais inválidas');
  });

  it('em resposta com sucesso=false e requer2FA=false deve exibir mensagem de erro no DOM', () => {
    const mockResult: LoginResult = {
      sucesso: false,
      token: null,
      expiresAt: null,
      requer2FA: false,
      erro: 'Credenciais inválidas.',
    };
    authServiceSpy.login.and.returnValue(of(mockResult));

    component.login = 'admin';
    component.senha = 'errada';
    component.submeter();
    fixture.detectChanges();

    const msgEl: HTMLElement = fixture.nativeElement.querySelector('.login-form__msg--erro');
    expect(msgEl).not.toBeNull();
    expect(msgEl.textContent).toContain('Credenciais inválidas');
  });
});
