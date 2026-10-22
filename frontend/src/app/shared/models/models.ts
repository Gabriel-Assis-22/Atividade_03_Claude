export interface Movie {
  id: number;
  titulo: string;
  posterUrl: string;
  ano: string;
}

export interface MovieDetail {
  id: number;
  titulo: string;
  sinopse: string;
  posterUrl: string | null;
  posterPath: string;
  ano: string;
  nota: string;
}

export interface Favorite {
  id: number;
  tmdbMovieId: number;
  titulo: string;
  posterPath: string | null;
  criadoEm: string;
}

export interface Comment {
  id: number;
  usuarioId: number;
  usuarioNome: string;
  tmdbMovieId: number;
  texto: string;
  criadoEm: string;
}

export interface AuthResponse {
  token: string;
  nome: string;
  email: string;
  role?: string;
  isPremium?: boolean;
}

export interface AuditLog {
  id: string;
  usuarioId: number | null;
  acao: string;
  detalhes: string | null;
  ipOrigem: string | null;
  timestamp: string;
}

export interface UserProfile {
  id: number;
  nome: string;
  email: string;
  role: string;
  fotoUrl: string | null;
  bio: string | null;
  isPremium?: boolean;
  criadoEm: string;
  favoritos: Favorite[];
}

export interface UploadPhotoResponse {
  fotoUrl: string;
  mensagem: string;
}

// ── Modelos do UniRide SaaS ──────────────────────────────────────────────────
export interface Veiculo {
  id: number;
  usuarioId: number;
  modelo: string;
  placa: string;
  cor: string;
  ano: number;
  fotoUrl: string | null;
  createdAt: string;
}

export interface Carona {
  id: number;
  motoristaId: number;
  veiculoId: number;
  cidadeOrigem: string;
  cidadeDestino: string;
  pontoEncontro: string;
  dataHoraSaida: string;
  vagasTotais: number;
  vagasDisponiveis: number;
  valorRateio: number;
  status: string;
  createdAt: string;
  motoristaNome?: string;
  motoristaFotoUrl?: string;
  veiculoModelo?: string;
  veiculoPlaca?: string;
}

export interface Reserva {
  id: number;
  caronaId: number;
  passageiroId: number;
  status: string;
  createdAt: string;
  passageiroNome?: string;
  cidadeOrigem?: string;
  cidadeDestino?: string;
  dataHoraSaida?: string;
  valorRateio?: number;
}

