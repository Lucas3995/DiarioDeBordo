import { formatarPosicao, ROTULO_POSICAO, TipoObra } from './obra.types';

describe('formatarPosicao', () => {
  it('deve formatar posição de manga como "capítulo N"', () => {
    expect(formatarPosicao(47, TipoObra.Manga)).toBe('capítulo 47');
  });

  it('deve formatar posição de manhwa como "capítulo N"', () => {
    expect(formatarPosicao(120, TipoObra.Manhwa)).toBe('capítulo 120');
  });

  it('deve formatar posição de anime como "episódio N"', () => {
    expect(formatarPosicao(12, TipoObra.Anime)).toBe('episódio 12');
  });

  it('deve formatar posição de série como "episódio N"', () => {
    expect(formatarPosicao(5, TipoObra.Serie)).toBe('episódio 5');
  });

  it('deve formatar posição de livro como "capítulo N"', () => {
    expect(formatarPosicao(200, TipoObra.Livro)).toBe('capítulo 200');
  });

  it('deve formatar posição de filme como "minuto N"', () => {
    expect(formatarPosicao(90, TipoObra.Filme)).toBe('minuto 90');
  });

  it('deve formatar posição de webnovel como "capítulo N"', () => {
    expect(formatarPosicao(1, TipoObra.Webnovel)).toBe('capítulo 1');
  });

  it('deve formatar posição 0 corretamente', () => {
    expect(formatarPosicao(0, TipoObra.Manga)).toBe('capítulo 0');
  });
});

describe('ROTULO_POSICAO', () => {
  it('deve conter rótulo para todos os tipos de obra', () => {
    Object.values(TipoObra).forEach((tipo) => {
      expect(ROTULO_POSICAO[tipo]).toBeDefined();
      expect(ROTULO_POSICAO[tipo].length).toBeGreaterThan(0);
    });
  });
});
