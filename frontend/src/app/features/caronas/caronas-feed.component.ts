import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { Carona, Reserva } from '../../../shared/models/models';

@Component({
  selector: 'app-caronas-feed',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div style="max-width: 1000px; margin: 0 auto; padding: 2rem 1rem; color: #fff;">
      <div style="display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 1rem; margin-bottom: 2rem;">
        <div>
          <h1 style="font-size: 2rem; margin: 0; font-weight: 800;">🚗 Caronas Disponíveis</h1>
          <p style="margin: 0.25rem 0 0 0; color: #94a3b8;">Encontre caronas diárias de colegas universitários</p>
        </div>
        <div style="display: flex; gap: 0.75rem;">
          <a *ngIf="auth.isMotorista()" routerLink="/painel" class="btn" style="padding: 0.6rem 1.2rem; background: #10b981; color: white; border-radius: 8px; text-decoration: none; font-weight: 600;">
            + Publicar Nova Rota
          </a>
          <button (click)="assinarPro()" class="btn" style="padding: 0.6rem 1.2rem; background: #6366f1; color: white; border-radius: 8px; border: none; cursor: pointer; font-weight: 600;">
            ⭐ Assinar UniRide Pro
          </button>
        </div>
      </div>

      <!-- Alertas de Sucesso / Erro -->
      <div *ngIf="mensagemSucesso()" style="background: rgba(16, 185, 129, 0.2); border: 1px solid #10b981; color: #6ee7b7; padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem;">
        {{ mensagemSucesso() }}
      </div>

      <div *ngIf="mensagemErro()" style="background: rgba(239, 68, 68, 0.2); border: 1px solid #ef4444; color: #fca5a5; padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem; display: flex; justify-content: space-between; align-items: center;">
        <div>{{ mensagemErro() }}</div>
        <button *ngIf="mensagemErro().includes('UniRide Pro')" (click)="assinarPro()" style="background: #6366f1; border: none; color: white; padding: 0.4rem 0.8rem; border-radius: 6px; cursor: pointer; font-weight: 600;">
          Fazer Upgrade
        </button>
      </div>

      <!-- Barra de Filtros -->
      <div style="background: #181824; padding: 1.25rem; border-radius: 12px; margin-bottom: 2rem; border: 1px solid rgba(255,255,255,0.08); display: grid; grid-template-columns: repeat(auto-fit, minmax(180px, 1fr)) auto; gap: 1rem; align-items: end;">
        <div>
          <label style="font-size: 0.8rem; color: #94a3b8; display: block; margin-bottom: 0.25rem;">Origem</label>
          <input type="text" [(ngModel)]="filtroOrigem" placeholder="Cidade de saída" style="width: 100%; padding: 0.6rem; border-radius: 6px; border: 1px solid rgba(255,255,255,0.15); background: #222233; color: white;">
        </div>
        <div>
          <label style="font-size: 0.8rem; color: #94a3b8; display: block; margin-bottom: 0.25rem;">Destino</label>
          <input type="text" [(ngModel)]="filtroDestino" placeholder="Cidade / Campus" style="width: 100%; padding: 0.6rem; border-radius: 6px; border: 1px solid rgba(255,255,255,0.15); background: #222233; color: white;">
        </div>
        <div>
          <button (click)="carregarCaronas()" style="padding: 0.65rem 1.4rem; background: #3b82f6; color: white; border: none; border-radius: 6px; cursor: pointer; font-weight: 600;">
            Filtrar
          </button>
        </div>
      </div>

      <!-- Feed com Cards de Carona -->
      <div *ngIf="loading()" style="text-align: center; padding: 3rem; color: #94a3b8;">
        Carregando caronas disponíveis...
      </div>

      <div *ngIf="!loading() && caronas().length === 0" style="text-align: center; padding: 3rem; background: #181824; border-radius: 12px; border: 1px dashed rgba(255,255,255,0.15);">
        <p style="font-size: 1.1rem; color: #cbd5e1;">Nenhuma carona encontrada para os critérios selecionados.</p>
        <span style="color: #94a3b8;">Tente limpar os filtros ou volte mais tarde!</span>
      </div>

      <div *ngIf="!loading() && caronas().length > 0" style="display: grid; grid-template-columns: repeat(auto-fill, minmax(310px, 1fr)); gap: 1.5rem;">
        <div *ngFor="let c of caronas()" style="background: #181824; border: 1px solid rgba(255,255,255,0.1); border-radius: 12px; padding: 1.5rem; display: flex; flex-direction: column; justify-content: space-between;">
          <div>
            <div style="display: flex; justify-content: space-between; align-items: start; margin-bottom: 1rem;">
              <div>
                <span style="font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.5px; color: #10b981; font-weight: 700;">
                  ● {{ c.vagasDisponiveis }} {{ c.vagasDisponiveis === 1 ? 'vaga restante' : 'vagas restantes' }}
                </span>
                <h3 style="margin: 0.25rem 0 0 0; font-size: 1.25rem;">{{ c.cidadeOrigem }} ➔ {{ c.cidadeDestino }}</h3>
              </div>
              <div style="text-align: right;">
                <span style="font-size: 1.25rem; font-weight: 800; color: #38bdf8;">R$ {{ c.valorRateio.toFixed(2) }}</span>
                <span style="display: block; font-size: 0.75rem; color: #94a3b8;">rateio</span>
              </div>
            </div>

            <div style="background: #222233; padding: 0.75rem; border-radius: 8px; margin-bottom: 1rem; font-size: 0.85rem; color: #cbd5e1;">
              <div>📍 <strong>Ponto de Encontro:</strong> {{ c.pontoEncontro }}</div>
              <div style="margin-top: 0.35rem;">⏰ <strong>Horário:</strong> {{ c.dataHoraSaida | date:'dd/MM/yyyy HH:mm' }}</div>
              <div style="margin-top: 0.35rem;">🚗 <strong>Veículo:</strong> {{ c.veiculoModelo || 'Não informado' }} ({{ c.veiculoPlaca || '---' }})</div>
            </div>

            <div style="display: flex; align-items: center; gap: 0.5rem; margin-bottom: 1.25rem;">
              <span style="width: 32px; height: 32px; border-radius: 50%; background: #374151; display: inline-flex; align-items: center; justify-content: center; font-size: 0.9rem;">👤</span>
              <span style="font-size: 0.9rem; font-weight: 600;">Condutor: {{ c.motoristaNome || 'Estudante' }}</span>
            </div>
          </div>

          <button (click)="reservar(c.id)" [disabled]="c.vagasDisponiveis <= 0" style="width: 100%; padding: 0.75rem; border-radius: 8px; background: #3b82f6; color: white; border: none; font-weight: 700; cursor: pointer;">
            {{ c.vagasDisponiveis > 0 ? 'Reservar Vaga no UniRide' : 'Carona Lotada' }}
          </button>
        </div>
      </div>
    </div>
  `,
})
export class CaronasFeedComponent implements OnInit {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  public auth = inject(AuthService);

  caronas = signal<Carona[]>([]);
  loading = signal(false);
  mensagemSucesso = signal('');
  mensagemErro = signal('');

  filtroOrigem = '';
  filtroDestino = '';

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['origem']) this.filtroOrigem = params['origem'];
      if (params['destino']) this.filtroDestino = params['destino'];
      if (params['payment'] === 'success') {
        this.mensagemSucesso.set('🎉 Assinatura UniRide Pro confirmada com sucesso! Suas reservas agora são ilimitadas.');
      }
      this.carregarCaronas();
    });
  }

  carregarCaronas() {
    this.loading.set(true);
    this.mensagemErro.set('');
    this.api.getCaronas({
      origem: this.filtroOrigem || undefined,
      destino: this.filtroDestino || undefined
    }).subscribe({
      next: (data) => {
        this.caronas.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.mensagemErro.set('Erro ao carregar lista de caronas.');
      }
    });
  }

  reservar(caronaId: number) {
    this.mensagemSucesso.set('');
    this.mensagemErro.set('');
    this.api.reservarCarona(caronaId).subscribe({
      next: (res) => {
        this.mensagemSucesso.set('✅ Vaga reservada com sucesso! Boa viagem.');
        this.carregarCaronas();
      },
      error: (err) => {
        this.mensagemErro.set(err.error?.erro ?? 'Erro ao solicitar reserva.');
      }
    });
  }

  assinarPro() {
    this.api.createCheckoutSession().subscribe({
      next: (res) => {
        if (res.checkoutUrl) {
          window.location.href = res.checkoutUrl;
        }
      },
      error: () => {
        this.mensagemErro.set('Não foi possível iniciar o checkout do Stripe no momento.');
      }
    });
  }
}
