#import "helpers.typ": *

= Atividades realizadas

Nesta seção são apresentadas as fichas detalhadas das cinco atividades obrigatórias do 2º bimestre (Atividades 7 a 11), contendo os hashes e links diretos verificáveis no GitHub, os horários exatos extraídos do controle de versão (fuso `America/Sao_Paulo`, UTC-3), as capturas comprobatórias de entrega (página web do GitHub acompanhada da verificação `git log` com data e hora explícitas) e o relato técnico das soluções adotadas.

#atividade(
  7, "Plano Premium — cobrança de verdade com Stripe",
  descricao: "Plano pago em modo de teste: Checkout Session do Stripe e webhook marcando o usuário como premium.",
  planejada: "09/10/2026",
  realizada: "19/10/2026 20:37",
  situacao: "entregue com atraso",
  evidencia: "GitHub — commit 59f4155 (19/10/2026 20:37) + fluxo de upgrade e bloqueio de cota do plano gratuito no UniRide",
  url: "https://github.com/Gabriel-Assis-22/Atividade_02_Claude/commit/59f4155b39f53bc7b8ade40b791db3016f4fa811",
)[
  *O que foi feito.* Implementou-se a integração ponta a ponta com o provedor de pagamentos *Stripe* (em modo de teste) utilizando o SDK `Stripe.net`, consolidada posteriormente no domínio do *UniRide SaaS*. No banco `MariaDB`, a `Migration_004_AddPremiumColumnsToUsuarios` adicionou as colunas `is_premium` e `stripe_customer_id` à tabela `usuarios`. No `backend` (.NET 10), foram construídos o `PaymentsController`, o serviço `StripePaymentService` para geração de `Checkout.Session` hospedada e o caso de uso `HandleStripeWebhookUseCase` para recepção assíncrona do evento `checkout.session.completed` com validação criptográfica HMAC-SHA256 via cabeçalho `Stripe-Signature`, promovendo o usuário ao plano pago sem transitar dados sensíveis de cartão pelo servidor (conformidade PCI-DSS) e desbloqueando o limite do plano gratuito (`HTTP 403 Forbidden` na tentativa de exceder a cota gratuita). A entrega inicial foi concluída em `19/10/2026 20:37` (10 dias após a data planejada de `09/10/2026`) em razão do acúmulo da consolidação do relatório da P1 na semana imediatamente anterior.

  #evidencia(
    [Atividade 7 — verificação no terminal Git com repositório, hash completo (59f4155) e data/hora exatas (19/10/2026 20:37 BRT)],
    arquivo: "prints/ativ07_gitlog.svg",
  )
  #evidencia(
    [Atividade 7 — painel autenticado do UniRide exibindo os controles de upgrade ("Assinar UniRide Pro") e autoatendimento Stripe],
    arquivo: "prints/ativ10_painel_motorista.png",
  )
  #evidencia(
    [Atividade 7 — resultado da diferenciação entre plano gratuito e pago: bloqueio HTTP 403 na cota gratuita e CTA de upgrade],
    arquivo: "prints/ativ11_limite_plano_403_ui.png",
  )

  *Dificuldades e como foram resolvidas.* A principal dificuldade técnica ocorreu na validação criptográfica do webhook: qualquer leitura prévia ou *model binding* automático do ASP.NET Core alterava o stream de bytes e invalidava o hash calculado por `EventUtility.ConstructEvent`. O problema foi resolvido lendo o corpo bruto diretamente via `StreamReader(HttpContext.Request.Body)` em endpoint `[AllowAnonymous]` dedicado e validando obrigatoriamente o segredo `STRIPE_WEBHOOK_SECRET` antes de acionar o caso de uso.
]

#pagebreak()

