-- Executar UMA ÚNICA VEZ no banco IAC_2026_02_gabriel_assis da disciplina
-- (via DBeaver, MySQL Workbench ou linha de comando)

CREATE TABLE IF NOT EXISTS usuarios (
  id INT AUTO_INCREMENT PRIMARY KEY,
  nome VARCHAR(100) NOT NULL,
  email VARCHAR(150) UNIQUE NOT NULL,
  senha_hash VARCHAR(255) NOT NULL,
  role VARCHAR(20) NOT NULL DEFAULT 'usuario',
  foto_chave VARCHAR(255) NULL,
  foto_url VARCHAR(500) NULL,
  bio VARCHAR(500) NULL,
  criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS favoritos (
  id INT AUTO_INCREMENT PRIMARY KEY,
  usuario_id INT NOT NULL,
  tmdb_movie_id INT NOT NULL,
  titulo VARCHAR(255) NOT NULL,
  poster_path VARCHAR(255),
  criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (usuario_id) REFERENCES usuarios(id),
  UNIQUE (usuario_id, tmdb_movie_id)
);

CREATE TABLE IF NOT EXISTS comentarios (
  id INT AUTO_INCREMENT PRIMARY KEY,
  usuario_id INT NOT NULL,
  tmdb_movie_id INT NOT NULL,
  texto TEXT NOT NULL,
  criado_em TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (usuario_id) REFERENCES usuarios(id)
);

-- ==========================================
-- Tabelas do SaaS UniRide (Atividade 10 e 11)
-- ==========================================

CREATE TABLE IF NOT EXISTS veiculos (
  id INT AUTO_INCREMENT PRIMARY KEY,
  usuario_id INT NOT NULL,
  modelo VARCHAR(100) NOT NULL,
  placa VARCHAR(20) NOT NULL,
  cor VARCHAR(50) NOT NULL,
  ano INT NOT NULL,
  foto_url VARCHAR(500) NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (usuario_id) REFERENCES usuarios(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS caronas (
  id INT AUTO_INCREMENT PRIMARY KEY,
  motorista_id INT NOT NULL,
  veiculo_id INT NOT NULL,
  cidade_origem VARCHAR(100) NOT NULL,
  cidade_destino VARCHAR(100) NOT NULL,
  ponto_encontro VARCHAR(255) NOT NULL,
  data_hora_saida DATETIME NOT NULL,
  vagas_totais INT NOT NULL,
  vagas_disponiveis INT NOT NULL,
  valor_rateio DECIMAL(10, 2) NOT NULL DEFAULT 0.00,
  status VARCHAR(20) NOT NULL DEFAULT 'disponivel',
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  INDEX idx_caronas_busca (cidade_origem, cidade_destino, data_hora_saida),
  FOREIGN KEY (motorista_id) REFERENCES usuarios(id) ON DELETE CASCADE,
  FOREIGN KEY (veiculo_id) REFERENCES veiculos(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS reservas (
  id INT AUTO_INCREMENT PRIMARY KEY,
  carona_id INT NOT NULL,
  passageiro_id INT NOT NULL,
  status VARCHAR(20) NOT NULL DEFAULT 'confirmada',
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  UNIQUE KEY uk_reserva_carona_passageiro (carona_id, passageiro_id),
  FOREIGN KEY (carona_id) REFERENCES caronas(id) ON DELETE CASCADE,
  FOREIGN KEY (passageiro_id) REFERENCES usuarios(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS avaliacoes (
  id INT AUTO_INCREMENT PRIMARY KEY,
  carona_id INT NOT NULL,
  avaliador_id INT NOT NULL,
  avaliado_id INT NOT NULL,
  nota INT NOT NULL,
  comentario VARCHAR(500) NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (carona_id) REFERENCES caronas(id) ON DELETE CASCADE,
  FOREIGN KEY (avaliador_id) REFERENCES usuarios(id) ON DELETE CASCADE,
  FOREIGN KEY (avaliado_id) REFERENCES usuarios(id) ON DELETE CASCADE
);

