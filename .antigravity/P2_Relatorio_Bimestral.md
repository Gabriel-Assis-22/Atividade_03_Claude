# [disciplinas](https://lapps.studio/) / [introdução à computação em nuvem](https://lapps.studio/disciplinas/introducao-computacao-nuvem/) / [atividades](https://lapps.studio/disciplinas/introducao-computacao-nuvem/atividades/) / p2

**ISW055 · P2 · Avaliação bimestral · individual**

# P2 — Relatório consolidado: arquitetura e atividades

A segunda avaliação também não é prova: é um relatório em PDF do 2º bimestre. Ele tem a arquitetura do SaaS que você colocou no ar, o quadro das atividades 7 a 11 com a data planejada e a data realizada de cada uma, e a evidência de cada entrega. O que vale é o que você consegue provar.

---

**ENTREGA:** **sexta-feira, 13/11/2026**  
*avaliação individual · PDF único · mesmo canal da P1*

Na P1 você prestou contas de um bimestre de entregas. Na P2 você presta contas de um produto: o catálogo de filmes virou um SaaS, com negócio, papéis, assinatura e limites. O relatório agora tem duas metades. Uma é a arquitetura do sistema que está no ar. A outra é o quadro das atividades do bimestre, da 7 à 11, com a prova de cada entrega.

> **Entrega individual, sem exceção.** Cada aluno escreve o próprio relatório, sobre o próprio SaaS e as próprias entregas.

---

## Conceito
### Por que a arquitetura entra no relatório

Na P1, o relatório provava que cada peça foi entregue. Agora as peças formam um sistema, e um sistema se explica pela arquitetura: quais componentes existem, onde rodam, como uma requisição atravessa todos eles e como o dinheiro entra pela assinatura.

É o documento que um colega precisaria pra dar manutenção no seu SaaS sem você do lado. É também a documentação reversa da atividade 8, agora feita sobre o produto final. Por isso ela descreve o que **está no ar**, não o que você gostaria de ter feito.

---

## Estrutura
### O que o PDF precisa conter, nesta ordem

> *[Diagrama da estrutura do relatório]*:
> À esquerda, o esqueleto do relatório em PDF: capa e sumário, introdução, metodologia, arquitetura do SaaS, quadro de entregas das atividades 7 a 11, uma ficha por atividade, considerações finais e declaração de autoria. À direita, os oito subtítulos-guia da seção de arquitetura.
>
> *Legenda*: O template já traz esse esqueleto pronto: os subtítulos da arquitetura vêm com orientação e tabelas pra completar, e as fichas da seção 5 alimentam sozinhas o quadro da seção 4.

- **Capa e sumário**: Fatec, disciplina, turma, seu nome, o nome do produto e o link do repositório. O sumário é gerado pelo template.
- **1 Introdução**: A disciplina, como o catálogo de filmes virou o seu SaaS, a dor que ele resolve e o que o relatório contém.
- **2 Metodologia**: Como as atividades foram feitas, a cópia do repositório na atividade 9 e de onde saíram as datas e horas.
- **3 Arquitetura do SaaS**: Componentes, onde roda, fluxo de uma requisição, fluxo de assinatura, modelo de dados, segurança, observabilidade/CI e camadas.
- **4 Quadro de entregas**: As atividades 7 a 11: nº · atividade · **data planejada** · **data realizada** · situação. Gerado a partir das fichas.
- **5 Atividades realizadas**: Uma ficha por atividade, da 7 à 11: descrição, datas, situação, evidência, link, o que foi feito, prints, dificuldades.
- **6 Considerações finais**: O que aprendeu no bimestre e o que ainda falta pro seu SaaS ser um produto de verdade.
- **Declaração de autoria**: Que o relatório é seu, individual, e que as evidências são verificáveis nos links.

---

## Arquitetura
### O que a seção 3 precisa mostrar

Diagramas podem ser feitos em qualquer ferramenta (draw.io, Excalidraw, Mermaid…) e entrar no PDF como imagem.

