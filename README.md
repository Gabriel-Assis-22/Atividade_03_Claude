# UniRide — Carona Universitária Compartilhada (SaaS em Microsserviços)

> **ISW055 · Introdução à Computação em Nuvem · Fatec Pompeia · 2026.2**  
> **Professor:** [@siriani](https://github.com/siriani) (Prof. Allan Lincoln Rodrigues Siriani)  
> **Aluno:** Gabriel Roberto de Assis  
> 📄 **Relatório Consolidado Bimestral (P2):** [`docs/P2_ISW055_Gabriel_Assis.pdf`](docs/P2_ISW055_Gabriel_Assis.pdf)  
> 📄 **Relatório Bimestral do 1º Bimestre (P1):** [`docs/P1_ISW055_Gabriel_Assis.pdf`](docs/P1_ISW055_Gabriel_Assis.pdf)  
> 📚 **Plano de Negócio & Lean Canvas:** [`README_UNIRIDE.md`](README_UNIRIDE.md) · **Backlog de Issues:** [`BACKLOG_UNIRIDE.md`](BACKLOG_UNIRIDE.md)  
> 📘 **Documentação Reversa (Atividade 8):** [`DOCUMENTACAO.md`](DOCUMENTACAO.md) · **OpenAPI 3.0:** [`docs/openapi.json`](docs/openapi.json)  
> 🔗 **Repositório Base (1º Bimestre + Atividades 7 e 8):** [github.com/Gabriel-Assis-22/Atividade_02_Claude](https://github.com/Gabriel-Assis-22/Atividade_02_Claude)

---

## 1. Sobre o Produto SaaS (UniRide)

O **UniRide** é uma plataforma *Software as a Service* (SaaS) de caronas universitárias diárias voltada a estudantes e condutores acadêmicos que viajam entre municípios da região centro-oeste paulista (Marília, Bauru, Garça, Tupã e Pompeia).

- **A Dor:** Estudantes sem veículo próprio gastam mais de R$ 400/mês com linhas intermunicipais rígidas e perdem até 3 horas por noite em deslocamento, enquanto dezenas de colegas condutores percorrem as mesmas rodovias com vagas ociosas e alto custo individual de combustível, recorrendo hoje a grupos informais de mensagens sem garantia de vaga, histórico ou verificação de perfil.
- **Proposta de Valor:** Viagens diárias compartilhadas entre cidades universitárias com economia de até 60%, confirmação atômica de assento em dois cliques e isolamento seguro por usuário.

### Planos e Modelo de Receita Recorrente (Stripe)

| Plano | Valor | Recursos e Regras de Cota na API |
| :--- | :---: | :--- |
| **Passageiro Free** | **R$ 0,00/mês** | Busca completa no feed de rotas e até **2 reservas ativas simultâneas** (bloqueio automático `HTTP 403 Forbidden` — `LIMITE_PLANO_FREE` na 3ª reserva). |
| **UniRide Pro** | **R$ 14,90/mês** | Checkout recorrente (`mode=subscription`) via **Stripe**, **reservas ilimitadas**, autoatendimento e cancelamento pelo **Stripe Customer Portal** e destaque para condutores. |

---

## 2. Arquitetura de Microsserviços

O ecossistema é composto por containers independentes conectados pela rede privada `app-network` (driver `bridge`). Apenas o container `frontend` (Nginx) publica porta externa (`:8208`), atuando como servidor da SPA e *Reverse Proxy / API Gateway*:

```text
                                  REDE EXTERNA / DOMÍNIO PÚBLICO
                                                │
                                                ▼ Porta 8208
                                      ┌───────────────────┐
                                      │     frontend      │
                                      │ (Angular + Nginx) │
                                      └─────────┬─────────┘
                                                │
                        REDE INTERNA DOCKER (app-network) — APENAS EXPOSE
                        ─────────────────────────────────────────────────
                           ├── /api/* e /swagger/* ──▶ backend (:8080 · .NET 10 DDD)
                           │                             ├──▶ auth-service (:8081 · JWT & RBAC)
                           │                             ├──▶ log-service (:8082) ──▶ redis (:6379 Streams)
                           │                             ├──▶ garage (:3900 · Object Storage S3 SigV4)
                           │                             ├──▶ MariaDB (:3306 · Dapper + FluentMigrator)
                           │                             └──▶ Stripe API / Webhooks (Stripe-Signature)
                           └── /storage/* ───────────▶ Proxy autenticado de mídia S3 no backend
```

| Serviço | Tecnologia / Imagem | Porta | Responsabilidade Principal |
| :--- | :--- | :---: | :--- |
| **`frontend`** | Angular 19 + `nginx:alpine` | `8208:80` (Pública) | Landing Page UniRide, Feed de Caronas, Painel Motorista/Admin e Proxy Reverso (`/api/`, `/swagger/`, `/storage/`). |
| **`backend`** | ASP.NET Core 10 C# (DDD) | `expose 8080` | Veículos, Caronas, Reservas atômicas, Avaliações, cotas por plano, Stripe Subscriptions/Portal/Webhooks e Métricas MRR. |
| **`auth-service`** | ASP.NET Core 10 C# (DDD) | `expose 8081` | Registro, login BCrypt + JWT HMAC-SHA256, 3 papéis RBAC (`passageiro`, `motorista`, `admin`) e reset de senha (TTL 30 min via Mailtrap). |
| **`log-service`** | ASP.NET Core 10 Web API | `expose 8082` | Centralização de auditoria e eventos de negócio (`carona_criada`, `reserva_solicitada`, `bloqueio_limite_plano`, `bloqueio_seguranca`). |
| **`redis`** | `redis:7-alpine` | `expose 6379` | Armazenamento *append-only* ordenado no tempo via **Redis Streams** (`XADD` / `XREVRANGE` em `audit_stream`). |
| **`garage` + `garage-init`** | `dxflrs/garage:v1.0.1` (Rust) | `expose 3900` | Object Storage compatível com AWS S3 para fotos de veículos e avatares de perfil (referenciados apenas por chave/URL no MariaDB). |
| **`MariaDB`** | MariaDB 10 (Infraestrutura) | `3306` | Tabelas `usuarios`, `veiculos`, `caronas`, `reservas`, `avaliacoes` e `reset_tokens`. |

---

## 3. Segurança, RBAC (3 Papéis) e Proteção Anti-IDOR

| Ação no Sistema | `admin` | `motorista` | `passageiro` |
| :--- | :---: | :---: | :---: |
| Buscar e visualizar caronas disponíveis (`GET /api/caronas`) | ✅ | ✅ | ✅ |
| Reservar vaga em carona (`POST /api/caronas/{id}/reservar`) | ✅ | ✅ | ✅ *(máx. 2 no Free)* |
| Cadastrar veículo e foto S3 (`POST /api/veiculos`) | ✅ | ✅ | ❌ (`403`) |
| Publicar nova carona (`POST /api/caronas`) | ✅ | ✅ | ❌ (`403`) |
| Alterar/cancelar recurso de outro usuário (Anti-IDOR) | Moderação | ❌ (`403`) | ❌ (`403`) |
| Consultar métricas de negócio e MRR (`GET /api/assinaturas/metricas`) | ✅ | ❌ (`403`) | ❌ (`403`) |
| Auditar logs do Redis Streams (`GET /api/logs`) | ✅ | ❌ (`403`) | ❌ (`403`) |

- **Gestão de Segredos:** Nenhuma chave privada é versionada no Git. Todas as credenciais são injetadas via variáveis de ambiente (`.env` ignorado no Git / variáveis da stack no Portainer), mantendo apenas os nomes documentados em [`.env.example`](.env.example).

---

## 4. Camadas de Computação em Nuvem (IaaS, PaaS e SaaS)

| Camada | O que é no UniRide | Quem Gerencia |
| :---: | :--- | :--- |
| **IaaS** | Máquina virtual Linux no cluster da disciplina (vCPU, RAM, disco virtual e roteamento `lapps.studio`). | Infraestrutura Fatec / Prof. [@siriani](https://github.com/siriani) |
| **PaaS** | Docker Engine + Portainer, MariaDB compartilhado e containers de plataforma (`redis:7-alpine`, `garage:v1.0.1`, `.NET 10`, `nginx:alpine`). | Plataforma base pela Fatec/Portainer; `Dockerfile`s, `docker-compose.yml` e migrações pelo aluno |
| **SaaS** | **UniRide** (produto final entregue ao estudante via navegador) e SaaS externos consumidos via API (**Stripe**, **Mailtrap**, **GitHub**, **TMDB**). | UniRide pelo aluno; serviços externos pelos respectivos provedores |

---

## 5. Como Executar Localmente

1. Copie `.env.example` para `.env` e preencha as variáveis do ambiente local:
   ```bash
   cp .env.example .env
   ```
2. Construa e inicie todos os microsserviços com Docker Compose:
   ```bash
   docker compose up --build
   ```
3. Acesse no navegador:
   - **Aplicação UniRide (Landing Page / SPA):** `http://localhost:8208`
   - **Documentação Interativa Swagger UI:** `http://localhost:8208/swagger/`
