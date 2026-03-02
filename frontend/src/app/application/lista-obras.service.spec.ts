import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { IListaObrasPort, ListaObrasParams, ListaObrasResult, ObraBuscaItem } from '../domain/lista-obras.port';
import { TipoObra } from '../domain/obra.types';
import { ObraListItem } from '../domain/obra-list-item';
import { ListaObrasService } from './lista-obras.service';

const OBRAS_MOCK: ObraListItem[] = [
  {
    id: '1',
    nome: 'One Piece',
    tipo: TipoObra.Manga,
    ultimaAtualizacaoPosicao: new Date('2026-02-20'),
    posicaoAtual: 1110,
    ordemPreferencia: 1,
    proximaInfo: { tipo: 'dias_ate_proxima', dias: 5 },
  },
  {
    id: '2',
    nome: 'Attack on Titan',
    tipo: TipoObra.Manga,
    ultimaAtualizacaoPosicao: new Date('2026-02-10'),
    posicaoAtual: 139,
    ordemPreferencia: 2,
  },
];

const RESULTADO_MOCK: ListaObrasResult = {
  items: OBRAS_MOCK,
  totalCount: 10,
};

describe('ListaObrasService', () => {
  let service: ListaObrasService;
  let portaSpy: jasmine.SpyObj<IListaObrasPort>;

  beforeEach(() => {
    portaSpy = jasmine.createSpyObj<IListaObrasPort>('IListaObrasPort', ['listarPagina', 'buscarPorNome']);
    portaSpy.listarPagina.and.returnValue(of(RESULTADO_MOCK));
    portaSpy.buscarPorNome.and.returnValue(of([{ id: '1', nome: 'One Piece' }]));

    TestBed.configureTestingModule({
      providers: [
        ListaObrasService,
        { provide: IListaObrasPort, useValue: portaSpy },
      ],
    });

    service = TestBed.inject(ListaObrasService);
  });

  it('deve ser criado', () => {
    expect(service).toBeTruthy();
  });

  describe('listarPagina()', () => {
    it('deve delegar à porta com os parâmetros recebidos', () => {
      const params: ListaObrasParams = { pageIndex: 0, pageSize: 10 };
      service.listarPagina(params).subscribe();
      expect(portaSpy.listarPagina).toHaveBeenCalledWith(params);
      expect(portaSpy.listarPagina).toHaveBeenCalledTimes(1);
    });

    it('deve retornar o resultado da porta intacto', (done) => {
      const params: ListaObrasParams = { pageIndex: 0, pageSize: 10 };
      service.listarPagina(params).subscribe((result) => {
        expect(result).toEqual(RESULTADO_MOCK);
        expect(result.items.length).toBe(2);
        expect(result.totalCount).toBe(10);
        done();
      });
    });

    it('deve retornar os items na ordem devolvida pela porta', (done) => {
      const params: ListaObrasParams = { pageIndex: 0, pageSize: 10 };
      service.listarPagina(params).subscribe((result) => {
        expect(result.items[0].nome).toBe('One Piece');
        expect(result.items[1].nome).toBe('Attack on Titan');
        done();
      });
    });

    it('deve passar pageIndex e pageSize diferentes corretamente', () => {
      const params: ListaObrasParams = { pageIndex: 2, pageSize: 25 };
      service.listarPagina(params).subscribe();
      expect(portaSpy.listarPagina).toHaveBeenCalledWith(params);
    });

    it('deve propagar resultado com lista vazia', (done) => {
      portaSpy.listarPagina.and.returnValue(of({ items: [], totalCount: 0 }));
      service.listarPagina({ pageIndex: 0, pageSize: 10 }).subscribe((result) => {
        expect(result.items).toEqual([]);
        expect(result.totalCount).toBe(0);
        done();
      });
    });
  });

  describe('buscarPorNome() — Demanda 4 (autocomplete)', () => {
    it('deve delegar à porta com termo e limit e retornar os itens', (done) => {
      const itensEsperados: ObraBuscaItem[] = [{ id: '1', nome: 'One Piece' }];
      service.buscarPorNome('One', 10).subscribe((items) => {
        expect(portaSpy.buscarPorNome).toHaveBeenCalledWith('One', 10);
        expect(items).toEqual(itensEsperados);
        done();
      });
    });
  });
});