- **3.1 Componentes e microsserviços**: Um diagrama com o app do negócio, o auth-service (login + RBAC), o log-service com Redis, o MariaDB, o MinIO e o Stripe com os webhooks: quem chama quem, o que é público (passa pelo domínio) e o que só existe na rede interna do Docker.
- **3.2 Onde roda**: A mesma stack do Portainer, na mesma VM e no mesmo domínio: o repositório de onde ela puxa o código, os containers, os limites de memória e os nomes das variáveis de ambiente, nunca os valores.
- **3.3 Fluxo de uma requisição**: Uma ação comum do seu negócio, do navegador até a resposta: domínio, app, auth-service (sessão e papel), MariaDB, log-service.
- **3.4 Fluxo de assinatura**: Checkout → webhook → plano no banco → limite liberado na API, incluindo o cancelamento pelo portal do cliente, a validação do webhook e o que acontece se o mesmo evento chegar duas vezes.
- **3.5 Modelo de dados do domínio**: As tabelas do negócio, as chaves estrangeiras e as colunas que ligam usuário, plano e assinatura do Stripe.
- **3.6 Segurança**: O que cada papel pode fazer, como o isolamento por usuário é garantido nas consultas e onde ficam os segredos: nas variáveis da stack, fora do repositório.
- **3.7 Observabilidade e CI/CD, se tiver**: Health checks, métricas, pipeline do GitHub Actions, Swagger. Se você não fez as extras, uma linha dizendo isso basta.
- **3.8 Camadas IaaS, PaaS e SaaS do projeto**: Onde cada camada aparece no seu projeto e quem gerencia cada uma. É a atividade 8 aplicada ao produto final.

---

## Escopo
### As 5 atividades do quadro

