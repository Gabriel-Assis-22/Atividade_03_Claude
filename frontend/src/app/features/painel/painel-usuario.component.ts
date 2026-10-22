import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { Carona, Reserva, Veiculo } from '../../../shared/models/models';

@Component({
  selector: 'app-painel-usuario',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div style="max-width: 1000px; margin: 0 auto; padding: 2rem 1rem; color: #fff;">
      <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem; flex-wrap: wrap; gap: 1rem;">
        <div>
          <h1 style="font-size: 2rem; margin: 0;">Painel de Controle — UniRide</h1>
          <p style="color: #94a3b8; margin: 0.25rem 0 0 0;">
            Conectado como: <strong>{{ auth.getCurrentUser()?.nome }}</strong> (Papel: {{ auth.getCurrentRole() }})
          </p>
        </div>
        <div>
          <button (click)="gerenciarAssinatura()" style="padding: 0.6rem 1.2rem; background: #4f46e5; color: white; border-radius: 8px; border: none; cursor: pointer; font-weight: 600;">
            💳 Gerenciar Assinatura Stripe
          </button>
        </div>
      </div>

      <!-- Alertas -->
      <div *ngIf="msgSucesso()" style="background: rgba(16, 185, 129, 0.2); border: 1px solid #10b981; color: #6ee7b7; padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem;">
        {{ msgSucesso() }}
      </div>
      <div *ngIf="msgErro()" style="background: rgba(239, 68, 68, 0.2); border: 1px solid #ef4444; color: #fca5a5; padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem;">
        {{ msgErro() }}
      </div>

      <!-- SEÇÃO DO MOTORISTA -->
      <div *ngIf="auth.isMotorista()" style="margin-bottom: 3rem;">
        <h2 style="font-size: 1.5rem; border-bottom: 1px solid rgba(255,255,255,0.1); padding-bottom: 0.5rem; margin-bottom: 1.5rem;">
          🚘 Área do Condutor
        </h2>

        <!-- Formulário de Cadastro de Veículo -->
        <div style="background: #181824; padding: 1.5rem; border-radius: 12px; margin-bottom: 2rem; border: 1px solid rgba(255,255,255,0.08);">
          <h3 style="margin-top: 0; font-size: 1.2rem;">Cadastrar Veículo</h3>
          <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1rem;">
            <div>
              <label style="font-size: 0.85rem; color: #94a3b8;">Modelo / Marca</label>
              <input type="text" [(ngModel)]="novoVeiculo.modelo" placeholder="Ex: Onix 1.0" class="form-input">
            </div>
            <div>
              <label style="font-size: 0.85rem; color: #94a3b8;">Placa</label>
              <input type="text" [(ngModel)]="novoVeiculo.placa" placeholder="ABC-1234" class="form-input">
            </div>
            <div>
              <label style="font-size: 0.85rem; color: #94a3b8;">Cor</label>
              <input type="text" [(ngModel)]="novoVeiculo.cor" placeholder="Prata" class="form-input">
            </div>
            <div>
              <label style="font-size: 0.85rem; color: #94a3b8;">Ano</label>
              <input type="number" [(ngModel)]="novoVeiculo.ano" placeholder="2022" class="form-input">
            </div>
          </div>
          <button (click)="salvarVeiculo()" style="margin-top: 1rem; padding: 0.6rem 1.4rem; background: #10b981; color: white; border: none; border-radius: 6px; cursor: pointer; font-weight: 600;">
            Salvar Veículo
          </button>
        </div>

        <!-- Formulário de Publicar Carona -->
        <div style="background: #181824; padding: 1.5rem; border-radius: 12px; margin-bottom: 2rem; border: 1px solid rgba(255,255,255,0.08);">
          <h3 style="margin-top: 0; font-size: 1.2rem;">Publicar Nova Rota de Carona</h3>
          <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1rem;">
            <div>
              <label style="font-size: 0.85rem; color: #94a3b8;">Veículo</label>
              <select [(ngModel)]="novaCarona.veiculoId" class="form-input">
                <option [ngValue]="0" disabled>Selecione um veículo</option>
                <option *ngFor="let v of veiculos()" [ngValue]="v.id">{{ v.modelo }} ({{ v.placa }})</option>
              </select>
            </div>
            <div>
              <label style="font-size: 0.85rem; color: #94a3b8;">Origem</label>
              <input type="text" [(ngModel)]="novaCarona.cidadeOrigem" placeholder="Marília" class="form-input">
            </div>
            <div>
              <label style="font-size: 0.85rem; color: #94a3b8;">Destino</label>
              <input type="text" [(ngModel)]="novaCarona.cidadeDestino" placeholder="Fatec Garça" class="form-input">
            </div>
            <div>
              <label style="font-size: 0.85rem; color: #94a3b8;">Ponto de Encontro</label>
              <input type="text" [(ngModel)]="novaCarona.pontoEncontro" placeholder="Posto Ipiranga Centro" class="form-input">
            </div>
            <div>
              <label style="font-size: 0.85rem; color: #94a3b8;">Data e Hora de Saída</label>
              <input type="datetime-local" [(ngModel)]="novaCarona.dataHoraSaida" class="form-input">
            </div>
            <div>
              <label style="font-size: 0.85rem; color: #94a3b8;">Vagas Oferecidas</label>
              <input type="number" [(ngModel)]="novaCarona.vagasTotais" min="1" max="6" class="form-input">
            </div>
            <div>
              <label style="font-size: 0.85rem; color: #94a3b8;">Valor do Rateio (R$)</label>
              <input type="number" [(ngModel)]="novaCarona.valorRateio" step="0.50" class="form-input">
            </div>
          </div>
          <button (click)="publicarCarona()" style="margin-top: 1rem; padding: 0.6rem 1.4rem; background: #3b82f6; color: white; border: none; border-radius: 6px; cursor: pointer; font-weight: 600;">
            Publicar Carona
          </button>
        </div>

        <!-- Minhas Caronas Publicadas -->
        <div>
          <h3>Minhas Rotas Publicadas</h3>
          <div *ngIf="minhasCaronas().length === 0" style="color: #94a3b8;">Você ainda não publicou nenhuma carona.</div>
          <div style="display: grid; gap: 1rem;">
            <div *ngFor="let c of minhasCaronas()" style="background: #222233; padding: 1rem; border-radius: 8px; display: flex; justify-content: space-between; align-items: center;">
              <div>
                <strong>{{ c.cidadeOrigem }} ➔ {{ c.cidadeDestino }}</strong>
                <div style="font-size: 0.85rem; color: #94a3b8;">Saída: {{ c.dataHoraSaida | date:'dd/MM/yyyy HH:mm' }} | Vagas: {{ c.vagasDisponiveis }}/{{ c.vagasTotais }} | Status: {{ c.status }}</div>
              </div>
              <button (click)="cancelarCarona(c.id)" style="background: #ef4444; border: none; color: white; padding: 0.4rem 0.8rem; border-radius: 6px; cursor: pointer;">
                Cancelar
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- SEÇÃO DO PASSAGEIRO: MINHAS RESERVAS -->
      <div>
        <h2 style="font-size: 1.5rem; border-bottom: 1px solid rgba(255,255,255,0.1); padding-bottom: 0.5rem; margin-bottom: 1.5rem;">
          🎒 Minhas Reservas de Carona
        </h2>
        <div *ngIf="minhasReservas().length === 0" style="color: #94a3b8;">
          Você não possui nenhuma reserva no momento. Vá até o feed para encontrar uma viagem!
        </div>
        <div style="display: grid; gap: 1rem;">
          <div *ngFor="let r of minhasReservas()" style="background: #181824; border: 1px solid rgba(255,255,255,0.1); padding: 1.25rem; border-radius: 10px; display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 1rem;">
            <div>
              <div style="font-weight: 700; font-size: 1.1rem;">{{ r.cidadeOrigem }} ➔ {{ r.cidadeDestino }}</div>
              <div style="font-size: 0.85rem; color: #94a3b8; margin-top: 0.25rem;">
                Data/Hora: {{ r.dataHoraSaida | date:'dd/MM/yyyy HH:mm' }} | Rateio: R$ {{ r.valorRateio?.toFixed(2) }} | Status: <strong>{{ r.status }}</strong>
              </div>
            </div>
            <button *ngIf="r.status === 'confirmada'" (click)="cancelarReserva(r.id)" style="background: #ef4444; color: white; border: none; padding: 0.5rem 1rem; border-radius: 6px; cursor: pointer; font-weight: 600;">
              Cancelar Reserva
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .form-input {
      width: 100%;
      padding: 0.6rem;
      border-radius: 6px;
      border: 1px solid rgba(255,255,255,0.15);
      background: #222233;
      color: white;
      margin-top: 0.25rem;
    }
  `]
})
export class PainelUsuarioComponent implements OnInit {
  private api = inject(ApiService);
  public auth = inject(AuthService);

  veiculos = signal<Veiculo[]>([]);
  minhasCaronas = signal<Carona[]>([]);
  minhasReservas = signal<Reserva[]>([]);

  msgSucesso = signal('');
  msgErro = signal('');

  novoVeiculo = {
    modelo: '',
    placa: '',
    cor: '',
    ano: 2022
  };

  novaCarona = {
    veiculoId: 0,
    cidadeOrigem: '',
    cidadeDestino: '',
    pontoEncontro: '',
    dataHoraSaida: '',
    vagasTotais: 3,
    valorRateio: 10.00
  };

  ngOnInit() {
    this.carregarDados();
  }

  carregarDados() {
    this.api.getMyReservas().subscribe(res => this.minhasReservas.set(res));

    if (this.auth.isMotorista()) {
      this.api.getMyVeiculos().subscribe(res => {
        this.veiculos.set(res);
        if (res.length > 0) this.novaCarona.veiculoId = res[0].id;
      });
      this.api.getMyCaronas().subscribe(res => this.minhasCaronas.set(res));
    }
  }

  salvarVeiculo() {
    this.msgSucesso.set('');
    this.msgErro.set('');
    if (!this.novoVeiculo.modelo || !this.novoVeiculo.placa) {
      this.msgErro.set('Preencha os dados do veículo.');
      return;
    }

    this.api.createVeiculo(this.novoVeiculo).subscribe({
      next: () => {
        this.msgSucesso.set('Veículo cadastrado com sucesso!');
        this.novoVeiculo = { modelo: '', placa: '', cor: '', ano: 2022 };
        this.carregarDados();
      },
      error: (err) => this.msgErro.set(err.error?.erro ?? 'Erro ao salvar veículo.')
    });
  }

  publicarCarona() {
    this.msgSucesso.set('');
    this.msgErro.set('');
    if (!this.novaCarona.veiculoId || !this.novaCarona.cidadeOrigem || !this.novaCarona.cidadeDestino) {
      this.msgErro.set('Preencha todos os campos da carona.');
      return;
    }

    this.api.createCarona({
      ...this.novaCarona,
      dataHoraSaida: new Date(this.novaCarona.dataHoraSaida).toISOString()
    }).subscribe({
      next: () => {
        this.msgSucesso.set('Carona publicada no feed com sucesso!');
        this.carregarDados();
      },
      error: (err) => this.msgErro.set(err.error?.erro ?? 'Erro ao publicar carona.')
    });
  }

  cancelarCarona(id: number) {
    if (!confirm('Deseja realmente cancelar esta viagem?')) return;
    this.api.cancelCarona(id).subscribe({
      next: () => {
        this.msgSucesso.set('Carona cancelada com sucesso.');
        this.carregarDados();
      },
      error: (err) => this.msgErro.set(err.error?.erro ?? 'Erro ao cancelar carona.')
    });
  }

  cancelarReserva(id: number) {
    if (!confirm('Deseja cancelar sua reserva? A vaga será liberada.')) return;
    this.api.cancelReserva(id).subscribe({
      next: () => {
        this.msgSucesso.set('Reserva cancelada com sucesso.');
        this.carregarDados();
      },
      error: (err) => this.msgErro.set(err.error?.erro ?? 'Erro ao cancelar reserva.')
    });
  }

  gerenciarAssinatura() {
    this.api.createCustomerPortal().subscribe({
      next: (res) => {
        if (res.portalUrl) {
          window.location.href = res.portalUrl;
        }
      },
      error: (err) => {
        this.msgErro.set(err.error?.erro ?? 'Não foi possível abrir o Stripe Customer Portal. Verifique se possui uma assinatura ativa.');
      }
    });
  }
}
