import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { ApiConfigService } from '../core/api-config.service';
import { IAuthPort } from '../domain/auth.port';
import { LoginResult } from '../domain/auth.types';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  let service: AuthService;
  let authPortSpy: jasmine.SpyObj<IAuthPort>;
  let apiConfigSpy: jasmine.SpyObj<ApiConfigService>;

  beforeEach(() => {
    authPortSpy = jasmine.createSpyObj<IAuthPort>('IAuthPort', ['login']);
    apiConfigSpy = jasmine.createSpyObj<ApiConfigService>('ApiConfigService', ['setToken', 'getApiUrl']);

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: IAuthPort, useValue: authPortSpy },
        { provide: ApiConfigService, useValue: apiConfigSpy },
      ],
    });

    service = TestBed.inject(AuthService);
  });

  // ────────────────── Sucesso ──────────────────

  it('em sucesso deve chamar setToken com o token', (done) => {
    const mockResult: LoginResult = {
      sucesso: true,
      token: 'jwt.token.valido',
      expiresAt: '2030-01-01T00:00:00Z',
      requer2FA: false,
      erro: null,
    };
    authPortSpy.login.and.returnValue(of(mockResult));

    service.login({ login: 'admin', senha: 'camaradinha@123' }).subscribe((result) => {
      expect(result.sucesso).toBeTrue();
      expect(apiConfigSpy.setToken).toHaveBeenCalledWith('jwt.token.valido');
      done();
    });
  });

  it('em sucesso com requer2FA=true não deve chamar setToken (sem token emitido)', (done) => {
    const mockResult: LoginResult = {
      sucesso: false,
      token: null,
      expiresAt: null,
      requer2FA: true,
      erro: null,
    };
    authPortSpy.login.and.returnValue(of(mockResult));

    service.login({ login: 'admin', senha: 'camaradinha@123' }).subscribe(() => {
      expect(apiConfigSpy.setToken).not.toHaveBeenCalled();
      done();
    });
  });

  // ────────────────── Falha ──────────────────

  it('em falha deve propagar o erro sem engolir', (done) => {
    const erro = new Error('Credenciais inválidas.');
    authPortSpy.login.and.returnValue(throwError(() => erro));

    service.login({ login: 'admin', senha: 'errada' }).subscribe({
      error: (err) => {
        expect(err.message).toBe('Credenciais inválidas.');
        expect(apiConfigSpy.setToken).not.toHaveBeenCalled();
        done();
      },
    });
  });
});
