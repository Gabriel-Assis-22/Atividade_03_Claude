import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApiService } from '../../../core/services/api.service';
import { AuthService } from '../../../core/services/auth.service';
import { UserProfile } from '../../../shared/models/models';

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <header class="site-header">
      <div class="header-inner">
        <a routerLink="/catalog" class="logo">🎬 Tom Hanks</a>
        <nav class="header-nav">
          <a routerLink="/catalog">Catálogo</a>
          <a routerLink="/favorites">Meus Favoritos</a>
          <a routerLink="/profile" class="active">Meu Perfil</a>
          <a (click)="auth.logout()" class="btn-logout" style="cursor:pointer">Sair</a>
        </nav>
      </div>
    </header>

    <main class="main-content">
      <div *ngIf="mensagemSucesso()" class="alert alert-success">
        {{ mensagemSucesso() }}
      </div>
      <div *ngIf="mensagemErro()" class="alert alert-error">
        {{ mensagemErro() }}
      </div>

      <div *ngIf="loading()" class="empty-state">
        <span class="empty-icon">⏳</span>
        <p>Carregando perfil...</p>
      </div>

      <div *ngIf="!loading() && perfil() as user" class="profile-container">
        <!-- Card Principal do Perfil -->
        <section class="profile-card">
          <div class="profile-header">
            <!-- Avatar & Upload de Foto -->
            <div class="avatar-wrapper">
              <div class="avatar-container" [class.avatar-loading]="uploading()">
                <img
                  *ngIf="user.fotoUrl"
                  [src]="user.fotoUrl"
                  [alt]="user.nome"
                  class="avatar-image"
                />
                <div *ngIf="!user.fotoUrl" class="avatar-placeholder">
                  {{ user.nome.charAt(0).toUpperCase() }}
                </div>

                <div *ngIf="uploading()" class="avatar-spinner">
                  <span>Enviando para o MinIO...</span>
                </div>
              </div>

              <label class="btn-change-photo" title="Escolha uma imagem de até 2MB (.jpg, .png, .webp)">
                <input
                  type="file"
                  accept="image/png, image/jpeg, image/jpg, image/webp"
                  (change)="onFileSelected($event)"
                  style="display:none"
                  [disabled]="uploading()"
                />
                📷 <span>Trocar Foto</span>
              </label>
            </div>

            <!-- Dados do Usuário -->
            <div class="profile-details">
              <div class="profile-title-row">
                <h1>{{ user.nome }}</h1>
                <span class="role-badge" [class.admin]="user.role === 'admin'">
                  {{ user.role === 'admin' ? 'Administrador' : 'Usuário' }}
                </span>
                <span *ngIf="user.isPremium" class="premium-badge" title="Assinante Plano Premium Ativo">
                  ⭐ Premium
                </span>
              </div>
              <p class="user-email">✉️ {{ user.email }}</p>
              <p class="user-joined">Membro desde: {{ user.criadoEm | date:'dd/MM/yyyy' }}</p>

              <!-- Bio -->
              <div class="bio-section">
                <div class="bio-header">
                  <h3>Biografia</h3>
                  <button
                    *ngIf="!editandoBio()"
                    (click)="abrirEdicaoBio()"
                    class="btn-edit-bio"
                  >
                    ✏️ Editar
                  </button>
                </div>

                <div *ngIf="!editandoBio()">
                  <p class="bio-text" *ngIf="user.bio">{{ user.bio }}</p>
                  <p class="bio-empty" *ngIf="!user.bio">Nenhuma biografia informada ainda. Clique em editar para adicionar!</p>
                </div>

                <div *ngIf="editandoBio()" class="bio-edit-form">
                  <textarea
                    [(ngModel)]="bioTexto"
                    rows="3"
                    maxlength="500"
                    placeholder="Escreva algo sobre você..."
                    class="bio-textarea"
                  ></textarea>
                  <div class="bio-actions">
                    <button
                      (click)="salvarBio()"
                      [disabled]="savingBio()"
                      class="btn-save-bio"
                    >
                      {{ savingBio() ? 'Salvando...' : 'Salvar Bio' }}
                    </button>
                    <button
                      (click)="cancelarEdicaoBio()"
                      [disabled]="savingBio()"
                      class="btn-cancel-bio"
                    >
                      Cancelar
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Seção de Plano Premium / Stripe (Atividade 7) -->
          <div class="subscription-box" [class.is-premium]="user.isPremium">
            <div class="subscription-content">
              <div class="subscription-header">
                <span class="subscription-icon">{{ user.isPremium ? '⭐' : '💎' }}</span>
                <div>
                  <h4>{{ user.isPremium ? 'Plano Premium Ativo' : 'Plano Gratuito — Limite de 3 Favoritos' }}</h4>
                  <p>
                    {{ user.isPremium
                        ? 'Você tem acesso a favoritos ilimitados e selo de assinante na plataforma.'
                        : 'Contas gratuitas podem salvar no máximo 3 filmes nos favoritos. Assine o Plano Premium por R$ 9,90/mês (modo de teste Stripe) para desbloquear favoritos ilimitados!' }}
                  </p>
                </div>
              </div>
              <div class="subscription-actions" *ngIf="!user.isPremium">
                <button (click)="assinarPremium()" [disabled]="subscribing()" class="btn-upgrade-premium">
                  {{ subscribing() ? 'Abrindo Stripe...' : '💳 Assinar Plano Premium (R$ 9,90)' }}
                </button>
              </div>
              <div class="subscription-actions" *ngIf="user.isPremium">
                <span class="badge-premium-active">✓ Assinatura Ativa (Modo Teste)</span>
              </div>
            </div>
          </div>

          <!-- Seção de Teste de Segurança Anti-IDOR (Requisito 4 da Atividade) -->
          <div class="idor-test-box">
            <div class="idor-info">
              <h4>🛡️ Teste de Controle de Acesso Anti-IDOR (Requisito 4)</h4>
              <p>
                Demonstre a tentativa de alterar o perfil de outro usuário (fornecendo ID alheio). 
                O backend deve recusar com <strong>HTTP 403 Forbidden</strong>.
              </p>
            </div>
            <button
              (click)="testarIdor()"
              [disabled]="testingIdor()"
              class="btn-test-idor"
            >
              {{ testingIdor() ? 'Enviando ataque...' : 'Disparar Teste de IDOR (ID 99999)' }}
            </button>
          </div>
        </section>

        <!-- Filmes Favoritados do Usuário -->
        <section class="favorites-section">
          <div class="section-title">
            <h2>❤️ Filmes Favoritados ({{ user.favoritos.length }})</h2>
          </div>

          <div *ngIf="user.favoritos.length === 0" class="empty-favorites">
            <span class="empty-icon">🍿</span>
            <p>Você ainda não favoritou nenhum filme.</p>
            <a routerLink="/catalog" class="btn-primary">Explorar Catálogo</a>
          </div>

          <div *ngIf="user.favoritos.length > 0" class="movies-grid">
            <a
              *ngFor="let fav of user.favoritos"
              [routerLink]="['/movie', fav.tmdbMovieId]"
              class="movie-card"
            >
              <div class="movie-poster">
                <img
                  *ngIf="fav.posterPath"
                  [src]="'https://image.tmdb.org/t/p/w500' + fav.posterPath"
                  [alt]="fav.titulo"
                  loading="lazy"
                />
                <div *ngIf="!fav.posterPath" class="poster-placeholder">🎬</div>
              </div>
              <div class="movie-info">
                <h3 class="movie-title">{{ fav.titulo }}</h3>
                <span class="movie-year">Favoritado em {{ fav.criadoEm | date:'dd/MM/yyyy' }}</span>
              </div>
            </a>
          </div>
        </section>
      </div>
    </main>

    <footer class="site-footer">
      <p>ISW055 · Atividade 06 — Upload de Foto de Perfil com MinIO & MariaDB</p>
    </footer>
  `,
  styles: [`
    .profile-container {
      max-width: 1000px;
      margin: 0 auto;
      display: flex;
      flex-direction: column;
      gap: 2.5rem;
    }

    .profile-card {
      background: var(--clr-surface);
      border: 1px solid var(--clr-border);
      border-radius: var(--radius);
      padding: 2.5rem;
      box-shadow: var(--shadow);
    }

    .profile-header {
      display: flex;
      gap: 2.5rem;
      align-items: flex-start;
    }

    @media (max-width: 768px) {
      .profile-header {
        flex-direction: column;
        align-items: center;
        text-align: center;
      }
    }

    /* Avatar */
    .avatar-wrapper {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 1rem;
      flex-shrink: 0;
    }

    .avatar-container {
      width: 150px;
      height: 150px;
      border-radius: 50%;
      overflow: hidden;
      background: var(--clr-surface2);
      border: 3px solid var(--clr-accent);
      display: flex;
      align-items: center;
      justify-content: center;
      position: relative;
      box-shadow: 0 4px 16px rgba(0, 0, 0, 0.4);
    }

    .avatar-image {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }

    .avatar-placeholder {
      font-size: 3.5rem;
      font-weight: 700;
      color: var(--clr-accent);
    }

    .avatar-loading {
      opacity: 0.6;
    }

    .avatar-spinner {
      position: absolute;
      inset: 0;
      background: rgba(0, 0, 0, 0.7);
      display: flex;
      align-items: center;
      justify-content: center;
      color: #fff;
      font-size: 0.8rem;
      padding: 0.5rem;
      text-align: center;
    }

    .btn-change-photo {
      background: var(--clr-surface2);
      color: var(--clr-text);
      border: 1px solid var(--clr-border);
      border-radius: var(--radius-sm);
      padding: 0.5rem 1rem;
      font-size: 0.85rem;
      font-weight: 500;
      cursor: pointer;
      display: inline-flex;
      align-items: center;
      gap: 0.4rem;
      transition: all var(--transition);
    }

    .btn-change-photo:hover {
      background: var(--clr-accent);
      color: #000;
      border-color: var(--clr-accent);
    }

    /* Detalhes */
    .profile-details {
      flex: 1;
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }

    .profile-title-row {
      display: flex;
      align-items: center;
      gap: 1rem;
      flex-wrap: wrap;
    }

    .profile-title-row h1 {
      font-size: 1.8rem;
      font-weight: 700;
      color: var(--clr-text);
    }

    .role-badge {
      background: var(--clr-surface2);
      color: var(--clr-muted);
      border: 1px solid var(--clr-border);
      border-radius: 999px;
      padding: 0.25rem 0.75rem;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.5px;
    }

    .role-badge.admin {
      background: rgba(232, 168, 56, 0.15);
      color: var(--clr-accent);
      border-color: rgba(232, 168, 56, 0.4);
    }

    .user-email {
      font-size: 0.95rem;
      color: var(--clr-muted);
    }

    .user-joined {
      font-size: 0.85rem;
      color: var(--clr-muted);
      margin-bottom: 0.5rem;
    }

    /* Bio */
    .bio-section {
      background: var(--clr-surface2);
      border: 1px solid var(--clr-border);
      border-radius: var(--radius-sm);
      padding: 1.25rem;
      margin-top: 0.5rem;
    }

    .bio-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.5rem;
    }

    .bio-header h3 {
      font-size: 1rem;
      font-weight: 600;
      color: var(--clr-accent);
    }

    .btn-edit-bio {
      background: transparent;
      border: none;
      color: var(--clr-accent);
      font-size: 0.8rem;
      font-weight: 500;
      cursor: pointer;
    }

    .btn-edit-bio:hover {
      text-decoration: underline;
    }

    .bio-text {
      color: var(--clr-text);
      font-size: 0.95rem;
      white-space: pre-line;
    }

    .bio-empty {
      color: var(--clr-muted);
      font-style: italic;
      font-size: 0.9rem;
    }

    .bio-textarea {
      width: 100%;
      background: var(--clr-bg);
      border: 1px solid var(--clr-border);
      border-radius: var(--radius-sm);
      color: var(--clr-text);
      padding: 0.75rem;
      font-family: inherit;
      font-size: 0.95rem;
      resize: vertical;
      margin-bottom: 0.75rem;
    }

    .bio-textarea:focus {
      outline: none;
      border-color: var(--clr-accent);
    }

    .bio-actions {
      display: flex;
      gap: 0.5rem;
    }

    .btn-save-bio {
      background: var(--clr-accent);
      color: #000;
      border: none;
      border-radius: var(--radius-sm);
      padding: 0.4rem 1rem;
      font-size: 0.85rem;
      font-weight: 600;
      cursor: pointer;
    }

    .btn-cancel-bio {
      background: var(--clr-surface);
      color: var(--clr-muted);
      border: 1px solid var(--clr-border);
      border-radius: var(--radius-sm);
      padding: 0.4rem 0.8rem;
      font-size: 0.85rem;
      cursor: pointer;
    }

    /* IDOR Box */
    .idor-test-box {
      margin-top: 2rem;
      padding: 1.25rem;
      background: rgba(255, 95, 109, 0.08);
      border: 1px dashed rgba(255, 95, 109, 0.4);
      border-radius: var(--radius-sm);
      display: flex;
      justify-content: space-between;
      align-items: center;
      gap: 1.5rem;
      flex-wrap: wrap;
    }

    .idor-info h4 {
      color: var(--clr-error);
      font-size: 0.95rem;
      margin-bottom: 0.25rem;
    }

    .idor-info p {
      font-size: 0.85rem;
      color: var(--clr-muted);
      max-width: 600px;
    }

    .btn-test-idor {
      background: transparent;
      border: 1px solid var(--clr-error);
      color: var(--clr-error);
      padding: 0.5rem 1rem;
      border-radius: var(--radius-sm);
      font-size: 0.85rem;
      font-weight: 600;
      cursor: pointer;
      transition: all var(--transition);
      white-space: nowrap;
    }

    .btn-test-idor:hover {
      background: var(--clr-error);
      color: #fff;
    }

    /* Favoritos */
    .favorites-section {
      display: flex;
      flex-direction: column;
      gap: 1.5rem;
    }

    .section-title h2 {
      font-size: 1.5rem;
      font-weight: 700;
    }

    .empty-favorites {
      text-align: center;
      padding: 3rem 1rem;
      background: var(--clr-surface);
      border: 1px dashed var(--clr-border);
      border-radius: var(--radius);
    }

    .empty-favorites .empty-icon {
      font-size: 3rem;
      display: block;
      margin-bottom: 1rem;
    }

    .empty-favorites p {
      color: var(--clr-muted);
      margin-bottom: 1.5rem;
    }

    .btn-primary {
      background: var(--clr-accent);
      color: #000;
      padding: 0.6rem 1.5rem;
      border-radius: var(--radius-sm);
      font-weight: 600;
      display: inline-block;
      transition: opacity var(--transition);
    }

    .btn-primary:hover {
      opacity: 0.9;
    }

    /* Estilos Plano Premium / Stripe */
    .premium-badge {
      display: inline-flex;
      align-items: center;
      gap: 0.25rem;
      padding: 0.25rem 0.65rem;
      background: linear-gradient(135deg, #f59e0b, #d97706);
      color: #fff;
      font-size: 0.75rem;
      font-weight: 700;
      border-radius: 9999px;
      letter-spacing: 0.04em;
      text-transform: uppercase;
      box-shadow: 0 0 12px rgba(245, 158, 11, 0.4);
    }

    .subscription-box {
      margin-top: 1.5rem;
      padding: 1.25rem;
      border-radius: var(--radius-md);
      background: rgba(30, 41, 59, 0.6);
      border: 1px solid rgba(255, 255, 255, 0.1);
      transition: all var(--transition);
    }

    .subscription-box.is-premium {
      background: linear-gradient(135deg, rgba(245, 158, 11, 0.12), rgba(217, 119, 6, 0.05));
      border-color: rgba(245, 158, 11, 0.4);
    }

    .subscription-content {
      display: flex;
      flex-wrap: wrap;
      align-items: center;
      justify-content: space-between;
      gap: 1rem;
    }

    .subscription-header {
      display: flex;
      align-items: center;
      gap: 1rem;
      max-width: 650px;
    }

    .subscription-icon {
      font-size: 2rem;
      flex-shrink: 0;
    }

    .subscription-header h4 {
      margin: 0 0 0.25rem;
      font-size: 1rem;
      color: var(--clr-text);
    }

    .subscription-header p {
      margin: 0;
      font-size: 0.85rem;
      color: var(--clr-muted);
      line-height: 1.4;
    }

    .btn-upgrade-premium {
      background: linear-gradient(135deg, #f59e0b, #d97706);
      color: #fff;
      border: none;
      padding: 0.65rem 1.25rem;
      border-radius: var(--radius-sm);
      font-weight: 700;
      font-size: 0.9rem;
      cursor: pointer;
      box-shadow: 0 4px 14px rgba(245, 158, 11, 0.35);
      transition: all var(--transition);
      white-space: nowrap;
    }

    .btn-upgrade-premium:hover:not(:disabled) {
      transform: translateY(-2px);
      box-shadow: 0 6px 18px rgba(245, 158, 11, 0.5);
    }

    .btn-upgrade-premium:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .badge-premium-active {
      display: inline-block;
      padding: 0.45rem 0.9rem;
      border-radius: var(--radius-sm);
      background: rgba(34, 197, 94, 0.15);
      border: 1px solid rgba(34, 197, 94, 0.4);
      color: #4ade80;
      font-size: 0.85rem;
      font-weight: 600;
    }
  `]
})
export class ProfilePageComponent implements OnInit {
  private api = inject(ApiService);
  private route = inject(ActivatedRoute);
  auth = inject(AuthService);

  perfil = signal<UserProfile | null>(null);
  loading = signal<boolean>(true);
  uploading = signal<boolean>(false);
  savingBio = signal<boolean>(false);
  testingIdor = signal<boolean>(false);
  subscribing = signal<boolean>(false);

  editandoBio = signal<boolean>(false);
  bioTexto = signal<string>('');

  mensagemSucesso = signal<string>('');
  mensagemErro = signal<string>('');

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['payment'] === 'success') {
        this.mensagemSucesso.set('🎉 Parabéns! Seu Plano Premium foi confirmado com sucesso via Stripe! Aproveite seus favoritos ilimitados.');
      } else if (params['payment'] === 'cancelled') {
        this.mensagemErro.set('O processo de pagamento foi cancelado. Você pode assinar quando desejar.');
      }
    });

    this.carregarPerfil();
  }

  carregarPerfil() {
    this.loading.set(true);
    this.mensagemErro.set('');

    this.api.getMyProfile().subscribe({
      next: (data) => {
        this.perfil.set(data);
        this.bioTexto.set(data.bio || '');
        this.loading.set(false);
      },
      error: (err) => {
        this.mensagemErro.set('Erro ao carregar o perfil de usuário.');
        this.loading.set(false);
      }
    });
  }

  abrirEdicaoBio() {
    this.bioTexto.set(this.perfil()?.bio || '');
    this.editandoBio.set(true);
  }

  cancelarEdicaoBio() {
    this.editandoBio.set(false);
  }

  salvarBio() {
    this.savingBio.set(true);
    this.mensagemSucesso.set('');
    this.mensagemErro.set('');

    this.api.updateProfile({ bio: this.bioTexto() }).subscribe({
      next: (res) => {
        this.perfil.update(p => p ? { ...p, bio: this.bioTexto() } : null);
        this.editandoBio.set(false);
        this.savingBio.set(false);
        this.mensagemSucesso.set('Biografia atualizada com sucesso!');
        setTimeout(() => this.mensagemSucesso.set(''), 4000);
      },
      error: (err) => {
        this.mensagemErro.set(err.error?.erro || 'Erro ao salvar biografia.');
        this.savingBio.set(false);
      }
    });
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const file = input.files[0];

    // Validação de tipo de arquivo
    const allowed = ['image/jpeg', 'image/png', 'image/webp', 'image/jpg'];
    if (!allowed.includes(file.type)) {
      this.mensagemErro.set('Formato de arquivo inválido. Selecione uma imagem (.jpg, .png, .webp).');
      return;
    }

    // Validação de tamanho (máx 2MB)
    const maxBytes = 2 * 1024 * 1024;
    if (file.size > maxBytes) {
      this.mensagemErro.set('O arquivo de imagem excede o tamanho máximo permitido de 2 MB.');
      return;
    }

    this.uploading.set(true);
    this.mensagemSucesso.set('');
    this.mensagemErro.set('');

    this.api.uploadProfilePhoto(file).subscribe({
      next: (res) => {
        this.perfil.update(p => p ? { ...p, fotoUrl: res.fotoUrl } : null);
        this.uploading.set(false);
        this.mensagemSucesso.set('Foto de perfil enviada e atualizada no MinIO com sucesso!');
        setTimeout(() => this.mensagemSucesso.set(''), 4000);
      },
      error: (err) => {
        this.mensagemErro.set(err.error?.erro || 'Erro ao realizar upload da foto para o MinIO.');
        this.uploading.set(false);
      }
    });
  }

  testarIdor() {
    this.testingIdor.set(true);
    this.mensagemSucesso.set('');
    this.mensagemErro.set('');

    // Dispara intencionalmente um TargetUserId alheio (ex: 99999) para demonstrar a proteção anti-IDOR
    this.api.updateProfile({ targetUserId: 99999, bio: 'Ataque IDOR simulação' }).subscribe({
      next: () => {
        this.mensagemErro.set('Falha de segurança: o backend permitiu editar o perfil de outro usuário!');
        this.testingIdor.set(false);
      },
      error: (err) => {
        this.testingIdor.set(false);
        if (err.status === 403) {
          this.mensagemErro.set(
            `🛡️ Proteção Anti-IDOR validada com sucesso! O backend recusou a requisição com HTTP 403 Forbidden: "${err.error?.erro || 'Acesso negado'}"`
          );
        } else {
          this.mensagemErro.set(`Resposta inesperada: HTTP ${err.status}`);
        }
      }
    });
  }

  assinarPremium() {
    this.subscribing.set(true);
    this.mensagemErro.set('');
    this.mensagemSucesso.set('');

    this.api.createCheckoutSession().subscribe({
      next: (res) => {
        if (res?.checkoutUrl) {
          window.location.href = res.checkoutUrl;
        } else {
          this.mensagemErro.set('URL de checkout não retornada pelo servidor.');
          this.subscribing.set(false);
        }
      },
      error: (err) => {
        this.mensagemErro.set(err.error?.erro || 'Erro ao conectar ao checkout do Stripe.');
        this.subscribing.set(false);
      }
    });
  }
}
