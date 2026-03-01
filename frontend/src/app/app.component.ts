import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';
import { DialogHostComponent } from './shared/dialog/dialog-host.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, DialogHostComponent],
  template: `
    <nav>
      <a routerLink="/">Início</a>
      <a routerLink="/obras">Obras</a>
      <a routerLink="/config">Configurações</a>
    </nav>
    <main>
      <router-outlet></router-outlet>
    </main>
    <app-dialog-host />
  `,
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'frontend';
}
