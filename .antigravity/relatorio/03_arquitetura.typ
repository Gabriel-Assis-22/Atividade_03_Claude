#import "helpers.typ": *

= Arquitetura do SaaS

Esta seção descreve a arquitetura real do *UniRide SaaS* tal como implementada nos repositórios e orquestrada em containers Docker, detalhando cada microsserviço, a infraestrutura de execução, os fluxos síncronos e assíncronos de negócio e cobrança, o modelo relacional e os mecanismos de segurança.

== Visão geral e componentes

O sistema é modularizado em microsserviços independentes interligados pela rede virtual privada `app-network` (driver `bridge`). Apenas o container `frontend` (Nginx) possui porta publicada para o mundo externo (`:8208`), atuando simultaneamente como servidor da Single Page Application (SPA) e *API Gateway / Reverse Proxy* para o `backend` e para o armazenamento de imagens. Todos os demais serviços operam estritamente isolados na rede interna com a diretiva `expose`.

#evidencia(
  [Diagrama de componentes, fronteira pública e rede interna Docker do UniRide SaaS],
  arquivo: "prints/diagrama_componentes.svg",
)

#table(
  columns: (2.8cm, 1fr, 3.3cm, 2.5cm),
  fill: (x, y) => if y == 0 { luma(235) } else { none },
  table.header([*Componente*], [*Responsabilidade*], [*Tecnologia / imagem*], [*Público?*]),
  [`frontend`], [Entrega da SPA (Landing Page, Feed de Caronas, Painel do Condutor/Passageiro/Admin) e proxy reverso para `/api/`, `/swagger/` e `/storage/`], [Angular 19 + `nginx:alpine` (`./frontend`)], [Sim (`:8208` / domínio)],
  [`backend`], [API central do negócio: CRUD de veículos e caronas, reserva atômica de vagas, enforcement de cotas SaaS, proxy de mídia S3 e integração Stripe], [ASP.NET Core 10 C\# DDD (`./backend`)], [Não (`expose :8080`)],
  [`auth-service`], [Gestão de identidade desacoplada: registro, hash BCrypt, login com emissão de JWT, 3 papéis RBAC e recuperação de senha com TTL de 30 min], [ASP.NET Core 10 C\# DDD (`./auth-service`)], [Não (`expose :8081`)],
  [`log-service`], [Centralização de auditoria: recepção assíncrona de eventos (`POST /logs`) e consulta cronológica protegida para administradores (`GET /logs`)], [ASP.NET Core 10 Web API (`./log-service`)], [Não (`expose :8082`)],
  [`redis`], [Persistência *append-only* ordenada no tempo da trilha de auditoria e eventos de negócio na estrutura `audit_stream` (`XADD` / `XREVRANGE`)], [`redis:7-alpine`], [Não (`expose :6379`)],
  [`MariaDB`], [Persistência relacional ACID de usuários, planos/assinaturas, veículos, caronas, reservas, avaliações e tokens de redefinição de senha], [MariaDB 10 + Dapper + FluentMigrator], [Gerenciado externo (`:3306`)],
  [`garage` + `garage-init`], [Object Storage distribuído compatível com AWS S3 (assinatura SigV4) para armazenamento binário de fotos de veículos e avatares de perfil], [`dxflrs/garage:v1.0.1` (Rust) + Alpine], [Não (`expose :3900` — via proxy `/storage/`)],
  [`Stripe` / `Mailtrap`], [Serviços SaaS externos: Checkout recorrente, Customer Portal e Webhooks assinados (Stripe); disparo transacional de e-mails de reset (Mailtrap)], [APIs HTTPS externas (`Stripe.net` / HTTP)], [Webhook `POST` → Nginx → `backend`],
)

== Onde roda

A aplicação roda na mesma máquina virtual Linux e na mesma stack gerenciada pelo *Portainer* (`portainer.lapps.studio`) utilizadas desde o primeiro bimestre, respondendo sob o domínio público `https://gabriel-assis-isw055.lapps.studio` (encaminhado pelo proxy reverso da infraestrutura para a porta host `8208` do container `frontend`). A stack puxa o código diretamente do repositório público do produto (#link("https://github.com/Gabriel-Assis-22/Atividade_03_Claude")[github.com/Gabriel-Assis-22/Atividade_03_Claude], branch `main`, arquivo `docker-compose.yml`).

Todos os serviços de longa duração utilizam política `restart: unless-stopped`, enquanto o container auxiliar `garage-init` executa em modo *one-shot* (`restart: "no"`) apenas para inicializar o layout do nó e provisionar o bucket sobre os volumes persistentes `garage_meta` e `garage_data`. O `docker-compose.yml` não fixa travas rígidas de `mem_limit` por container, delegando o escalonamento de memória ao daemon Docker da VM; para preservar consumo enxuto na máquina compartilhada, optou-se deliberadamente por imagens base `alpine`/`aspnet:10.0` e pelo binário em Rust do `Garage S3` (\~35 MB de RAM em operação, frente a \~250 MB do MinIO tradicional).

Nenhum segredo ou credencial é versionado no Git. As configurações são injetadas exclusivamente pelas variáveis de ambiente da stack no Portainer (documentadas em `.env.example` apenas pelos nomes):

#table(
  columns: (3.2cm, 1fr),
  fill: (x, y) => if y == 0 { luma(235) } else { none },
  table.header([*Serviço*], [*Nomes das variáveis de ambiente declaradas (sem valores)*]),
  [`backend`], [`DB_HOST`, `DB_PORT`, `DB_USER`, `DB_PASSWORD`, `DB_NAME`, `JWT_SECRET`, `AUTH_SERVICE_URL`, `LOG_SERVICE_URL`, `FRONTEND_URL`, `ASPNETCORE_URLS`, `GARAGE_ENDPOINT`, `GARAGE_ACCESS_KEY`, `GARAGE_SECRET_KEY`, `GARAGE_BUCKET_NAME`, `GARAGE_PUBLIC_BASE_URL`, `STRIPE_SECRET_KEY`, `STRIPE_WEBHOOK_SECRET`, `STRIPE_PRICE_ID`],
  [`auth-service`], [`DB_HOST`, `DB_PORT`, `DB_USER`, `DB_PASSWORD`, `DB_NAME`, `JWT_SECRET`, `MAILTRAP_API_TOKEN`, `MAILTRAP_INBOX_ID`, `FRONTEND_URL`, `LOG_SERVICE_URL`, `ASPNETCORE_URLS`],
  [`log-service`], [`REDIS_CONNECTION`, `JWT_SECRET`, `ASPNETCORE_URLS`],
  [`garage-init`], [`GARAGE_ACCESS_KEY`, `GARAGE_SECRET_KEY`, `GARAGE_BUCKET_NAME`],
)

== Fluxo de uma requisição

Para ilustrar o caminho ponta a ponta através dos componentes da stack, considera-se a operação central do negócio: *um passageiro autenticado solicita a reserva de um assento em uma carona disponível* (`POST /api/caronas/{id}/reservar`).

#evidencia(
  [Fluxo completo de requisição: reserva de assento em carona atravessando Nginx, Backend, JWT, MariaDB e Redis Streams],
  arquivo: "prints/diagrama_fluxo_requisicao.svg",
)

