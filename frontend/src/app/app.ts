import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink],
  template: `
    <nav style="display: flex; justify-content: space-between; align-items: center; padding: 1rem 2rem; background: #13131c; border-bottom: 1px solid rgba(255,255,255,0.08);">
      <div style="display: flex; align-items: center; gap: 1.5rem;">
        <a routerLink="/" style="display: flex; align-items: center; gap: 0.5rem; text-decoration: none; color: white; font-weight: 800; font-size: 1.25rem;">
          <span>🚗</span> <span>UniRide</span>
        </a>
        <div *ngIf="auth.isLoggedIn()" style="display: flex; gap: 1rem;">
          <a routerLink="/feed" style="color: #cbd5e1; text-decoration: none; font-size: 0.95rem;">Caronas</a>
          <a routerLink="/painel" style="color: #cbd5e1; text-decoration: none; font-size: 0.95rem;">Meu Painel</a>
        </div>
      </div>

      <div style="display: flex; align-items: center; gap: 1rem;">
        <ng-container *ngIf="auth.isLoggedIn(); else deslogado">
          <span style="font-size: 0.85rem; color: #94a3b8;">
            Olá, <strong>{{ auth.getCurrentUser()?.nome }}</strong>
          </span>
          <button (click)="auth.logout()" style="background: transparent; border: 1px solid rgba(255,255,255,0.2); color: #cbd5e1; padding: 0.35rem 0.8rem; border-radius: 6px; cursor: pointer; font-size: 0.85rem;">
            Sair
          </button>
        </ng-container>
        <ng-template #deslogado>
          <a routerLink="/auth/login" style="color: #cbd5e1; text-decoration: none; font-size: 0.9rem;">Entrar</a>
          <a routerLink="/auth/register" style="background: #4f46e5; color: white; padding: 0.4rem 0.9rem; border-radius: 6px; text-decoration: none; font-size: 0.9rem; font-weight: 600;">Cadastre-se</a>
        </ng-template>
      </div>
    </nav>
    <main>
      <router-outlet />
    </main>
  `,
})
export class App {
  public auth = inject(AuthService);
}