#atividade(
  8, "SaaS, PaaS, IaaS — e a documentação que o projeto nunca teve",
  descricao: "Camadas de nuvem do próprio projeto e documentação reversa: especificação, arquitetura, regras de negócio e modelo de dados.",
  planejada: "16/10/2026",
  realizada: "19/10/2026 20:59",
  situacao: "entregue com atraso",
  evidencia: "GitHub — commits 22af01e (20:54) e 89f32ef (19/10/2026 20:59) + documentação reversa, DER e Swagger UI do UniRide",
  url: "https://github.com/Gabriel-Assis-22/Atividade_02_Claude/commit/89f32ef54b2749581776d9d0d7df27cebb754985",
)[
  *O que foi feito.* Realizou-se o levantamento completo por engenharia reversa de todos os microsserviços, fluxos de dados, regras de negócio implícitas e tabelas relacionais do projeto, consolidando o manual arquitetural em `DOCUMENTACAO.md` (commit `22af01e` às `20:54`) e adicionando ao `README.md` principal o quadro classificatório das camadas de computação em nuvem (*IaaS*, *PaaS* e *SaaS*) aplicadas à própria stack do projeto, com referência cruzada direta para a documentação reversa (commit `89f32ef` às `20:59`). Na mesma noite (`19/10/2026 22:43`, commit `8cf880e`), implementou-se também a entrega extra opcional de documentação interativa *OpenAPI 3.0 / Swagger UI* (`Swashbuckle.AspNetCore`) nos serviços `.NET`, atualizada integralmente para os endpoints do domínio UniRide. A atividade registrou atraso de três dias em relação ao prazo de `16/10/2026` por depender da finalização prévia das tabelas e rotas financeiras da Atividade 7 para retratar o sistema completo sem lacunas.

  #evidencia(
    [Atividade 8 — verificação no terminal Git com repositório, hashes (22af01e, 89f32ef, 8cf880e) e datas/horas exatas (19/10/2026 BRT)],
    arquivo: "prints/ativ08_gitlog.svg",
  )
  #evidencia(
    [Atividade 8 — documentação estruturada e mapeamento de reaproveitamento arquitetural no repositório público do UniRide],
    arquivo: "prints/ativ09_readme_github.png",
  )
  #evidencia(
    [Atividade 8 — resultado da engenharia reversa: contratos da API documentados e testáveis no Swagger UI (/swagger)],
    arquivo: "prints/arquitetura_swagger_ui.png",
  )

  *Dificuldades e como foram resolvidas.* Mapear fielmente fronteiras que haviam crescido organicamente em seis containers exigiu auditar cada `Dockerfile`, `nginx.conf` e migração FluentMigrator para distinguir com precisão o que pertencia à camada PaaS gerenciada pela Fatec/Portainer e o que constituía SaaS próprio ou SaaS de terceiros (Stripe, Mailtrap e GitHub). Essa auditoria serviu de alicerce direto para executar a transição limpa para o domínio de caronas universitárias.
]

#pagebreak()

#atividade(
  9, "Fork e plano de negócio",
  descricao: "Repositório novo com o histórico do anterior, README do produto (dor, persona, Lean Canvas, modelo de receita, mapa de reaproveitamento) e backlog em issues.",
  planejada: "23/10/2026",
  realizada: "22/10/2026 20:32",
  situacao: "entregue",
  evidencia: "GitHub — Issue #1 mencionando @siriani (22/10/2026 20:32) + commit fb3abaa + README do produto e backlog",
  url: "https://github.com/Gabriel-Assis-22/Atividade_03_Claude/issues/1",
)[
  *O que foi feito.* Criou-se o novo repositório público dedicado ao produto SaaS (#link("https://github.com/Gabriel-Assis-22/Atividade_03_Claude")[github.com/Gabriel-Assis-22/Atividade_03_Claude]) a partir da base consolidada do projeto anterior, estruturando a concepção de negócio do *UniRide — Carona Universitária Compartilhada* no commit `fb3abaa` (`22/10/2026 13:37`) e formalizando a entrega dentro do prazo por meio da abertura da *Issue \#1* (`22/10/2026 20:32`) com menção direta a `@siriani`. Foram elaborados o documento executivo de produto (`README_UNIRIDE.md` / `README.md`) — contemplando problema real de deslocamento intermunicipal estudantil, personas primárias, *Lean Canvas* completo em 9 blocos, tabela de precificação recorrente (*Passageiro Free* vs *UniRide Pro* R\$ 14,90/mês) e matriz de reaproveitamento técnico dos microsserviços — além do mapeamento das entregas de engenharia (`BACKLOG_UNIRIDE.md`).

  #evidencia(
    [Atividade 9 — Issue \#1 aberta no repositório do produto mencionando `@siriani`],
    arquivo: "prints/ativ09_issue_github.png",
  )
  #evidencia(
    [Atividade 9 — verificação via GitHub API e Git do timestamp exato da Issue \#1 (22/10/2026 20:32 BRT) e do commit fb3abaa],
    arquivo: "prints/ativ09_gitlog.svg",
  )
  #evidencia(
    [Atividade 9 — resultado: especificação de negócio do UniRide com Lean Canvas, modelo de receita e reaproveitamento],
    arquivo: "prints/ativ09_readme_github.png",
  )

  *Dificuldades e como foram resolvidas.* Na migração para o repositório `Atividade_03_Claude`, a cópia inicial dos arquivos consolidou a base do 1º bimestre em um commit raiz (`7dcbc4f`) em vez de `git push --mirror`; para preservar total transparência histórica, o repositório anterior (`Atividade_02_Claude`) foi mantido público e referenciado explicitamente na documentação, permitindo auditar todos os hashes originais de agosto a outubro sem perda de rastreabilidade.
]

