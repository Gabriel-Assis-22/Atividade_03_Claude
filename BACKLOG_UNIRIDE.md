# Backlog de Entregas — UniRide (Trilha P2: Atividades 10 e 11)

> Repositório: [github.com/Gabriel-Assis-22/UniRide_SaaS](https://github.com/Gabriel-Assis-22/UniRide_SaaS)  
> Professor: [@siriani](https://github.com/siriani)

Este documento reúne as **issues de engenharia** especificadas para o roteiro de evolução do produto nas Atividades 10 e 11, conforme exigido no item 4 da Atividade 9.

---

### Issue 1: Modelagem Relacional e Migrations das Entidades de Mobilidade
- **Título:** `[Atividade 10] Modelar tabelas de veículos, caronas e reservas no MariaDB`
- **Etiquetas:** `database`, `backend`, `migration`
- **Descrição:**
  - Criar migrations FluentMigrator para as tabelas `veiculos`, `caronas`, `reservas` e `avaliacoes`.
  - Configurar chaves estrangeiras com `usuarios.id` e índices para otimizar busca por `cidade_origem`, `cidade_destino` e `data_hora_saida`.
  - Garantir cláusulas de isolamento multi-tenant por usuário.

---

### Issue 2: Autenticação com 3+ Papéis (Passageiro, Motorista e Administrador)
- **Título:** `[Atividade 10] Implementar suporte aos papéis passageiro, motorista e admin no auth-service`
- **Etiquetas:** `auth-service`, `security`, `rbac`
- **Descrição:**
  - Estender as claims do token JWT no `auth-service` para suportar os papéis `passageiro`, `motorista` e `admin`.
  - Aplicar políticas de autorização `[Authorize(Roles = "...")]` nos endpoints do backend.
  - Proteger rotas de cadastro de veículos e viagens para que apenas condutores possam publicar.

---

### Issue 3: Endpoints REST e Regras de Negócio do Domínio de Caronas
- **Título:** `[Atividade 10] Construir API REST do domínio UniRide com isolamento estrito e proteção anti-IDOR`
- **Etiquetas:** `api`, `backend`, `use-cases`
- **Descrição:**
  - Desenvolver endpoints `GET /api/caronas` com filtros de cidade e data.
  - Endpoint `POST /api/caronas/{id}/reservar` decrementando vagas de forma atômica e validando duplicidade.
  - Implementar verificação estrita de titularidade para impedir que usuários alterem ou cancelem viagens de terceiros (anti-IDOR).

---

### Issue 4: Landing Page Pública e Interface do Produto em Angular 19
- **Título:** `[Atividade 10] Criar Landing Page pública e painéis do passageiro e condutor`
- **Etiquetas:** `frontend`, `angular`, `ui/ux`
- **Descrição:**
  - Construir Landing Page responsiva com proposta de valor, busca rápida de caronas e tabela de preços.
  - Desenvolver tela de feed de caronas disponíveis com card de condutor e vagas.
  - Desenvolver painel do motorista para gerenciamento de rotas e passageiros confirmados.

---

### Issue 5: Auditoria de Eventos no Redis Streams e Upload no Garage S3
- **Título:** `[Atividade 10] Integrar emissão de eventos de caronas no Redis e upload de veículos no S3`
- **Etiquetas:** `log-service`, `redis`, `storage`
- **Descrição:**
  - Emitir eventos estruturados no Redis Streams: `carona_criada`, `reserva_solicitada`, `reserva_cancelada` e bloqueios de segurança.
  - Adaptar o upload de imagens via proxy Nginx `/storage/` para armazenar fotos de veículos e documentos de condutores no bucket S3.

---

### Issue 6: Assinatura Recorrente com Stripe e Portal do Cliente
- **Título:** `[Atividade 11] Integrar Stripe Subscriptions, Customer Portal e enforcement de limites na API`
- **Etiquetas:** `payments`, `stripe`, `saas`
- **Descrição:**
  - Criar planos Free e Pro recorrentes no painel de teste do Stripe.
  - Implementar sessão de checkout no modo `subscription` e integração com Stripe Customer Portal.
  - Implementar webhooks de ciclo de vida (`customer.subscription.created/deleted`, `invoice.payment_succeeded/failed`).
  - Aplicar enforcement na API bloqueando usuários do plano gratuito com mais de 2 reservas ativas simultâneas (`HTTP 403 Forbidden`).
