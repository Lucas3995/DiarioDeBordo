import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  template: `
    <nav>
      <a routerLink="/">Início</a>
      <a routerLink="/obras">Obras</a>
      <a routerLink="/config">Configurações</a>
    </nav>
    <main>
      <router-outlet></router-outlet>
    </main>
  `,
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'frontend';
}