#pagebreak()

#atividade(
  10, "Seu negócio no ar — o domínio no lugar dos filmes",
  descricao: "Stack apontando para o repositório novo, tabelas e API do domínio, papéis do negócio, isolamento por usuário, eventos no log, upload e landing page.",
  planejada: "06/11/2026",
  realizada: "22/10/2026 13:38",
  situacao: "entregue",
  evidencia: "GitHub — commit a7b62c4 (22/10/2026 13:38) + prints da Landing Page, do Painel do Condutor e dos logs Anti-IDOR no Redis",
  url: "https://github.com/Gabriel-Assis-22/Atividade_03_Claude/commit/a7b62c482df12b305309814f70f51dae64354875",
)[
  *O que foi feito.* Substituiu-se integralmente o domínio anterior pelo núcleo operacional do *UniRide* (commit `a7b62c4` em `22/10/2026 13:38`). No banco `MariaDB`, as migrações `_006` a `_009` criaram as tabelas `veiculos`, `caronas`, `reservas` e `avaliacoes`. No `auth-service` e no `backend`, foram configurados os 3 papéis do negócio (`passageiro`, `motorista` e `admin`), CRUD de veículos com upload autenticado de fotos no `Garage S3`, oferta e busca filtrada de rotas, reserva atômica de assentos (`vagas_disponiveis - 1` condicional), proteção estrita *Anti-IDOR* (`HTTP 403 Forbidden` ao tentar manipular carona, veículo ou reserva de terceiro) e publicação de eventos de domínio no `log-service` / Redis Streams (`carona_criada`, `reserva_solicitada`, `bloqueio_seguranca`). No `frontend` Angular 19, desenvolveu-se a Landing Page pública com proposta de valor, busca rápida e comparativo de planos, além do Feed de Caronas e Painel do Condutor/Passageiro.

  #evidencia(
    [Atividade 10 — página do commit a7b62c4 no repositório do produto (Gabriel-Assis-22/Atividade_03_Claude)],
    arquivo: "prints/ativ10_commit_github.png",
  )
  #evidencia(
    [Atividade 10 — verificação no terminal Git com repositório, hash completo e data/hora exatas (22/10/2026 13:38 BRT)],
    arquivo: "prints/ativ10_gitlog.svg",
  )
  #evidencia(
    [Atividade 10 — resultado (1/3): Landing Page pública do UniRide com busca de rotas e comparativo de planos Free vs Pro],
    arquivo: "prints/ativ10_landing_uniride.png",
  )
  #evidencia(
    [Atividade 10 — resultado (2/3): Painel do Condutor autenticado (papel motorista) com cadastro de veículo, publicação de carona e isolamento das próprias rotas],
    arquivo: "prints/ativ10_painel_motorista.png",
  )
  #evidencia(
    [Atividade 10 — resultado (3/3): consulta administrativa GET /api/logs exibindo eventos de negócio (carona_criada) e bloqueio Anti-IDOR (bloqueio_seguranca) no Redis Streams],
    arquivo: "prints/ativ10_logs_redis_swagger.png",
  )

  *Dificuldades e como foram resolvidas.* Ao injetar auditoria transversal nos casos de uso de caronas e reservas, surgiu uma referência circular/indevida entre `Application` e `Infrastructure` durante o `dotnet publish` em container limpo, além da interceptação de assets `.js` do Swagger pela regra estática do Nginx. Ambos foram solucionados movendo o contrato `IAuditClient` para `Domain.Services` (respeitando o DDD) e aplicando o modificador `location ^~ /swagger/` no `nginx.conf`.
]

