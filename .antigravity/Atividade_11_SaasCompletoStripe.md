**Fonte:** https://lapps.studio/disciplinas/introducao-computacao-nuvem/atividades/saas-completo-stripe/

Atividade: SaaS completo com Stripe — Allan Siriani

        github.com/siriani
      ]
    
  

  
    

[disciplinas](/) / [introdução à computação em nuvem](/disciplinas/introducao-computacao-nuvem/) / [atividades](/disciplinas/introducao-computacao-nuvem/atividades/) / saas completo
    

ISW055 · Atividade 11 · Assinatura recorrente
    

# SaaS completo — planos, limites e assinatura com Stripe
    

O Plano Premium de 09/10 cobrava uma vez e marcava um campo. Um SaaS cobra todo mês, deixa o cliente trocar de plano e cancelar sozinho, e precisa saber, a cada requisição, o que o plano daquele usuário permite.
  

  
    
      Entrega sexta-feira, 13/11/2026
      continuação do Plano Premium · mesmo dia da P2
    
    
      

Você já integrou o Stripe uma vez: checkout, webhook e um campo `premium` no banco. Agora o pagamento vira **assinatura**, e o seu produto vira um SaaS de verdade: tem plano gratuito, plano pago, limite pra quem não paga e um painel que mostra quanto o negócio fatura por mês, ainda que em modo de teste.
    
    
      **Modo de teste, sempre, e nada de chave no repositório.** Planos, checkout, webhooks e portal do cliente rodam no modo de teste do Stripe; cobrança real não existe nesta atividade. A chave secreta e o segredo do webhook ficam nas variáveis de ambiente da stack no Portainer, nunca no código. Dado de cartão continua nunca passando pelo seu servidor.
    
  

  
    
      

Conceito
      

## Assinatura é um estado, não um pagamento
    
    
      

No Plano Premium, um pagamento acontecia uma vez e pronto. Numa assinatura, o pagamento se repete todo mês e muita coisa acontece **longe do seu app**: o cartão é recusado na renovação, o cliente troca de plano pelo portal, cancela às três da manhã. Quem sabe o estado verdadeiro da assinatura é o Stripe. O seu banco guarda uma cópia, e quem mantém essa cópia em dia são os **webhooks**.
      

Por isso o plano do usuário nunca muda porque o navegador voltou do checkout dizendo "deu certo". Ele muda quando o webhook chega, é validado e é processado. E toda rota que depende do plano consulta o banco a cada requisição: é isso que faz o limite do Free valer na API, e não só na tela.
    
  

  
    
      

Diagrama
      

## O ciclo de vida de uma assinatura
    
    
      
        
          
          Usuário
          Free → Pro

          
          App
          cria a sessão (subscription)

          
          Stripe Checkout
          cobra todo mês

          
          Webhook
          validado e processado uma vez

          
          Portal do cliente
          troca de plano · cancelamento

          
          Plano no banco
          plano, status, ids do Stripe

          
          Limites na API
          Free: 3 por mês · Pro: sem limite

          
          
          redireciona
          
          eventos
          
          atualiza
          
          updated / deleted
          
          gerenciar assinatura
          
          a cada requisição
        
      
      O app só abre caminhos (checkout e portal). Quem muda o plano no banco é sempre o webhook, que chega por fora, a qualquer hora, e pode chegar mais de uma vez.
    
  

  
    
      

Webhooks
      

## Os quatro eventos e o que cada um muda no seu banco
    
    
      

        
          
| Evento | Quando chega | O que o seu app faz |
        
        
          
| checkout.session.completed | O cliente concluiu o checkout da assinatura | Guarda no usuário o customer e a subscription do Stripe e muda o plano pra Pro |
          
| customer.subscription.updated | Troca de plano, renovação, cancelamento agendado, mudança de status | Atualiza o plano e o status da assinatura (active, past_due, canceled…) |
          
| customer.subscription.deleted | A assinatura terminou | Volta o usuário pro Free |
          
| invoice.payment_failed | A cobrança da renovação falhou | Marca a assinatura como inadimplente e avisa o usuário; o Pro não continua de graça |
        
      
    
    

Valide a assinatura de todo webhook, como no Plano Premium, e trate o mesmo evento chegando duas vezes: o Stripe pode reenviar, e processar duas vezes não pode mudar o resultado.
  

  
    
      

