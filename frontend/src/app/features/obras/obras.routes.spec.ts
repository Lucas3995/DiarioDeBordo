import { OBRAS_ROUTES } from './obras.routes';

describe('OBRAS_ROUTES', () => {
  it('não deve conter rota para página de atualizar-posicao (demanda 1 — atualização em popup)', () => {
    const atualizarPosicaoRoute = OBRAS_ROUTES.find((r) => r.path === 'atualizar-posicao');
    expect(atualizarPosicaoRoute).toBeUndefined();
  });

  it('deve conter apenas a rota vazia que carrega a lista de obras', () => {
    expect(OBRAS_ROUTES.length).toBe(1);
    expect(OBRAS_ROUTES[0].path).toBe('');
  });
});
