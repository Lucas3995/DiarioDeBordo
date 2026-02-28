import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { ApiConfigService } from '../core/api-config.service';
import { IAuthPort } from '../domain/auth.port';
import { LoginResult } from '../domain/auth.types';
import { AuthHttp } from './auth.http';

describe('AuthHttp', () => {
  let authHttp: AuthHttp;
  let httpMock: HttpTestingController;
  let apiConfigSpy: jasmine.SpyObj<ApiConfigService>;

  const apiUrl = 'http://localhost:5000';

  beforeEach(() => {
    apiConfigSpy = jasmine.createSpyObj<ApiConfigService>('ApiConfigService', ['getApiUrl', 'setToken']);
    apiConfigSpy.getApiUrl.and.returnValue(apiUrl);

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        AuthHttp,
        { provide: ApiConfigService, useValue: apiConfigSpy },
        { provide: IAuthPort, useClass: AuthHttp },
      ],
    });

    authHttp = TestBed.inject(AuthHttp);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  // ────────────────── URL e método da requisição ──────────────────

  it('deve chamar POST na URL correta', () => {
    authHttp.login({ login: 'admin', senha: 'camaradinha@123' }).subscribe();

    const req = httpMock.expectOne(`${apiUrl}/api/auth/login`);
    expect(req.request.method).toBe('POST');
    req.flush({ sucesso: true, token: 'tok', expiresAt: null, requer2FA: false, erro: null });
  });

  // ────────────────── Body da requisição ──────────────────

  it('deve enviar login e senha no body', () => {
    authHttp.login({ login: 'admin', senha: 'camaradinha@123' }).subscribe();

    const req = httpMock.expectOne(`${apiUrl}/api/auth/login`);
    expect(req.request.body).toEqual({ login: 'admin', senha: 'camaradinha@123' });
    req.flush({ sucesso: true, token: 'tok', expiresAt: null, requer2FA: false, erro: null });
  });

  // ────────────────── Sucesso: setToken e mapeamento ──────────────────

  it('em sucesso deve chamar setToken com o token recebido', () => {
    const mockResult: LoginResult = {
      sucesso: true,
      token: 'jwt.token.valido',
      expiresAt: '2030-01-01T00:00:00Z',
      requer2FA: false,
      erro: null,
    };

    authHttp.login({ login: 'admin', senha: 'camaradinha@123' }).subscribe();

    const req = httpMock.expectOne(`${apiUrl}/api/auth/login`);
    req.flush(mockResult);

    expect(apiConfigSpy.setToken).toHaveBeenCalledWith('jwt.token.valido');
  });

  it('em sucesso deve retornar LoginResult com token, expiresAt e requer2FA mapeados', (done) => {
    const mockResult: LoginResult = {
      sucesso: true,
      token: 'jwt.token.valido',
      expiresAt: '2030-01-01T00:00:00Z',
      requer2FA: false,
      erro: null,
    };

    authHttp.login({ login: 'admin', senha: 'camaradinha@123' }).subscribe((result) => {
      expect(result.token).toBe('jwt.token.valido');
      expect(result.expiresAt).toBe('2030-01-01T00:00:00Z');
      expect(result.requer2FA).toBeFalse();
      done();
    });

    httpMock.expectOne(`${apiUrl}/api/auth/login`).flush(mockResult);
  });

  // ────────────────── apiUrl não configurado ──────────────────

  it('quando apiUrl não configurado deve retornar erro sem fazer request HTTP', (done) => {
    apiConfigSpy.getApiUrl.and.returnValue('');

    authHttp.login({ login: 'admin', senha: 'camaradinha@123' }).subscribe({
      error: (err) => {
        expect(err.message).toContain('URL da API');
        done();
      },
    });

    httpMock.expectNone(`${apiUrl}/api/auth/login`);
  });

  // ────────────────── Erro HTTP 401 ──────────────────

  it('erro HTTP 401 deve ser propagado como Observable error', (done) => {
    authHttp.login({ login: 'admin', senha: 'senhaErrada' }).subscribe({
      error: (err) => {
        expect(err.status).toBe(401);
        done();
      },
    });

    const req = httpMock.expectOne(`${apiUrl}/api/auth/login`);
    req.flush({ erro: 'Credenciais inválidas.' }, { status: 401, statusText: 'Unauthorized' });
  });
});
