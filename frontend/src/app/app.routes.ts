import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', loadChildren: () => import('./features/home/home.routes').then((m) => m.HOME_ROUTES) },
  { path: 'config', loadChildren: () => import('./features/config/config.routes').then((m) => m.CONFIG_ROUTES) },
  { path: '**', redirectTo: '' },
];
