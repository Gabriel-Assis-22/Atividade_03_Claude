**Fonte:** https://lapps.studio/disciplinas/introducao-computacao-nuvem/atividades/fork-plano-de-negocio/

Atividade: Fork e plano de negócio — Allan Siriani

        github.com/siriani
      ]
    
  

  
    

[disciplinas](/) / [introdução à computação em nuvem](/disciplinas/introducao-computacao-nuvem/) / [atividades](/disciplinas/introducao-computacao-nuvem/atividades/) / fork e plano de negócio
    

ISW055 · Atividade 9 · Novo projeto
    

# Fork e plano de negócio — qual dor você vai resolver?
    

Você tem uma arquitetura completa: login, papéis, logs, upload e pagamento. Agora ela deixa de servir a um catálogo de filmes do Tom Hanks e passa a servir a um negócio seu. A primeira entrega do novo projeto é copiar o repositório com todo o histórico e escrever o plano de negócio.
  

  
    
      Entrega sexta-feira, 23/10/2026
      1ª entrega do SaaS · evolui semana a semana até 13/11
    
    
      

Agora que você tem uma arquitetura completa, vamos fazer um fork do seu projeto e começar o bimestre. A ideia é que você reaproveite toda a arquitetura e troque o sistema: em vez de um sistema de filmes do Tom Hanks, pense em uma **dor** ou oportunidade de negócio e crie um **SaaS completo**, com todos os componentes e microsserviços desenvolvidos até agora. Ele evolui semana a semana: nesta atividade, o fork e o plano; na atividade 10, o negócio no ar; na 11, a assinatura com Stripe.
      

Esta primeira entrega quase não tem código. Ela responde à pergunta que vem antes de qualquer linha: **qual dor você vai resolver, pra quem, e por que alguém pagaria por isso?**
    
    
      **Mesma infra e mesmo domínio: não muda nada.** O SaaS novo roda no mesmo lugar do projeto atual: a mesma stack no Portainer, os mesmos serviços (MariaDB, Redis, MinIO, auth, logs, Stripe) e o mesmo subdomínio que você já usa. Muda o negócio, não a infraestrutura. Não crie stack nova nem peça endereço novo. A troca da stack para o repositório novo é a atividade 10.
    
  

  
    
      

Conceito
      

## Por que começar pelo negócio e não pelo código
    
    
      

A maioria dos produtos de software que fracassam não falha por causa da tecnologia: falha porque resolve um problema que ninguém tem, ou que ninguém aceita pagar pra resolver. Um SaaS (software como serviço) vive de assinatura, então a pergunta "por que alguém pagaria todo mês?" precisa de resposta antes do primeiro commit.
      

Por isso o plano de negócio vem antes do código, e é **específico**. "Ajudar estudantes" não é uma dor. "O aluno da Fatec que mora em Marília gasta R$ 400 por mês de ônibus e perde 2 horas por dia porque não sabe quem da turma vai de carro" é uma dor: tem gente, momento, custo e uma solução atual ruim. Quanto mais específica, mais fácil decidir o que o sistema faz e o que ele *não* faz.
    
  

  
    
      

Fork
      

## Um repositório novo, com todo o histórico
      

O botão *Fork* do GitHub foi feito pra copiar o repositório de **outra** pessoa: ele não cria uma cópia de um repositório na mesma conta pessoal que já é dona dele. Então o fork aqui é uma cópia espelhada, com todos os commits, num repositório novo com o nome do produto.
    
    

```
# 1. no GitHub: crie um repositório NOVO, público e VAZIO (sem README,
#    sem .gitignore, sem licença), com o nome do produto

# 2. no terminal: espelhe o repositório atual, com todo o histórico
git clone --bare https://github.com/SEU-USUARIO/REPOSITORIO-ATUAL.git
cd REPOSITORIO-ATUAL.git
git push --mirror https://github.com/SEU-USUARIO/NOME-DO-PRODUTO.git
cd ..
rm -rf REPOSITORIO-ATUAL.git

# 3. daqui pra frente, trabalhe só no repositório novo
git clone https://github.com/SEU-USUARIO/NOME-DO-PRODUTO.git
```

    

