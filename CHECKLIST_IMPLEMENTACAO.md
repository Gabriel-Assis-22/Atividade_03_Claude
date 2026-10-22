# Checklist de Implementação — UniRide SaaS

> **Diretrizes do Projeto:**
> - **KISS (Keep It Simple, Stupid):** Manter a implementação o mais simples, direta e enxuta possível, evitando sobre-engenharia.
> - **Padrão e Consistência:** Manter convenções unificadas de nomenclatura, arquitetura em camadas e boas práticas de código limpo.
> - **Rastreabilidade:** Marcar cada etapa com `[x]` à medida que for finalizada e testada.

---

## 📌 Atividade 10 — Domínio de Caronas, Autenticação e Interface

### 1. Modelagem de Dados & Migrations (MariaDB / FluentMigrator)
- [x] Criar migration para tabela `veiculos` (`id`, `usuario_id`, `modelo`, `placa`, `cor`, `ano`, `foto_url`, `created_at`).
- [x] Criar migration para tabela `caronas` (`id`, `motorista_id`, `veiculo_id`, `cidade_origem`, `cidade_destino`, `ponto_encontro`, `data_hora_saida`, `vagas_totais`, `vagas_disponiveis`, `valor_rateio`, `status`, `created_at`).
- [x] Criar migration para tabela `reservas` (`id`, `carona_id`, `passageiro_id`, `status`, `created_at`).
- [x] Criar migration para tabela `avaliacoes` (`id`, `carona_id`, `avaliador_id`, `avaliado_id`, `nota`, `comentario`, `created_at`).
- [x] Configurar chaves estrangeiras (`FK`), índices de busca (`origem`, `destino`, `data_hora_saida`) e execução das migrations com sucesso.

---

### 2. Autenticação e Autorização RBAC (`auth-service`)
- [x] Adicionar suporte aos papéis (Roles): `passageiro`, `motorista` e `admin` no modelo de usuário/registro.
- [x] Incluir claim de `role` no payload do JWT gerado no login.
- [x] Configurar middleware de autorização e aplicar `[Authorize(Roles = "...")]` nas rotas do backend.
- [x] Restringir publicação de veículos e caronas exclusivamente para condutores/motoristas.

---

### 3. API REST do Domínio de Caronas & Regras de Negócio
- [x] **CRUD de Veículos:**
  - [x] `POST /api/veiculos` — Cadastrar veículo (apenas motorista).
  - [x] `GET /api/veiculos/meus` — Listar veículos do motorista logado.
- [x] **Gerenciamento de Caronas:**
  - [x] `POST /api/caronas` — Publicar carona (apenas motorista).
  - [x] `GET /api/caronas` — Listar caronas disponíveis com filtros simples (origem, destino, data).
  - [x] `GET /api/caronas/{id}` — Detalhes da carona.
  - [x] `DELETE /api/caronas/{id}` ou cancelamento — Validação anti-IDOR (apenas o criador da carona pode cancelar/editar).
- [x] **Reservas de Assentos:**
  - [x] `POST /api/caronas/{id}/reservar` — Solicitar reserva.
  - [x] Validações na reserva: impedir reserva na própria carona, evitar duplicidade e decrementar `vagas_disponiveis` de forma atômica.
  - [x] `POST /api/reservas/{id}/cancelar` — Cancelar reserva (anti-IDOR: apenas o passageiro titular ou o motorista) e restaurar vaga.
  - [x] `GET /api/reservas/minhas` — Histórico de reservas do passageiro logado.

---

### 4. Auditoria com Redis Streams & Armazenamento de Arquivos
- [x] Emissão de eventos estruturados no Redis Streams (`carona_criada`, `reserva_solicitada`, `reserva_cancelada`).
- [x] Integração com endpoint de upload via Nginx (`/storage/`) apontando para o bucket Garage S3 (fotos de veículos/perfil).

---

### 5. Frontend (Interface Web em Angular)
- [x] **Landing Page Pública:**
  - [x] Apresentação simples da proposta de valor universitária.
  - [x] Tabela comparativa clara dos planos (Passageiro Free vs. UniRide Pro).
  - [x] Formulário simples de busca rápida de caronas (origem/destino).
- [x] **Feed e Busca de Caronas:**
  - [x] Cards de caronas disponíveis (motorista, trajeto, horário, vagas restantes e valor).
  - [x] Botão de ação rápida para solicitar reserva.
- [x] **Painel do Usuário:**
  - [x] Visão do Passageiro: lista de reservas ativas e histórico.
  - [x] Visão do Motorista: formulário enxuto de publicação de carona e lista de passageiros confirmados.

---

## 💳 Atividade 11 — Monetização e Cobrança Recorrente (Stripe)

### 1. Configuração do Stripe (Modo Teste)
- [x] Criar produtos e preços recorrentes no Stripe Dashboard (modo teste):
  - [x] Plano Passageiro Free (R$ 0,00/mês).
  - [x] Plano UniRide Pro (R$ 14,90/mês).
- [x] Configurar chaves de API do Stripe nas variáveis de ambiente do backend.

---

### 2. Checkout e Portal do Cliente
- [x] Endpoint `POST /api/assinaturas/checkout` — Criação da sessão do Stripe Checkout (`mode: 'subscription'`).
- [x] Endpoint `POST /api/assinaturas/portal` — Geração do link para o Stripe Customer Portal (gerenciamento e cancelamento pelo usuário).
- [x] Redirecionamento e páginas de feedback no frontend (`/sucesso` e `/cancelado`).

---

### 3. Webhooks do Stripe
- [x] Endpoint `POST /api/webhooks/stripe` para recepção de eventos.
- [x] Validação rigorosa da assinatura do webhook (`Stripe-Signature`).
- [x] Tratar eventos essenciais:
  - [x] `customer.subscription.created` / `updated`: atualizar status do plano do usuário para `Pro`.
  - [x] `customer.subscription.deleted`: rebaixar usuário para o plano `Free`.
  - [x] `invoice.payment_failed`: alertar ou suspender status caso o pagamento falhe.

---

### 4. Enforcement de Regras SaaS na API
- [x] Criar middleware ou verificação de cota nas rotas de reserva.
- [x] Bloquear com `HTTP 403 Forbidden` a criação de reservas para usuários do plano `Free` que já possuam **2 reservas ativas simultâneas**.
- [x] Liberar reservas ilimitadas para usuários com assinatura `Pro` ativa.

---

## 🧪 Validação e Entrega Final
- [x] Testes manuais do fluxo completo: Cadastro -> Publicação de Carona -> Reserva -> Notificação no Redis.
- [x] Teste do fluxo do Stripe: Assinar Pro via Checkout -> Webhook processado -> Liberação de limite.
- [x] Revisão de código: checagem de anti-IDOR, remoção de código morto e garantia de simplicidade do projeto.
