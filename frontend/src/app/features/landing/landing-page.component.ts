import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="landing-page" style="max-width: 1100px; margin: 0 auto; padding: 2rem 1rem; color: #fff;">
      <!-- Hero Section -->
      <header style="text-align: center; padding: 3rem 1rem; background: linear-gradient(135deg, rgba(79, 70, 229, 0.15), rgba(16, 185, 129, 0.15)); border-radius: 16px; margin-bottom: 2.5rem; border: 1px solid rgba(255,255,255,0.1);">
        <span style="font-size: 3rem;">🚗💨</span>
        <h1 style="font-size: 2.5rem; margin: 0.5rem 0; font-weight: 800; background: linear-gradient(90deg, #60a5fa, #34d399); -webkit-background-clip: text; -webkit-text-fill-color: transparent;">UniRide</h1>
        <p style="font-size: 1.2rem; color: #cbd5e1; max-width: 650px; margin: 0 auto 1.5rem;">
          Caronas universitárias diárias entre cidades da região. Economize até 60% em transporte, viaje com conforto e embarque com a segurança da comunidade acadêmica.
        </p>
        <div style="display: flex; gap: 1rem; justify-content: center; flex-wrap: wrap;">
          <a routerLink="/auth/register" class="btn btn-primary" style="padding: 0.8rem 1.8rem; font-size: 1.1rem; border-radius: 8px; text-decoration: none; background: #4f46e5; color: white;">Começar Agora</a>
          <a routerLink="/auth/login" class="btn" style="padding: 0.8rem 1.8rem; font-size: 1.1rem; border-radius: 8px; text-decoration: none; border: 1px solid rgba(255,255,255,0.2); color: white;">Fazer Login</a>
        </div>
      </header>

      <!-- Busca Rápida de Caronas -->
      <section style="background: #181824; padding: 1.5rem; border-radius: 12px; margin-bottom: 3rem; border: 1px solid rgba(255,255,255,0.08);">
        <h2 style="font-size: 1.3rem; margin-bottom: 1rem; display: flex; align-items: center; gap: 0.5rem;">
          <span>🔍</span> Busca Rápida de Rotas no Campus
        </h2>
        <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)) auto; gap: 1rem; align-items: end;">
          <div>
            <label style="font-size: 0.85rem; color: #94a3b8; display: block; margin-bottom: 0.25rem;">Cidade Origem</label>
            <input type="text" [(ngModel)]="buscaOrigem" placeholder="Ex: Marília" class="form-control" style="width: 100%; padding: 0.65rem; border-radius: 6px; border: 1px solid rgba(255,255,255,0.15); background: #222233; color: white;">
          </div>
          <div>
            <label style="font-size: 0.85rem; color: #94a3b8; display: block; margin-bottom: 0.25rem;">Cidade Destino (Campus)</label>
            <input type="text" [(ngModel)]="buscaDestino" placeholder="Ex: Fatec Garça" class="form-control" style="width: 100%; padding: 0.65rem; border-radius: 6px; border: 1px solid rgba(255,255,255,0.15); background: #222233; color: white;">
          </div>
          <div>
            <button (click)="buscar()" class="btn btn-primary" style="padding: 0.7rem 1.5rem; border-radius: 6px; background: #10b981; border: none; color: white; cursor: pointer; font-weight: 600;">
              Ver Viagens Disponíveis
            </button>
          </div>
        </div>
      </section>

      <!-- Tabela Comparativa de Planos (SaaS Stripe) -->
      <section style="margin-bottom: 3rem;">
        <h2 style="text-align: center; font-size: 1.8rem; margin-bottom: 0.5rem;">Planos Feitos para Estudantes</h2>
        <p style="text-align: center; color: #94a3b8; margin-bottom: 2rem;">Escolha a opção ideal para a sua rotina de estudos</p>
        
        <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(280px, 1fr)); gap: 1.5rem;">
          <!-- Plano Gratuito -->
          <div style="background: #181824; border: 1px solid rgba(255,255,255,0.1); border-radius: 12px; padding: 2rem; display: flex; flex-direction: column;">
            <h3 style="font-size: 1.3rem; margin-top: 0;">Passageiro Free</h3>
            <div style="font-size: 2rem; font-weight: 700; margin: 1rem 0;">R$ 0<small style="font-size: 0.9rem; color: #94a3b8; font-weight: 400;">/mês</small></div>
            <ul style="list-style: none; padding: 0; margin: 0 0 2rem 0; color: #cbd5e1; line-height: 2;">
              <li>✔ Busca e visualização de todas as caronas</li>
              <li>✔ Até <strong>2 reservas ativas simultâneas</strong></li>
              <li>✔ Rateio de combustível direto com motorista</li>
              <li>✔ Histórico básico de viagens</li>
            </ul>
            <a routerLink="/auth/register" style="margin-top: auto; text-align: center; padding: 0.75rem; border-radius: 6px; border: 1px solid rgba(255,255,255,0.2); color: white; text-decoration: none;">Cadastrar Grátis</a>
          </div>

          <!-- Plano UniRide Pro -->
          <div style="background: #1a1e36; border: 2px solid #6366f1; border-radius: 12px; padding: 2rem; display: flex; flex-direction: column; position: relative;">
            <span style="position: absolute; top: -12px; right: 20px; background: #6366f1; color: white; padding: 2px 10px; border-radius: 12px; font-size: 0.75rem; font-weight: 700; text-transform: uppercase;">Mais Popular</span>
            <h3 style="font-size: 1.3rem; margin-top: 0; color: #818cf8;">UniRide Pro</h3>
            <div style="font-size: 2rem; font-weight: 700; margin: 1rem 0;">R$ 14,90<small style="font-size: 0.9rem; color: #94a3b8; font-weight: 400;">/mês via Stripe</small></div>
            <ul style="list-style: none; padding: 0; margin: 0 0 2rem 0; color: #cbd5e1; line-height: 2;">
              <li>⭐ <strong>Reservas ilimitadas</strong> de caronas no mês</li>
              <li>⭐ Notificações prioritárias de novas rotas</li>
              <li>⭐ Condutores: publicação ilimitada de viagens</li>
              <li>⭐ Selo de <strong>Motorista Verificado</strong> no feed</li>
              <li>⭐ Cancelamento fácil via Stripe Customer Portal</li>
            </ul>
            <a routerLink="/auth/register" style="margin-top: auto; text-align: center; padding: 0.75rem; border-radius: 6px; background: #6366f1; color: white; text-decoration: none; font-weight: 600;">Assinar Pro</a>
          </div>
        </div>
      </section>
    </div>
  `,
})
export class LandingPageComponent {
  buscaOrigem = '';
  buscaDestino = '';

  constructor(private router: Router) {}

  buscar() {
    this.router.navigate(['/feed'], {
      queryParams: {
        origem: this.buscaOrigem || undefined,
        destino: this.buscaDestino || undefined
      }
    });
  }
}