Requisitos
      

## O que precisa funcionar
    
    
      
        1
        
          

#### Planos no Stripe, em modo de teste
          

Free e Pro (Team é opcional), criados como produtos com preço recorrente mensal em R$. O Free pode ter preço R$ 0,00. Os nomes, os preços e os limites são os do modelo de receita do seu README.
        
      
      
        2
        
          

#### Checkout de assinatura
          

A rota que cria a Checkout Session com `mode=subscription` pro preço escolhido, ligada a um customer do Stripe que fica guardado no usuário. É o fluxo do Plano Premium, agora recorrente.
        
      
      
        3
        
          

#### Webhooks mantendo o plano no banco
          

Os quatro eventos da tabela acima, com validação da assinatura e sem efeito duplicado quando o mesmo evento chega de novo.
        
      
      
        4
        
          

#### Portal do cliente
          

Um botão "Gerenciar assinatura" que abre o Billing Portal do Stripe, onde o cliente troca de plano e cancela sozinho. Configure o portal no painel do Stripe, em modo de teste. Se o cancelamento for "no fim do período", o `customer.subscription.deleted` só chega no fim do mês; pra testar na hora, configure o cancelamento imediato.
        
      
      
        5
        
          

#### Limites por plano na API
          

O que o Free não pode fazer é barrado no backend, ex.: Free cria 3 registros por mês, Pro não tem limite. Estourou o limite, a API recusa com uma mensagem dizendo qual é o limite e como liberar.
        
      
      
        6
        
          

#### Painel admin
          

Só pro papel admin: quantos usuários em cada plano, assinaturas ativas, o MRR de teste (a soma do valor mensal das assinaturas ativas) e os últimos eventos do log.
        
      
    
  

  
    
      

Cenário de uso
      

## É assim que eu vou testar
    
    
      

Crio uma conta nova, que nasce no Free.
      

Uso até estourar o limite do Free, e a API recusa com uma mensagem clara.
      

Assino o Pro com o cartão de teste `4242 4242 4242 4242`, qualquer data futura e qualquer CVC.
      

Volto pro app: o plano já é Pro e o limite sumiu, porque o webhook chegou, e não porque o redirect disse que deu certo.
      

Abro o portal do cliente e cancelo. Quando o webhook do cancelamento chega, a conta volta pro Free.
      

Entro como admin e vejo os usuários por plano, as assinaturas ativas, o MRR de teste e os últimos eventos.
    
  

  
    
      

Documentação
      

## Onde consultar
    
    
      

        
          
| Documentação do Stripe | Pra quê |
        
        
          
| [Build a subscriptions integration](https://docs.stripe.com/billing/subscriptions/build-subscriptions?ui=stripe-hosted) | Checkout de assinatura hospedado pelo Stripe, do produto ao portal |
          
| [Using webhooks with subscriptions](https://docs.stripe.com/billing/subscriptions/webhooks) | Quais eventos uma assinatura gera e quando |
          
| [Receive Stripe events in your webhook endpoint](https://docs.stripe.com/webhooks) | Validar a assinatura e lidar com eventos repetidos |
          
| [Customer portal](https://docs.stripe.com/customer-management) | Configurar e abrir o Billing Portal |
          
| [Testing](https://docs.stripe.com/testing) | Cartões de teste, inclusive os que recusam a cobrança |
        
      
    
  

  
    
      

Entrega
      

## Mesmo repositório, mesmo canal
    
    

      
- Continue no **repositório novo, público**, o mesmo das atividades 9 e 10
      
- Continue mencionando o professor no README: [github.com/siriani](https://github.com/siriani)
      
- Abra uma **issue** no repositório do produto com o título `Entrega — SaaS completo`, mencionando `@siriani`, com o link do commit e os prints. A data e a hora da issue entram como evidência da data realizada
      
- Nenhuma chave do Stripe no repositório: só nas variáveis da stack no Portainer, e no repositório um `.env.example` com os nomes
      
- Prints: a assinatura concluída com o cartão 4242, o dashboard do Stripe em modo de teste com a assinatura, o webhook recebido (no seu log ou em Webhooks no painel do Stripe), o limite bloqueando no Free e o painel admin
    

  

  
    
      lapps.studio · ISW055 · atividade publicada 15/10/2026
      
        [GitHub](https://github.com/siriani)
