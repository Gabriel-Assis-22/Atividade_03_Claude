**Fonte:** https://lapps.studio/disciplinas/introducao-computacao-nuvem/atividades/dominio-no-ar/

Atividade: Seu negócio no ar — Allan Siriani

        github.com/siriani
      ]
    
  

  
    

[disciplinas](/) / [introdução à computação em nuvem](/disciplinas/introducao-computacao-nuvem/) / [atividades](/disciplinas/introducao-computacao-nuvem/atividades/) / seu negócio no ar
    

ISW055 · Atividade 10 · Domínio do negócio
    

# Seu negócio no ar — trocando os filmes pelo seu domínio
    

O plano vira sistema. O catálogo do Tom Hanks sai e o recurso principal do seu negócio entra, com tabelas, API, telas, papéis e eventos próprios. Tudo na mesma stack e no mesmo endereço de sempre.
  

  
    
      Entrega sexta-feira, 06/11/2026
      duas semanas · a sexta 30/10 é feriado
    
    
      

Na atividade 9 você escreveu o que o produto faz, pra quem e quanto custa. Agora ele precisa existir: quem abrir o seu endereço tem que ver o seu negócio, não um catálogo de filmes. Você não começa do zero. O login, os papéis, o log, o upload e o Stripe já estão prontos e só mudam de assunto. É o mapa de reaproveitamento saindo do README e virando código.
    
    
      **Mesma infra e mesmo domínio.** A stack do Portainer continua a mesma e só passa a usar o código do repositório novo. Não crie stack nova, não peça porta nova e não troque de subdomínio: mesmas variáveis de ambiente, mesmo endereço, mesmos limites (1 GB de RAM por container).
    
  

  
    
      

Diagrama
      

## O que muda e o que fica
      

Muda o código e o assunto. A infraestrutura, o endereço e os serviços continuam onde estavam.
    
    
      
        
          
          repositório antigo
          fica como está (P1)

          
          repositório novo
          NOME-DO-PRODUTO

          
          pull +
          redeploy

          
          mesma stack no Portainer (VM)

          
          app do negócio
          telas, API, landing: muda tudo

          
          auth-service
          3+ papéis do negócio

          
          log-service + Redis
          eventos de negócio

          
          MariaDB
          tabelas do domínio

          
          MinIO
          arquivos do domínio

          
          variáveis de ambiente
          as mesmas de antes

          mesmos containers, mesmos limites de memória

          
          usuários
          os papéis do negócio
          

          
          mesmo subdomínio
          o endereço de sempre
          

          
            
            
          
        
      
      O repositório antigo não alimenta mais nada. A stack puxa o código do repositório novo e continua atendendo no mesmo subdomínio, com os mesmos serviços de apoio.
    
  

  
    
      

Conceito
      

## Cada cliente no seu quadrado
    
    
      

Um SaaS atende muita gente no mesmo sistema, no mesmo banco e nos mesmos containers. Isso se chama *multi-tenancy*, e a regra que o sustenta é simples: **cada usuário só enxerga os próprios dados**. O motorista vê as caronas dele, não as dos outros. A barbearia vê a agenda dela, não a da concorrente.
      

Esse isolamento não pode depender da tela. Esconder o botão não protege nada se a API devolve o registro de outra pessoa quando alguém troca o ID na URL. Toda consulta filtra pelo dono, e toda ação confere o papel no backend. É o mesmo RBAC da atividade 4, agora aplicado a um negócio de verdade.
    
  

  
    
      

Requisitos
      

## O que precisa funcionar
    
    
      
        1
        
          

#### A stack aponta pro repositório novo
          

Na stack que já existe no Portainer, troque o repositório pelo novo e faça o pull + redeploy. Mesma stack, mesmas variáveis de ambiente, mesmo subdomínio. No fim, todos os containers rodando e saudáveis.
        
      
      
        2
        
          

#### Modelagem no MariaDB
          