1. *Origem no Navegador:* O estudante logado na SPA Angular clica em *"Reservar Vaga no UniRide"*. O interceptador HTTP (`jwt.interceptor.ts`) anexa automaticamente o cabeçalho `Authorization: Bearer <token>` emitido no login pelo `auth-service`.
2. *Entrada pelo Domínio e Proxy Reverso:* A requisição HTTPS chega ao subdomínio público, alcança a porta `:8208` do container `frontend` e é roteada pela regra `location /api/` do Nginx para `http://backend:8080/api/caronas/{id}/reservar`, preservando `X-Real-IP` e `X-Forwarded-For`.
3. *Autenticação e Papel (RBAC):* O middleware `JwtBearer` do `backend` valida localmente a assinatura criptográfica HMAC-SHA256 do token usando `JWT_SECRET` (Padrão B — Claims no Token) e extrai de forma confiável o identificador `CurrentUserId` e a claim `role`.
4. *Regras de Domínio e Transação no MariaDB:* O caso de uso `ReservaUseCases.ReservarAssentoAsync` consulta o `MariaDB` via `Dapper` para: (a) confirmar que a carona existe, está `disponivel` e possui vagas; (b) impedir que o motorista reserve a própria viagem; (c) barrar reserva duplicada (`uk_reserva_carona_passageiro`); e (d) verificar a coluna `is_premium` do passageiro em `usuarios` — se for plano gratuito (`is_premium = 0`) e já possuir 2 reservas confirmadas, interrompe com `HTTP 403 Forbidden` (`LIMITE_PLANO_FREE`). Caso aprovado, executa o decremento condicional atômico `UPDATE caronas SET vagas_disponiveis = vagas_disponiveis - 1 WHERE id = @Id AND vagas_disponiveis > 0` e insere o registro em `reservas`.
5. *Auditoria Assíncrona e Resposta:* O `AuditClient` envia `POST http://log-service:8082/logs` com o evento `reserva_solicitada` (ou `bloqueio_limite_plano`), persistido no Redis via `XADD audit_stream *`, e o `CaronasController` devolve `HTTP 201 Created` à SPA, que atualiza o contador de assentos na tela.