#pagebreak()

#atividade(
  11, "SaaS completo — planos, limites e assinatura com Stripe",
  descricao: "Planos recorrentes, checkout de assinatura, webhooks atualizando o plano, portal do cliente, limites por plano na API e painel admin.",
  planejada: "13/11/2026",
  realizada: "22/10/2026 13:51",
  situacao: "entregue",
  evidencia: "GitHub — commit b939889 (22/10/2026 13:51) + prints do bloqueio HTTP 403 por limite Free e do Painel Admin de MRR",
  url: "https://github.com/Gabriel-Assis-22/Atividade_03_Claude/commit/b939889af679961bbae8cd0bad9cd2b133e84a74",
)[
  *O que foi feito.* Concluiu-se a camada comercial SaaS do UniRide no commit `b939889` (`22/10/2026 13:51`, antecedendo o prazo de `13/11/2026`). Evoluiu-se a cobrança para assinatura mensal recorrente (`Mode = "subscription"`, R\$ 14,90/mês) em `AssinaturasController` e `StripePaymentService`, adicionou-se a geração de sessões do *Stripe Customer Portal* (`POST /api/assinaturas/portal`) para autogestão e cancelamento, e ampliou-se o `WebhooksStripeController` / `HandleStripeWebhookUseCase` para processar `checkout.session.completed`, `customer.subscription.created`, `customer.subscription.updated` e `customer.subscription.deleted` de forma idempotente. Na API de reservas (`ReservaUseCases`), implementou-se o *enforcement* rígido de cota por plano: passageiros no plano gratuito (`is_premium = 0`) são bloqueados na 3ª reserva ativa simultânea com `HTTP 403 Forbidden` (`codigo = "LIMITE_PLANO_FREE"`), gerando banner de conversão imediata na UI e evento `bloqueio_limite_plano` no Redis. Por fim, construiu-se o *Painel Administrativo de Métricas SaaS* (`GET /api/assinaturas/metricas`), exibindo total de usuários, contas Free, assinantes Pro e *Monthly Recurring Revenue* (MRR). Em transparência técnica, registra-se que no ambiente local validado as variáveis `STRIPE_SECRET_KEY` e `STRIPE_WEBHOOK_SECRET` estavam preenchidas com *placeholders* de configuração, de modo que o bloqueio 403 de cota, o painel MRR e as rotas de assinatura foram validados em execução real na stack Docker, bastando parametrizar as chaves `sk_test_...` / `whsec_...` no Portainer para redirecionamento externo com o cartão `4242`.

  #evidencia(
    [Atividade 11 — página do commit b939889 no repositório do produto (Gabriel-Assis-22/Atividade_03_Claude)],
    arquivo: "prints/ativ11_commit_github.png",
  )
  #evidencia(
    [Atividade 11 — verificação no terminal Git com repositório, hash completo e data/hora exatas (22/10/2026 13:51 BRT)],
    arquivo: "prints/ativ11_gitlog.svg",
  )
  #evidencia(
    [Atividade 11 — resultado (1/2): API e Feed bloqueando a 3ª reserva simultânea do Passageiro Free com HTTP 403 e CTA de upgrade],
    arquivo: "prints/ativ11_limite_plano_403_ui.png",
  )
  #evidencia(
    [Atividade 11 — resultado (2/2): Painel Administrativo exibindo indicadores de usuários, assinantes ativos, MRR e Portal Stripe],
    arquivo: "prints/ativ11_painel_admin_mrr.png",
  )

  *Dificuldades e como foram resolvidas.* Em eventos subsequentes do ciclo de vida recorrente (`customer.subscription.updated` e `customer.subscription.deleted`), o objeto `Subscription` enviado pelo Stripe não carrega o campo `ClientReferenceId` original da sessão de checkout. Para impedir que renovações ou cancelamentos falhassem em localizar o estudante no `MariaDB`, configurou-se a gravação redundante do `usuarioId` dentro de `SubscriptionData.Metadata` já na abertura do checkout, garantindo vínculo determinístico em todas as transições futuras da assinatura.
]