Pelo menos 3 tabelas novas do domínio, ligadas por chaves estrangeiras, criadas por um script de migração e de seed versionado no repositório (ex.: `db/001_dominio.sql`). Quem clonar o repositório consegue recriar o banco.
        
      
      
        3
        
          

#### API própria com CRUD
          

Listar, criar, detalhar, editar e remover o recurso principal do negócio. Se você fez a extra do Swagger, a documentação acompanha as rotas novas.
        
      
      
        4
        
          

#### Telas do recurso principal
          

Listar, criar, detalhar e editar, com os textos e os nomes do seu negócio.
        
      
      
        5
        
          

#### Papéis do negócio
          

Pelo menos 3 papéis no auth-service com nomes do negócio (ex.: admin, motorista, passageiro). As permissões valem na API: ação proibida responde 403 mesmo quando a chamada é feita direto, sem passar pela tela.
        
      
      
        6
        
          

#### Isolamento por usuário
          

Cada usuário (ou organização, se o seu negócio tiver equipes) só vê e altera os próprios dados. Abrir o ID de um registro de outra pessoa dá 403 ou 404.
        
      
      
        7
        
          

#### Eventos de negócio no log
          

O log-service registra o que importa pro negócio, com quem e quando: "carona criada", "vaga reservada", "carona cancelada". Login e erro continuam valendo.
        
      
      
        8
        
          

#### MinIO ligado ao domínio
          

Um upload que faz sentido pro negócio, guardado no MinIO e ligado ao registro certo: a foto do carro, a imagem do produto, o comprovante.
        
      
      
        9
        
          

#### Landing page pública
          

Uma página aberta, sem login, com a proposta de valor do seu plano de negócio e um botão pra criar conta.
        
      
      
        10
        
          

#### Nada de Tom Hanks
          

Saem os filmes do código atual: nomes, textos, rotas, tabelas que não servem mais e a chave do TMDB. Se não usa mais, remova a variável da stack também. O histórico continua no git, e é pra isso que ele serve.
        
      
    
  

  
    
      

Cenário de uso
      

## É assim que eu vou testar
    
    
      

Abro o seu subdomínio de sempre sem login e vejo a landing page com a proposta de valor.
      

Crio duas contas com papéis diferentes do negócio, por exemplo um motorista e um passageiro.
      

Com a primeira conta, crio, edito e apago registros do recurso principal, inclusive com upload.
      

Com a segunda conta, troco o ID na URL (ou chamo a API direto) pra abrir um registro da primeira e recebo 403 ou 404.
      

Tento, direto na API, uma ação que o meu papel não permite e recebo 403, mesmo sem o botão na tela.
      

Entro como admin e vejo no log os eventos de negócio dessas ações, com quem e quando.
      

Procuro "Tom Hanks", "filme" e "TMDB" nas telas e no código atual e não encontro nada.
    
  

  
    
      

Entrega
      

## Mesmo repositório da atividade 9
    
    

      
- Continue no **repositório novo, público**, o mesmo da atividade 9
      
- Continue mencionando o professor no README: [github.com/siriani](https://github.com/siriani)
      
- Abra uma **issue** no repositório do produto com o título `Entrega — Seu negócio no ar`, mencionando `@siriani`, com o link do commit, a URL do sistema no ar e os prints. A data e a hora da issue entram como evidência da data realizada
      
- No README: a URL do sistema no ar (o mesmo subdomínio de sempre) e o diagrama de arquitetura atualizado, com os serviços e como eles conversam
      
- O script de migração e seed do banco versionado no repositório
      
- Prints: a landing page, a listagem do recurso principal, o 403 de um papel sem permissão, o bloqueio ao abrir o registro de outro usuário, os eventos de negócio no log, o arquivo no MinIO e a stack no Portainer com os containers saudáveis
    

  

  
    
      lapps.studio · ISW055 · atividade publicada 15/10/2026
      
        [GitHub](https://github.com/siriani)