As atividades obrigatórias do 2º bimestre, da 7 à 11. As datas planejadas são as publicadas na [lista de atividades](https://lapps.studio/disciplinas/introducao-computacao-nuvem/atividades/). A data realizada é você quem informa, com prova.

| Nº | Atividade | Data planejada | Evidência esperada |
| :--- | :--- | :--- | :--- |
| **7** | Plano Premium — cobrança de verdade com Stripe | 09/10/2026 | GitHub — commit, print do checkout concluído e do usuário premium |
| **8** | SaaS, PaaS, IaaS — e a documentação que o projeto nunca teve | 16/10/2026 | GitHub — commit, DOCUMENTACAO.md linkado no README |
| **9** | Fork e plano de negócio | 23/10/2026 | GitHub — issue de entrega, repositório novo, README do produto, print do histórico e das issues |
| **10** | Seu negócio no ar | 06/11/2026 | GitHub — issue de entrega, commit, URL no ar, prints da landing, do 403 e dos eventos no log |
| **11** | SaaS completo — planos, limites e assinatura com Stripe | 13/11/2026 | GitHub — issue de entrega, commit, prints da assinatura com o cartão 4242, do webhook e do limite bloqueando |

As atividades extras (Swagger, CI/CD, observabilidade) não são obrigatórias. Se você fez alguma, ela entra na seção 3.7 da arquitetura.

---

## Evidência
### Como provar a data e a hora de cada entrega

Evidência é **link que abre** + **print com data e hora visíveis**. Um sem o outro não vale.

#### GitHub — Onde está a data e a hora
Abra a página do commit da entrega no GitHub (aba *Commits* → clique no commit): ela mostra data e hora exatas. No terminal: `git log -1 --date=format:"%d/%m/%Y %H:%M" --format="%h %ad"`. O print precisa mostrar o hash, a data/hora e o nome do repositório. Da atividade 9 em diante, a issue de entrega mencionando @siriani também vale: ela mostra a data e a hora em que foi aberta.

#### Dois repositórios — Antigo ou novo, tanto faz
As atividades 7 e 8 estão no repositório antigo e também no novo, porque o histórico foi copiado inteiro na atividade 9. O commit tem o mesmo hash, a mesma data e a mesma hora nos dois. Use o link que preferir.

#### Regra geral — O print precisa ser legível
Tela inteira, sem zoom que corte a data. Se o print não deixa claro de qual entrega se trata, ele não é evidência — é uma imagem.

---

## Template
### Baixe o modelo — já vem com as atividades 7 a 11

O template está em **Typst**, como o da P1. Ele já traz a capa, o sumário, a seção de arquitetura com os subtítulos-guia e as tabelas pra completar, as fichas das atividades com as datas planejadas preenchidas, e o quadro de entregas que se monta sozinho.

- [⬇ P2_ISW055_template.typ](https://lapps.studio/disciplinas/introducao-computacao-nuvem/atividades/p2-relatorio-bimestral/P2_ISW055_template.typ?v=b451c743) *(fonte editável)*
- [⬇ P2_ISW055_template.pdf](https://lapps.studio/disciplinas/introducao-computacao-nuvem/atividades/p2-relatorio-bimestral/P2_ISW055_template.pdf?v=1a982182) *(como fica compilado)*

#### 1. Compilar no navegador — sem instalar nada
Entre em [typst.app](https://typst.app) (conta gratuita), crie um projeto vazio, envie o `.typ` e uma pasta `prints/` com as suas imagens e diagramas. O PDF aparece ao lado enquanto você edita; baixe pelo botão de exportar.

#### 2. Ou no terminal
Com o Typst instalado (o mesmo da P1), rode `typst compile P2_ISW055_template.typ` na pasta onde estão o arquivo e os prints.

#### 3. Prefere Word ou Google Docs?
Pode — desde que o PDF final tenha **exatamente a mesma estrutura** do template (seções, subtítulos da arquitetura, quadro de entregas com as mesmas colunas, fichas, declaração). Use o PDF do template como referência visual.

---

## Requisitos
### O que será conferido

1. **Arquitetura que bate com o sistema no ar**  
   Os oito subtítulos da seção 3 preenchidos com o seu projeto. Componente, fluxo ou tabela que está no relatório tem que existir no sistema que está rodando no seu subdomínio.

2. **Quadro de entregas completo**  
   As atividades 7 a 11 na tabela, com data planejada, data realizada (dia **e hora**) e situação — inclusive as não entregues, marcadas como tal.

3. **Uma ficha por atividade, com evidência que abre**  
   Uma ficha completa para cada atividade de 7 a 11: link direto (não a home do seu perfil), print da entrega com data/hora visíveis e print do resultado. Links que não abrem ou prints sem data invalidam a evidência daquela atividade.

4. **Introdução e metodologia de verdade**  
   Texto seu, no mínimo dois parágrafos cada, sem copiar o enunciado das atividades nem esta página. A metodologia diz de onde saíram as datas e horas.

5. **Organização e legibilidade**  
   Sumário funcionando, seções na ordem, prints e diagramas legíveis com legenda, sem blocos de orientação do template esquecidos no meio do texto.

6. **Honestidade**  
   Atividade não entregue ou entregue com atraso aparece assim no quadro, com uma linha sobre o motivo. Uma ficha honesta de atividade não feita vale mais que uma evidência forjada — e evidência forjada zera a P2.

> **Atrasos.** Cada atividade de 7 a 11 entregue com atraso ou não entregue desconta **0,25** da nota da P2. A própria P2 perde **0,25 por dia** de atraso depois de 13/11/2026, contado pela data do primeiro commit do PDF em `docs/`.

---

## Entrega
### Como entregar

- PDF único, nomeado `P2_ISW055_Nome_Sobrenome.pdf`
- Commitado no **repositório do produto** (o novo, da atividade 9), na pasta `docs/` — a data e a hora do primeiro commit do PDF são a data da entrega
- Link do PDF adicionado ao README, que continua mencionando [github.com/siriani](https://github.com/siriani)
- Uma **issue** aberta no repositório do produto com o título `Entrega — P2`, mencionando `@siriani`, com o link do PDF em `docs/` e do commit
- Entrega individual — um relatório por aluno, sobre o seu próprio repositório
- Prazo: **sexta-feira, 13/11/2026**, o mesmo dia da atividade 11 — comece agora: a arquitetura e as fichas das atividades 7 e 8 já podem ser escritas hoje, e o quadro se completa sozinho a cada ficha preenchida

---
*lapps.studio · ISW055 · P2 publicada 15/10/2026 · [GitHub](https://github.com/siriani)*

