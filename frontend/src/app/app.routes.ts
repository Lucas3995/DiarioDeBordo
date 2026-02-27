import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', loadChildren: () => import('./features/home/home.routes').then((m) => m.HOME_ROUTES) },
  { path: 'config', loadChildren: () => import('./features/config/config.routes').then((m) => m.CONFIG_ROUTES) },
  { path: 'obras', loadChildren: () => import('./features/obras/obras.routes').then((m) => m.OBRAS_ROUTES) },
  { path: '**', redirectTo: '' },
];