== Fluxo de assinatura

A monetização do *Plano UniRide Pro* (R\$ 14,90/mês) opera em regime de assinatura recorrente (`Mode = "subscription"`) processada integralmente pelo Stripe em modo de teste:

#evidencia(
  [Ciclo de vida da assinatura recorrente: Checkout Session, validação criptográfica de Webhook, MariaDB e Customer Portal],
  arquivo: "prints/diagrama_fluxo_assinatura.svg",
)

- *Início do Checkout (`POST /api/assinaturas/checkout`):* A partir do botão *"Assinar UniRide Pro"* (ou do botão *"Fazer Upgrade"* exibido quando o limite gratuito bloqueia a 3ª reserva), o `StripePaymentService` cria uma `Checkout.Session` com `Mode = "subscription"`, valor mensal de R\$ 14,90 (`STRIPE_PRICE_ID` ou `PriceData` recorrente em BRL), injetando o `usuarioId` tanto em `ClientReferenceId` quanto nos dicionários `Metadata` da sessão e da `SubscriptionData`. O navegador é redirecionado para a página hospedada pelo Stripe (onde se utiliza o cartão de teste `4242 4242 4242 4242`).
- *Recepção e Validação Criptográfica do Webhook (`POST /api/webhooks/stripe` e `/api/payments/webhook`):* Após a cobrança, o Stripe dispara uma requisição assíncrona ao backend contendo o cabeçalho `Stripe-Signature`. O servidor exige a presença desse cabeçalho (recusando chamadas sem assinatura com `HTTP 400 Bad Request`) e executa `EventUtility.ConstructEvent(jsonPayload, stripeSignature, _webhookSecret)`, que recalcula o HMAC-SHA256 do payload bruto contra `STRIPE_WEBHOOK_SECRET` e valida a janela temporal contra ataques de *replay*.
- *Transição de Estado no Banco e Idempotência:* O `HandleStripeWebhookUseCase` processa os eventos `checkout.session.completed`, `customer.subscription.created` e `customer.subscription.updated` (marcando `is_premium = 1` quando `status` é `active` ou `trialing` e salvando `stripe_customer_id` na tabela `usuarios`) e `customer.subscription.deleted` (rebaixando `is_premium = 0`). Caso o Stripe entregue *o mesmo evento duas vezes* (comportamento padrão *at-least-once* de webhooks), a operação `UPDATE usuarios SET is_premium = @IsPremium, stripe_customer_id = @StripeCustomerId WHERE id = @Id` é naturalmente *idempotente*: o estado resultante permanece idêntico, sem duplicação de registros nem inconsistência de saldo, retornando `200 OK` (`{"received": true}`).
- *Cancelamento pelo Portal do Cliente (`POST /api/assinaturas/portal`):* No painel do usuário, o assinante aciona `BillingPortal.SessionService.CreateAsync` passando seu `stripe_customer_id`. No portal oficial do Stripe, ele pode cancelar a renovação ou trocar o meio de pagamento; o cancelamento emite `customer.subscription.deleted`, revertendo o usuário automaticamente às cotas do plano gratuito.

== Modelo de dados do domínio

