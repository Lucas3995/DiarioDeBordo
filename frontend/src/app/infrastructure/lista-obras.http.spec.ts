import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ListaObrasHttp } from './lista-obras.http';
import { ApiConfigService } from '../core/api-config.service';

describe('ListaObrasHttp', () => {
  let service: ListaObrasHttp;
  let httpTesting: HttpTestingController;
  let apiConfigSpy: jasmine.SpyObj<ApiConfigService>;

  beforeEach(() => {
    apiConfigSpy = jasmine.createSpyObj<ApiConfigService>('ApiConfigService', ['getApiUrl', 'getToken']);
    apiConfigSpy.getApiUrl.and.returnValue('http://localhost:5000');
    apiConfigSpy.getToken.and.returnValue('token-teste');

    TestBed.configureTestingModule({
      providers: [
        ListaObrasHttp,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ApiConfigService, useValue: apiConfigSpy },
      ],
    });

    service = TestBed.inject(ListaObrasHttp);
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('deve criar o serviço', () => {
    expect(service).toBeTruthy();
  });

  it('deve chamar GET /api/obras com pageIndex e pageSize corretos', () => {
    service.listarPagina({ pageIndex: 0, pageSize: 10 }).subscribe();

    const req = httpTesting.expectOne((r) => r.url === 'http://localhost:5000/api/obras');
    expect(req.request.params.get('pageIndex')).toBe('0');
    expect(req.request.params.get('pageSize')).toBe('10');
    req.flush({ items: [], totalCount: 0 });
  });

  it('deve enviar o cabeçalho Authorization com o token', () => {
    service.listarPagina({ pageIndex: 0, pageSize: 10 }).subscribe();

    const req = httpTesting.expectOne((r) => r.url === 'http://localhost:5000/api/obras');
    expect(req.request.headers.get('Authorization')).toBe('Bearer token-teste');
    req.flush({ items: [], totalCount: 0 });
  });

  it('deve mapear os itens recebidos para ObraListItem', () => {
    let resultado: { items: unknown[]; totalCount: number } | undefined;

    service.listarPagina({ pageIndex: 0, pageSize: 10 }).subscribe((r) => (resultado = r));

    const req = httpTesting.expectOne((r) => r.url === 'http://localhost:5000/api/obras');
    req.flush({
      items: [
        {
          id: 'abc-123',
          nome: 'One Piece',
          tipo: 'manga',
          ultimaAtualizacaoPosicao: '2026-02-20T00:00:00Z',
          posicaoAtual: 1110,
          proximaInfo: { tipo: 'dias_ate_proxima', dias: 5 },
          ordemPreferencia: 1,
        },
      ],
      totalCount: 1,
    });

    expect(resultado).toBeDefined();
    expect(resultado!.totalCount).toBe(1);
    const item = resultado!.items[0] as {
      id: string;
      nome: string;
      tipo: string;
      posicaoAtual: number;
      proximaInfo: { tipo: string; dias: number };
    };
    expect(item.id).toBe('abc-123');
    expect(item.nome).toBe('One Piece');
    expect(item.tipo).toBe('manga');
    expect(item.posicaoAtual).toBe(1110);
    expect(item.proximaInfo).toEqual({ tipo: 'dias_ate_proxima', dias: 5 });
  });

  it('deve mapear proximaInfo de partes_ja_publicadas', () => {
    let resultado: { items: unknown[] } | undefined;

    service.listarPagina({ pageIndex: 0, pageSize: 10 }).subscribe((r) => (resultado = r));

    const req = httpTesting.expectOne((r) => r.url === 'http://localhost:5000/api/obras');
    req.flush({
      items: [
        {
          id: '1',
          nome: 'Vinland Saga',
          tipo: 'manga',
          ultimaAtualizacaoPosicao: '2026-02-05T00:00:00Z',
          posicaoAtual: 198,
          proximaInfo: { tipo: 'partes_ja_publicadas', quantidade: 2 },
          ordemPreferencia: 2,
        },
      ],
      totalCount: 1,
    });

    const item = resultado!.items[0] as { proximaInfo: { tipo: string; quantidade: number } };
    expect(item.proximaInfo).toEqual({ tipo: 'partes_ja_publicadas', quantidade: 2 });
  });

  it('deve retornar erro quando apiUrl não estiver configurada', () => {
    apiConfigSpy.getApiUrl.and.returnValue('');
    let erro: Error | undefined;

    service.listarPagina({ pageIndex: 0, pageSize: 10 }).subscribe({
      error: (e: Error) => (erro = e),
    });

    expect(erro?.message).toContain('URL da API não configurada');
    httpTesting.expectNone(() => true);
  });

  it('deve mapear ultimaAtualizacaoPosicao como Date', () => {
    let resultado: { items: unknown[] } | undefined;

    service.listarPagina({ pageIndex: 0, pageSize: 10 }).subscribe((r) => (resultado = r));

    const req = httpTesting.expectOne((r) => r.url === 'http://localhost:5000/api/obras');
    req.flush({
      items: [
        {
          id: '1',
          nome: 'Teste',
          tipo: 'livro',
          ultimaAtualizacaoPosicao: '2026-01-15T00:00:00Z',
          posicaoAtual: 10,
          ordemPreferencia: 1,
        },
      ],
      totalCount: 1,
    });

    const item = resultado!.items[0] as { ultimaAtualizacaoPosicao: Date };
    expect(item.ultimaAtualizacaoPosicao).toBeInstanceOf(Date);
    expect(item.ultimaAtualizacaoPosicao.getFullYear()).toBe(2026);
  });

  describe('Demanda 4 — buscarPorNome (autocomplete)', () => {
    it('deve existir método buscarPorNome no adaptador', () => {
      expect(typeof (service as unknown as { buscarPorNome?: (termo: string, limit?: number) => unknown }).buscarPorNome).toBe('function');
    });

    it('ao chamar buscarPorNome deve fazer GET /api/obras/buscar com q e limit', () => {
      const svc = service as unknown as { buscarPorNome: (termo: string, limit?: number) => ReturnType<ListaObrasHttp['listarPagina']> };
      if (typeof svc.buscarPorNome !== 'function') {
        fail('ListaObrasHttp deve implementar buscarPorNome(termo, limit)');
        return;
      }
      svc.buscarPorNome('One', 10).subscribe();

      const req = httpTesting.expectOne((r) => r.url.includes('/api/obras/buscar'));
      expect(req.request.params.get('q')).toBe('One');
      expect(req.request.params.get('limit')).toBe('10');
      req.flush([{ id: 'id-1', nome: 'One Piece' }]);
    });

    it('deve mapear resposta de buscarPorNome para array de { id, nome }', (done) => {
      const svc = service as unknown as { buscarPorNome: (termo: string, limit?: number) => Observable<Array<{ id: string; nome: string }>> };
      if (typeof svc.buscarPorNome !== 'function') {
        fail('ListaObrasHttp deve implementar buscarPorNome(termo, limit)');
        done();
        return;
      }
      svc.buscarPorNome('One', 5).subscribe((items) => {
        expect(Array.isArray(items)).toBe(true);
        expect(items.length).toBe(1);
        expect(items[0].id).toBe('id-1');
        expect(items[0].nome).toBe('One Piece');
        done();
      });

      const req = httpTesting.expectOne((r) => r.url.includes('/api/obras/buscar'));
      req.flush([{ id: 'id-1', nome: 'One Piece' }]);
    });
  });
});
