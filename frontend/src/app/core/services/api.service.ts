import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Movie, MovieDetail, Favorite, Comment, AuditLog, UserProfile, UploadPhotoResponse, Carona, Reserva, Veiculo, SaasMetricas } from '../../shared/models/models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  // Catalog
  getMovies() { return this.http.get<Movie[]>(`${this.base}/catalog`); }
  getMovie(id: number) { return this.http.get<MovieDetail>(`${this.base}/catalog/${id}`); }

  // Favorites
  getFavorites() { return this.http.get<Favorite[]>(`${this.base}/favorites`); }
  checkFavorite(movieId: number) { return this.http.get<{ isFavorito: boolean }>(`${this.base}/favorites/${movieId}/check`); }
  addFavorite(body: { tmdbMovieId: number; titulo: string; posterPath: string }) {
    return this.http.post(`${this.base}/favorites`, body);
  }
  removeFavorite(movieId: number) { return this.http.delete(`${this.base}/favorites/${movieId}`); }

  // Comments
  getComments(movieId: number) { return this.http.get<Comment[]>(`${this.base}/comments/${movieId}`); }
  addComment(body: { tmdbMovieId: number; texto: string }) {
    return this.http.post(`${this.base}/comments`, body);
  }
  deleteComment(id: number) {
    return this.http.delete(`${this.base}/comments/${id}`);
  }

  // Audit Logs (Admin)
  getLogs(limite: number = 50) {
    return this.http.get<AuditLog[]>(`${this.base}/logs?limite=${limite}`);
  }

  // Profile
  getMyProfile() {
    return this.http.get<UserProfile>(`${this.base}/profile/me`);
  }

  updateProfile(body: { targetUserId?: number; bio: string | null }) {
    return this.http.put<{ mensagem: string }>(`${this.base}/profile`, body);
  }

  uploadProfilePhoto(file: File) {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<UploadPhotoResponse>(`${this.base}/profile/photo`, formData);
  }

  // Payments / Stripe Subscriptions & MRR
  createCheckoutSession() {
    return this.http.post<{ checkoutUrl: string }>(`${this.base}/payments/checkout`, {});
  }

  createCustomerPortal() {
    return this.http.post<{ portalUrl: string }>(`${this.base}/payments/portal`, {});
  }

  getSaasMetricas() {
    return this.http.get<SaasMetricas>(`${this.base}/payments/metricas`);
  }

  // ── UniRide Caronas & Mobilidade ───────────────────────────────────────────
  getCaronas(params?: { origem?: string; destino?: string; data?: string }) {
    let query = '';
    if (params) {
      const q = new URLSearchParams();
      if (params.origem) q.append('origem', params.origem);
      if (params.destino) q.append('destino', params.destino);
      if (params.data) q.append('data', params.data);
      query = q.toString() ? `?${q.toString()}` : '';
    }
    return this.http.get<Carona[]>(`${this.base}/caronas${query}`);
  }

  getCarona(id: number) {
    return this.http.get<Carona>(`${this.base}/caronas/${id}`);
  }

  getMyCaronas() {
    return this.http.get<Carona[]>(`${this.base}/caronas/minhas`);
  }

  createCarona(body: { veiculoId: number; cidadeOrigem: string; cidadeDestino: string; pontoEncontro: string; dataHoraSaida: string; vagasTotais: number; valorRateio: number }) {
    return this.http.post<{ id: number; mensagem: string }>(`${this.base}/caronas`, body);
  }

  cancelCarona(id: number) {
    return this.http.delete<{ mensagem: string }>(`${this.base}/caronas/${id}`);
  }

  reservarCarona(caronaId: number) {
    return this.http.post<{ reservaId: number; mensagem: string }>(`${this.base}/caronas/${caronaId}/reservar`, {});
  }

  getMyReservas() {
    return this.http.get<Reserva[]>(`${this.base}/reservas/minhas`);
  }

  cancelReserva(reservaId: number) {
    return this.http.post<{ mensagem: string }>(`${this.base}/reservas/${reservaId}/cancelar`, {});
  }

  getMyVeiculos() {
    return this.http.get<Veiculo[]>(`${this.base}/veiculos/meus`);
  }

  createVeiculo(body: { modelo: string; placa: string; cor: string; ano: number; fotoUrl?: string }) {
    return this.http.post<{ id: number; mensagem: string }>(`${this.base}/veiculos`, body);
  }
}
