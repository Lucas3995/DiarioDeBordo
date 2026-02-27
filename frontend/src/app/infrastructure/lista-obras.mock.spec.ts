import { ListaObrasMock } from './lista-obras.mock';
import { IListaObrasPort } from '../domain/lista-obras.port';
import { TipoObra } from '../domain/obra.types';

describe('ListaObrasMock', () => {
  let mock: ListaObrasMock;

  beforeEach(() => {
    mock = new ListaObrasMock();
  });

  it('deve ser criado', () => {
    expect(mock).toBeTruthy();
  });

  it('deve implementar IListaObrasPort', () => {
    expect(mock instanceof IListaObrasPort).toBeTrue();
  });

  describe('listarPagina()', () => {
    it('deve retornar items e totalCount', (done) => {
      mock.listarPagina({ pageIndex: 0, pageSize: 10 }).subscribe((result) => {
        expect(result.items).toBeDefined();
        expect(result.totalCount).toBeGreaterThan(0);
        done();
      });
    });

    it('deve respeitar o pageSize solicitado (até o total disponível)', (done) => {
      mock.listarPagina({ pageIndex: 0, pageSize: 3 }).subscribe((result) => {
        expect(result.items.length).toBeLessThanOrEqual(3);
        done();
      });
    });

    it('deve retornar página correta conforme pageIndex', (done) => {
      mock.listarPagina({ pageIndex: 0, pageSize: 2 }).subscribe((primeiraPage) => {
        mock.listarPagina({ pageIndex: 1, pageSize: 2 }).subscribe((segundaPage) => {
          expect(primeiraPage.items[0].id).not.toBe(segundaPage.items[0].id);
          done();
        });
      });
    });

    it('deve retornar lista vazia quando pageIndex excede o total', (done) => {
      mock.listarPagina({ pageIndex: 9999, pageSize: 10 }).subscribe((result) => {
        expect(result.items.length).toBe(0);
        expect(result.totalCount).toBeGreaterThan(0);
        done();
      });
    });

    it('deve retornar items com todos os campos obrigatórios preenchidos', (done) => {
      mock.listarPagina({ pageIndex: 0, pageSize: 10 }).subscribe((result) => {
        result.items.forEach((item) => {
          expect(item.id).toBeDefined();
          expect(item.nome).toBeTruthy();
          expect(item.tipo).toBeDefined();
          expect(Object.values(TipoObra)).toContain(item.tipo);
          expect(item.ultimaAtualizacaoPosicao).toBeDefined();
          expect(item.ultimaAtualizacaoPosicao instanceof Date).toBeTrue();
          expect(typeof item.posicaoAtual).toBe('number');
          expect(typeof item.ordemPreferencia).toBe('number');
        });
        done();
      });
    });

    it('deve retornar items ordenados por ordemPreferencia', (done) => {
      mock.listarPagina({ pageIndex: 0, pageSize: 100 }).subscribe((result) => {
        const ordens = result.items.map((i) => i.ordemPreferencia);
        for (let i = 1; i < ordens.length; i++) {
          expect(ordens[i]).toBeGreaterThanOrEqual(ordens[i - 1]);
        }
        done();
      });
    });

    it('deve incluir proximaInfo quando disponível', (done) => {
      mock.listarPagina({ pageIndex: 0, pageSize: 100 }).subscribe((result) => {
        const comProximaInfo = result.items.filter((i) => i.proximaInfo);
        expect(comProximaInfo.length).toBeGreaterThan(0);

        comProximaInfo.forEach((item) => {
          const info = item.proximaInfo!;
          if (info.tipo === 'dias_ate_proxima') {
            expect(typeof info.dias).toBe('number');
            expect(info.dias).toBeGreaterThanOrEqual(0);
          } else {
            expect(typeof info.quantidade).toBe('number');
            expect(info.quantidade).toBeGreaterThan(0);
          }
        });
        done();
      });
    });

    it('deve conter totalCount igual ao total da lista interna', (done) => {
      mock.listarPagina({ pageIndex: 0, pageSize: 100 }).subscribe((paginaCompleta) => {
        const total = paginaCompleta.totalCount;
        mock.listarPagina({ pageIndex: 0, pageSize: 3 }).subscribe((paginaPequena) => {
          expect(paginaPequena.totalCount).toBe(total);
          done();
        });
      });
    });

    it('deve suportar pageSize 10, 25, 50 e 100', (done) => {
      const pageSizes = [10, 25, 50, 100];
      let completed = 0;

      pageSizes.forEach((pageSize) => {
        mock.listarPagina({ pageIndex: 0, pageSize }).subscribe((result) => {
          expect(result.items.length).toBeLessThanOrEqual(pageSize);
          completed++;
          if (completed === pageSizes.length) done();
        });
      });
    });
  });
});
