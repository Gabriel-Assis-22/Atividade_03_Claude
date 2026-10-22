# UniRide — Carona Universitária Compartilhada

> ISW055 · Arquitetura de Microsserviços e Aplicações em Cloud · Professor [@siriani](https://github.com/siriani)  
> Aluno: Gabriel Assis  
> Repositório do Produto: [github.com/Gabriel-Assis-22/UniRide_SaaS](https://github.com/Gabriel-Assis-22/UniRide_SaaS)  
> Projeto Original (P1): [github.com/Gabriel-Assis-22/Atividade_02_Claude](https://github.com/Gabriel-Assis-22/Atividade_02_Claude)

---

## 1. Nome e Frase de Impacto
**UniRide**: Plataforma inteligente de caronas compartilhadas que conecta motoristas e passageiros universitários para viagens diárias econômicas, seguras e validadas no campus.

---

## 2. A Dor
Estudantes universitários da região (ex.: Marília, Bauru, Garça, Pompeia) enfrentam gastos superiores a **R$ 400 por mês** em passagens de ônibus intermunicipais e perdem de **2 a 3 horas diárias** em pontos de embarque noturnos ou itinerários circulares demorados. Ao mesmo tempo, dezenas de alunos condutores realizam o mesmo trajeto todos os dias com assentos vazios, gastando até **R$ 600 mensais com combustível** sozinhos. 

Hoje, essa necessidade tenta ser resolvida de maneira caótica e insegura em grupos informais de WhatsApp/Telegram (mensagens perdidas, falta de confirmação e zero checagem de perfil) ou em aplicativos como BlaBlaCar (foco rodoviário/interestadual, sem validação da comunidade acadêmica).

---

## 3. Persona
* **Nome:** Lucas Silveira, 21 anos.
* **Contexto:** Estudante do 4º semestre de Desenvolvimento de Software Multiplataforma (DSM) na Fatec. Reside em Marília e viaja diariamente até o campus (45 km ida e volta).
* **Rotina e Obstáculos:** Possui aulas das 19h00 às 22h30. Depende do ônibus intermunicipal das 23h05, chegando em casa quase à meia-noite. Tenta encontrar caronas em grupos de conversa, mas frequentemente fica na mão por falta de aviso dos motoristas ou por mensagens soterradas no fluxo do chat. Gostaria de reservar assentos com antecedência, ter previsibilidade de horário e ratear um valor justo diretamente pelo celular.

---

## 4. Concorrentes e Alternativas Atuais

| Alternativa / Concorrente | O que faz | O que faz mal / Ponto fraco |
| :--- | :--- | :--- |
| **Grupos de WhatsApp/Telegram** | Conversas entre alunos da faculdade para combinar caronas pontuais. | Desorganizado, mensagens se perdem rapidamente, não garante assento, sem histórico nem validação de identidade institucional. |
| **Ônibus Intermunicipal Rodoviário** | Linhas regulares de transporte coletivo intermunicipal. | Muito caro (passagens diárias acumuladas), itinerários lentos com múltiplas paradas, horários fixos e inflexíveis. |
| **BlaBlaCar** | Plataforma consolidada de caronas intermunicipais/interestaduais. | Focado em viagens de longa distância/fins de semana; não atende a rotina diária de estudantes nem possui validação do campus. |

---

## 5. Proposta de Valor
> *Viagens diárias entre cidades universitárias com custo até 60% menor que o ônibus, embarque garantido e a segurança de dividir o trajeto exclusivamente com membros validados do campus.*

---

## 6. Modelo de Receita com Stripe (Planos e Cobrança)
A monetização opera em modelo de assinatura recorrente com provedor **Stripe (modo teste)**, garantindo conformidade PCI-DSS sem retenção de cartões no banco local:

* **Plano Gratuito (Passageiro Free — R$ 0,00):**
  - Busca e visualização de todas as caronas disponíveis.
  - Limite de até **2 reservas ativas simultâneas**.
  - Histórico básico de viagens.
* **Plano UniRide Pro (R$ 14,90/mês via Stripe):**
  - Reservas ilimitadas de caronas no mês.
  - Notificações prioritárias de novas rotas no horário de interesse.
  - Para condutores: publicação ilimitada de rotas, selo de **Motorista Verificado** e destaque visual no feed de buscas.

---

## 7. Métricas de Sucesso (KPIs)
1. **MRR (Monthly Recurring Revenue):** Receita recorrente mensal gerada pelas assinaturas do Plano Pro.
2. **Total de Assinantes Ativos:** Quantidade de condutores e passageiros frequentes na base Pro.
3. **Taxa de Ocupação de Assentos:** Média de assentos reservados por viagem cadastrada (meta: > 75%).
4. **Volume de Viagens Concluídas:** Total de trajetos realizados mensalmente na plataforma.
5. **Taxa de Cancelamento (Churn Rate):** Retenção de assinantes mês a mês (meta: churn < 5%).

---

## 8. Lean Canvas do Produto (9 Blocos)

| Bloco | Detalhamento para o UniRide |
| :--- | :--- |
| **1. Problema** | • Custo de transporte elevado (> R$ 400/mês) para alunos da região.<br>• Perda excessiva de tempo com horários rígidos de ônibus rodoviários.<br>• Insegurança e desorganização dos grupos informais de WhatsApp. |
| **2. Segmentos de Clientes** | • Estudantes universitários sem veículo próprio (passageiros).<br>• Alunos e professores condutores que viajam sozinhos de carro todos os dias.<br>• *Early Adopters:* Alunos de DSM/ADS que moram nas cidades vizinhas mais distantes. |
| **3. Proposta Única de Valor** | Viagens universitárias diárias seguras, previsíveis e com divisão justa de combustível, reservadas em 2 cliques. |
| **4. Solução** | 1. Feed e busca de caronas com filtros de cidade, data, horário e vagas.<br>2. Sistema de reserva garantida de assentos com isolamento de dados.<br>3. Verificação de vínculo com upload de documento/veículo e assinaturas Stripe. |
| **5. Canais** | • Divulgação direta nas salas de aula e murais do campus.<br>• Parceria com Centros Acadêmicos e Atléticas da Fatec.<br>• Grupos de turmas e redes sociais universitárias. |
| **6. Fontes de Receita** | • Assinatura mensal do **Plano UniRide Pro (R$ 14,90/mês)** processada via Stripe.<br>• Futura taxa de conveniência sobre divisões financeiras. |
| **7. Estrutura de Custos** | • Infraestrutura de nuvem (VM Proxmox Fatec / IaaS).<br>• Taxa de processamento por transação do Stripe (modo teste).<br>• Custos de manutenção e desenvolvimento do software. |
| **8. Métricas-Chave** | • Usuários cadastrados ativos.<br>• Assinantes Pro e MRR consolidado.<br>• Caronas ofertadas vs. assentos preenchidos.<br>• Churn mensal de assinaturas. |
| **9. Vantagem Injusta** | Validação focada na comunidade acadêmica e presença física direta no ecossistema universitário, criando efeito de rede local difícil de ser replicado por grandes apps generalistas. |

---

## 9. Mapa de Reaproveitamento da Arquitetura Atual

Toda a infraestrutura e os microsserviços desenvolvidos na P1 são 100% reaproveitados para suportar o novo negócio:

| Peça do Projeto Atual (P1) | O que vira no SaaS (UniRide) | Justificativa Técnica |
| :--- | :--- | :--- |
| **Catálogo de Filmes (TMDB API + MariaDB)** | **Feed de Caronas Disponíveis** | Substitui a busca de filmes por listagem de viagens com filtros de cidade de origem, destino, horário e assentos livres. |
| **Favoritos & Comentários** | **Caronas Salvas & Avaliações de Condutores** | Persistência relacional reaproveitada para salvar rotas frequentes e emitir feedbacks/notas mútuas pós-viagem. |
| **Microsserviço `auth-service` (JWT + BCrypt)** | **Gestão de Identidade com 3 Papéis: `passageiro`, `motorista` e `admin`** | Extensão das claims do JWT para segregar motoristas (que publicam viagens), passageiros (que reservam) e admins (que moderam). |
| **`log-service` (Redis Streams)** | **Trilha de Eventos e Auditoria de Operações** | Registro em stream append-only de eventos do domínio: `carona_criada`, `reserva_solicitada`, `reserva_cancelada` e bloqueios de segurança. |
| **Garage S3 Storage (`/storage/`)** | **Upload de Fotos de Veículos e Documentos** | Armazenamento de imagens de perfil, fotos de veículos cadastrados e comprovantes via chamadas AWS SigV4. |
| **Integração Stripe (Checkout + Webhooks)** | **Assinatura Recorrente UniRide Pro** | Checkout para cobrança recorrente mensal de condutores e passageiros frequentes com validação de webhook assinado. |
| **Swagger, CI/CD e Observabilidade** | **Governança, Contratos e Deploy da Nova API** | Documentação OpenAPI dos novos endpoints de mobilidade, pipeline de build automatizado e health checks dos serviços. |
