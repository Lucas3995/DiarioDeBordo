import { Routes } from '@angular/router';

export const OBRAS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./obra-lista.component').then((m) => m.ObraListaComponent),
  },
  {
    path: 'atualizar-posicao',
    loadComponent: () =>
      import('./atualizar-posicao/atualizar-posicao.component').then((m) => m.AtualizarPosicaoComponent),
  },
];