O esquema relacional no `MariaDB` é gerenciado de forma automatizada e idempotente na subida dos containers pelo `FluentMigrator` (migrações `_001` a `_009` no `backend` e `_001` a `_004` no `auth-service`, com tabelas de controle de versão segregadas). A ligação entre identidade, plano comercial e assinatura no Stripe reside diretamente na tabela `usuarios` por meio das colunas `role`, `is_premium` (`0` = Passageiro Free; `1` = UniRide Pro) e `stripe_customer_id` (`cus_...`).

#evidencia(
  [Diagrama Entidade-Relacionamento (DER) das tabelas do domínio UniRide no MariaDB],
  arquivo: "prints/diagrama_modelo_dados.svg",
)

#table(
  columns: (2.8cm, 3.6cm, 1fr),
  fill: (x, y) => if y == 0 { luma(235) } else { none },
  table.header([*Tabela*], [*PK / Chaves Estrangeiras*], [*Colunas principais e papel no domínio*]),
  [`usuarios`], [`PK id`], [`nome`, `email` (`UNIQUE`), `senha_hash`, `role` (`passageiro`, `motorista`, `admin`), `is_premium` (plano ativo), `stripe_customer_id` (vínculo Stripe), `foto_chave`, `foto_url`, `bio`, `criado_em`],
  [`veiculos`], [`PK id` \ `FK usuario_id → usuarios.id`], [`modelo`, `placa`, `cor`, `ano`, `foto_url` (chave pública no Garage S3), `created_at`. Exclusivo de condutores],
  [`caronas`], [`PK id` \ `FK motorista_id → usuarios.id` \ `FK veiculo_id → veiculos.id`], [`cidade_origem`, `cidade_destino`, `ponto_encontro`, `data_hora_saida`, `vagas_totais`, `vagas_disponiveis`, `valor_rateio`, `status` (`disponivel`, `cancelada`). Índice composto `idx_caronas_busca`],
  [`reservas`], [`PK id` \ `FK carona_id → caronas.id` \ `FK passageiro_id → usuarios.id`], [`status` (`confirmada`, `cancelada`), `created_at`. Constraint única `uk_reserva_carona_passageiro` evitando dupla reserva na mesma viagem],
  [`avaliacoes`], [`PK id` \ `FK carona_id → caronas.id` \ `FK avaliador_id, avaliado_id`], [`nota`, `comentario`, `created_at`. Reputação mútua entre condutores e passageiros após viagens],
  [`reset_tokens`], [`PK id` \ `FK usuario_id → usuarios.id`], [`token` (32 bytes hex único), `criado_em`, `expira_em` (+30 min), `usado`. Mantida pelo `auth-service`],
)

== Segurança

O modelo de segurança combina controle de acesso baseado em papéis (*RBAC*) avaliado no servidor via atributos `[Authorize(Roles = "...")]` com verificações explícitas de posse de recurso (*Anti-IDOR*) na camada de casos de uso:

#table(
  columns: (1fr, 2.3cm, 2.4cm, 2.4cm),
  align: (left, center, center, center),
  fill: (x, y) => if y == 0 { luma(235) } else { none },
  table.header([*Ação no sistema*], [*`admin`*], [*`motorista`*], [*`passageiro`*]),
  [Buscar e visualizar feed de caronas disponíveis (`GET /api/caronas`)], [sim], [sim], [sim],
  [Reservar assento em carona de terceiros (`POST /api/caronas/{id}/reservar`)], [sim], [sim], [sim (cota plano)],
  [Cadastrar veículo e enviar foto ao S3 (`POST /api/veiculos`)], [sim], [sim], [não (`403`)],
  [Publicar nova rota de carona (`POST /api/caronas`)], [sim], [sim], [não (`403`)],
  [Cancelar carona ou usar veículo pertencente a outro usuário], [moderação], [não (`403`)], [não (`403`)],
  [Alterar perfil/foto de outro usuário ou cancelar reserva alheia], [não (`403`)], [não (`403`)], [não (`403`)],
  [Consultar métricas financeiras de MRR (`GET /api/assinaturas/metricas`)], [sim], [não (`403`)], [não (`403`)],
  [Auditar trilha de eventos do Redis Streams (`GET /api/logs`)], [sim], [não (`403`)], [não (`403`)],
)