É o procedimento da documentação oficial: [Duplicating a repository](https://docs.github.com/en/repositories/creating-and-managing-repositories/duplicating-a-repository). O repositório antigo fica como está, porque ele é a evidência da P1. A única mudança nele é uma linha no topo do README apontando pro novo: `Este projeto continua em github.com/SEU-USUARIO/NOME-DO-PRODUTO`. Confira na aba *Commits* do repositório novo: os commits de agosto em diante têm que estar lá.
  

  
    
      

README
      

## O README do produto, nesta ordem
      

O README do repositório novo substitui o README dos filmes. É ele que eu leio pra corrigir esta atividade.
    
    
      **1 Nome e frase**O nome do produto e uma frase de uma linha que diz o que ele faz e pra quem.
      **2 A dor**Quem sofre, quando, quanto custa hoje (dinheiro, tempo, risco) e como essa pessoa resolve hoje.
      **3 Persona**Uma pessoa concreta que sente a dor: nome fictício, idade, rotina, o que ela tenta fazer e o que atrapalha.
      **4 Concorrentes e alternativas**Pelo menos 3: apps, planilhas, grupos de WhatsApp, o "jeito de sempre". O que cada um faz mal.
      **5 Proposta de valor**Por que o seu produto é melhor que as alternativas para essa persona, numa frase.
      **6 Modelo de receita com Stripe**Os planos (ex.: Free × Pro), o preço mensal em R$, o que o Free limita e por que alguém pagaria o Pro.
      **7 Métricas de sucesso**Como você vai saber se está dando certo: assinantes, MRR (receita recorrente mensal), retenção.
      **8 Lean Canvas**Uma tabela com os 9 blocos, preenchida para o seu produto (veja abaixo).
    
  

  
    
      

Lean Canvas
      

## Os 9 blocos, numa tabela
      

O Lean Canvas é a versão do *Business Model Canvas* que Ash Maurya adaptou pra startups: cabe numa página e obriga a escrever o essencial. Referência: [Lean canvas (Wikipedia, em inglês)](https://en.wikipedia.org/wiki/Lean_canvas).
    
    
      

        
          
| Bloco | Pergunta que ele responde |
        
        
          
| Problema | Quais são as 1 a 3 dores principais? Como elas são resolvidas hoje? |
          
| Segmentos de clientes | Quem sente a dor? Quem são os primeiros que adotariam o produto? |
          
| Proposta única de valor | Numa frase, por que escolher o seu produto e não as alternativas? |
          
| Solução | As 3 funcionalidades que atacam cada dor, nem uma a mais. |
          
| Canais | Como o cliente fica sabendo do produto e chega até ele? |
          
| Fontes de receita | Quem paga, quanto e como: os planos do Stripe. |
          
| Estrutura de custos | O que custa pra manter no ar: infraestrutura, taxas do Stripe, tempo. |
          
| Métricas-chave | Os números que você acompanha: cadastros, assinantes, MRR, retenção. |
          
| Vantagem injusta | O que não dá pra copiar fácil: acesso a um público, conhecimento, dados. |
        
      
    
  

  
    
      

Reaproveitamento
      

## O que cada peça atual vira no SaaS
      

Nada da arquitetura vai pro lixo: cada serviço que você construiu ganha um papel no negócio novo. Coloque no README uma tabela como esta, preenchida para o seu produto. O exemplo usa o tema da carona universitária.
    
    
      

        
          
| Peça do projeto atual | O que vira no SaaS | Exemplo: carona universitária |
        
        
          
| Catálogo de filmes (TMDB) | Recurso principal do negócio | Caronas: origem, destino, horário, vagas |
          
| Microsserviço de login + RBAC | Papéis do negócio | admin, motorista, passageiro |
          
| Logs e auditoria (Redis) | Eventos de negócio | "carona criada", "vaga reservada", "carona cancelada" |
          
| Upload e perfil (MinIO) | Arquivos do domínio | Foto do carro, foto de perfil do motorista |
          
| Plano Premium (Stripe) | Assinatura | Motorista Pro: caronas ilimitadas por mês |
          
| Swagger, CI/CD, observabilidade | Continuam valendo, pra quem fez as extras | API de caronas documentada, deploy a cada push |
        
      
    
  

  
    
      

Temas sugeridos
      

## Doze dores, todas cobráveis com Stripe
      

Escolha uma ou proponha a sua, desde que a dor seja específica e dê pra cobrar por ela com Stripe.
    
    
      
        Tema 1 · mobilidade
        

#### Carona universitária
        

Fatec ↔ cidades da região. Receita: assinatura do motorista ou taxa por viagem.
      
      
        Tema 2 · estudo
        

#### "Tinder" de grupos de estudo
        

Match por disciplina e horário. Receita: plano Pro com grupos ilimitados.
      
      
        Tema 3 · agro
        

#### Cotações do agro
        

Preços de grãos e insumos de cooperativas e cerealistas da região. Receita: alertas pagos e histórico.
      
      
        Tema 4 · agro
        

#### Aluguel de máquinas entre produtores
        

Máquinas e implementos parados que podem render. Receita: taxa por reserva.
      
      
        Tema 5 · agro
        

#### Caderno de campo digital
        

Registro de aplicações e atividades por talhão. Receita: assinatura por propriedade.
      
      
        Tema 6 · estudo
        

#### Monitoria e aulas particulares
        

Entre alunos do próprio campus. Receita: pagamento por sessão.
      
      
        Tema 7 · moradia
        

#### Gestão de república
        

Contas divididas, tarefas da casa, mensalidade. Receita: assinatura da república.
      
      
        Tema 8 · esporte
        

#### Reserva de quadras e espaços
        

Horários livres e reserva sem telefonema. Receita: pagamento da reserva.
      
      
        Tema 9 · pequeno negócio
        

#### Agenda pra barbearia, salão ou pet shop
        

SaaS de pequeno negócio: horários, clientes, lembretes. Receita: assinatura mensal.
      
      
        Tema 10 · carreira
        

#### Bolsa de estágios e freelas
        

Vagas para alunos do campus. Receita: a empresa paga para anunciar.
      
      
        Tema 11 · marketplace
        

#### Livros e materiais usados do campus
        

Compra e venda entre alunos. Receita: anúncio em destaque pago.
      
      
        Tema 12 · eventos
        

#### Ingressos de atléticas e centros acadêmicos
        

Venda e controle de entrada de festas, jogos e eventos. Receita: taxa por ingresso.
      
    
  

  
    
      

Requisitos
      

## O que entregar
    
    
      
        1
        
          

#### Repositório novo, com o histórico
          

Público, com o nome do produto, feito pelo espelhamento acima. Os commits de todas as atividades anteriores aparecem no histórico do repositório novo.
        
      
      
        2
        
          

#### README do produto
          

Os 8 blocos, na ordem: nome e frase, a dor, persona, concorrentes e alternativas, proposta de valor, modelo de receita com Stripe, métricas de sucesso e o Lean Canvas com os 9 blocos.
        
      
      
        3
        
          

#### Mapa de reaproveitamento
          

A tabela "peça atual → o que vira no SaaS", com todas as peças: catálogo, login + RBAC, logs, upload, Plano Premium e as extras que você fez.
        
      
      
        4
        
          

#### Backlog no GitHub
          

Pelo menos 5 [issues](https://docs.github.com/en/issues/tracking-your-work-with-issues/learning-about-issues/quickstart) no repositório novo (ou um [GitHub Project](https://docs.github.com/en/issues/planning-and-tracking-with-projects/learning-about-projects/about-projects)) com as próximas entregas, ex.: "modelar as tabelas de caronas", "criar os papéis motorista e passageiro", "landing page", "plano Pro no Stripe". Elas viram o seu roteiro das atividades 10 e 11.
        
      
    
  

  
    
      

Avaliação
      

## O que eu vou olhar
    
    
      Dor específica e plausível: quem, quando, quanto custa
      Persona concreta, que sente essa dor
      Modelo de receita coerente: planos, preço em R$, limite do Free
      Mapa de reaproveitamento completo, peça por peça
      Pelo menos 5 issues com as próximas entregas
      Repositório novo com o histórico do anterior
    
  

  
    
      

Entrega
      

## Repositório novo, mesmo canal
    
    

      
- Trabalhe no **repositório novo, público**, com o histórico do anterior: daqui até o fim do semestre, tudo acontece nele
      
- Continue mencionando o professor no README: [github.com/siriani](https://github.com/siriani)
      
- Abra uma **issue** no repositório do produto com o título `Entrega — Fork e plano de negócio`, mencionando `@siriani`, com o link do repositório novo e do commit do README do produto, e os prints. A data e a hora da issue entram como evidência da data realizada
      
- No topo do README do repositório antigo, uma linha com o link do repositório novo
      
- Print da aba *Commits* do repositório novo mostrando os commits antigos e o commit do README do produto
      
- Print da lista de issues (ou do GitHub Project) com as próximas entregas
    

  

  
    
      lapps.studio · ISW055 · atividade publicada 15/10/2026
      
        [GitHub](https://github.com/siriani)
