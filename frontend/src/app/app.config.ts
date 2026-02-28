import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { routes } from './app.routes';
import { IListaObrasPort } from './domain/lista-obras.port';
import { ListaObrasHttp } from './infrastructure/lista-obras.http';
import { ListaObrasService } from './application/lista-obras.service';
import { IAuthPort } from './domain/auth.port';
import { AuthHttp } from './infrastructure/auth.http';
import { AuthService } from './application/auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    { provide: IListaObrasPort, useClass: ListaObrasHttp },
    ListaObrasService,
    { provide: IAuthPort, useClass: AuthHttp },
    AuthService,
  ],
};