*Isolamento por usuário e proteção Anti-IDOR:* Em todos os endpoints autenticados (`VeiculosController`, `CaronasController`, `ReservasController`, `ProfileController`, `PaymentsController`), a identidade operante jamais é lida do corpo JSON ou de parâmetros manipuláveis pelo cliente, mas sim extraída criptograficamente das claims do JWT (`CurrentUserId`). Consultas de listagem pessoal (`GET /api/veiculos/meus`, `GET /api/caronas/minhas`, `GET /api/reservas/minhas`) filtram obrigatoriamente por `WHERE usuario_id = @CurrentUserId`. Nas mutações (`CriarCaronaAsync`, `CancelarCaronaAsync`, `CancelarReservaAsync`, `UpdateProfileUseCase`), o caso de uso confere no banco se o recurso pertence ao `solicitanteId`; qualquer divergência lança `UnauthorizedAccessException`, respondida com `HTTP 403 Forbidden` e registrada imediatamente no `log-service` (`bloqueio_seguranca` / `tentativa_idor_bloqueada`).

*Gestão de segredos:* Chaves privadas (`JWT_SECRET`, `DB_PASSWORD`, `STRIPE_SECRET_KEY`, `STRIPE_WEBHOOK_SECRET`, `GARAGE_SECRET_KEY`, `MAILTRAP_API_TOKEN`) residem exclusivamente nas variáveis de ambiente da stack no Portainer e no arquivo `.env` local ignorado pelo `.gitignore`. No repositório público consta apenas `.env.example` com chaves ilustrativas sem valor real.

== Observabilidade, CI/CD e documentação da API

Entre as entregas extras opcionais da disciplina, foi implementada integralmente a *Documentação Interativa de API com Swagger / OpenAPI 3.0* (`Swashbuckle.AspNetCore`), acessível na rota pública `/swagger/` (via proxy reverso Nginx) e versionada estaticamente em `docs/openapi.json`, incluindo suporte nativo ao esquema de segurança `Bearer JWT` para teste direto de todos os contratos de Caronas, Veículos, Reservas, Assinaturas Stripe, Perfil e Logs. Na vertente de observabilidade de negócio e segurança, o sistema conta com o painel administrativo de indicadores SaaS (`GET /api/assinaturas/metricas` calculando usuários totais, Free, Pro e MRR) e com a trilha centralizada de eventos no `log-service` / Redis Streams. As atividades extras opcionais de pipeline CI/CD no GitHub Actions e endpoints dedicados de métricas Prometheus não foram implementadas neste ciclo.

#evidencia(
  [Especificação OpenAPI 3.0 / Swagger UI do UniRide SaaS em execução na rota /swagger],
  arquivo: "prints/arquitetura_swagger_ui.png",
)

== Camadas IaaS, PaaS e SaaS do projeto

Aplicando ao produto final UniRide a classificação arquitetural de computação em nuvem estudada na Atividade 8, as responsabilidades operacionais distribuem-se nas três camadas abaixo:

#table(
  columns: (2cm, 1fr, 3.8cm),
  fill: (x, y) => if y == 0 { luma(235) } else { none },
  table.header([*Camada*], [*O que é no projeto UniRide*], [*Quem gerencia*]),
  [*IaaS*], [Máquina virtual Linux provisionada no cluster da disciplina (vCPU, memória RAM, armazenamento em disco virtual, rede e roteamento de borda para `lapps.studio`)], [Infraestrutura da Fatec / Prof. Allan Siriani (SO e daemon sob uso da stack)],
  [*PaaS*], [Ambiente de orquestração *Docker Engine + Portainer*, banco relacional compartilhado *MariaDB* (`:3306`) e containers de plataforma (`redis:7-alpine`, `garage:v1.0.1`, runtime `aspnet:10.0` e `nginx:alpine`) sobre a rede `app-network`], [Plataforma base pela Fatec/Portainer; definição de `Dockerfile`s, `docker-compose.yml` e migrações pelo aluno],
  [*SaaS*], [1) *O próprio UniRide*, entregue pronto via navegador ao estudante universitário final sem instalação local; \ 2) *Serviços SaaS de terceiros consumidos via API:* *Stripe* (pagamentos, portal e webhooks), *Mailtrap* (e-mails transacionais) e *GitHub* (hospedagem Git e issues)], [UniRide operado pelo aluno desenvolvedor; Stripe, Mailtrap e GitHub gerenciados integralmente pelos respectivos provedores],
)
