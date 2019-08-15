# Arquitetura orientada a eventos (POC)

Repositório usado como uma prova conceitual do apache kafka na arquitetura orientada a eventos.

![event-flow](/assets/event-flow.JPG)

**Requisitos:**

* Dotnet Core 2.2+
* C#
* Apache Kafka 2.2.0
* Apache ZooKeeper
* Confluent's .NET Client for Apache Kafka 1.1.0

# Executando a POC

Para executar a POC é necessário executar primeiro o projeto `ClientService.ConsoleApp` e depois o projeto `ValidationService.ConsoleApp`. A ideia é fazer com que o projeto `ClientService` gere uma quantidade `X` (será informada via console) de clientes no tópico `Customer`.

Após isso, o projeto `ValidationService`, que está escutando esse tópico, vai ler cada mensagem, validar e jogar o resultado no tópico `Customer-Validation`. Esse tópico deve conter apenas os clientes validados. 

O projeto `ClientService` além de produzir mensagens no tópico `Customer`, também está encarregado de ficar ouvindo o tópico `Customer-Validation` e emitira uma saída no console de todos os clientes que forem validados.

## Executando o `ClientService`

```
cd client-service\ClientService.ConsoleApp
dotnet run
Entre com a quantidade de mensagens: [informe o número de clientes que deseja gerar]
Delivered '1' to 'Customer [[0]] @31'
Costumer validated: [customerName-1 - 16900263-4d58-4d6e-bad2-ed8a8b41ba6e] registry validated with status: [Actived]
```

## Executando o `ValidationService`

```
cd validation-service\ValidationService.ConsoleApp
dotnet run
1 = New Customer:[customerName-1 - 30929ba2-c496-4d2a-9a23-64ca5060a8bd] registry validated.
```

# Configurações dos produtores

Estamos utilizando as seguintes configurações:

* **BootstrapServers** (bootstrap.servers): Define a lista (separada por vírgula) do par "host:porta" de cada broker.

**Observação:**

 A quantidade de configurações e seus respectivos valores podem aumentar com o decorrer da evolução da POC.

# Configurações dos consumidores

Estamos utilizando as seguintes configurações:

* **BootstrapServers** (bootstrap.servers): Define a lista (separada por vírgula) do par "host:porta" de cada broker.
* GroupId (group.id): Especifica o nome do grupo de consumidores ao qual um consumidor do Kafka pertence. O grupo é criado automaticamente caso ele não exista.
* **EnableAutoCommit** (enable.auto.commit): Por padrão, como o consumidor lê as mensagens do Kafka, ele periodicamente confirma seu deslocamento atual (definido como o deslocamento da próxima mensagem a ser lida) para as partições que ele está lendo de volta para o Kafka. Muitas vezes você gostaria de ter mais controle sobre quando as compensações são confirmadas. Neste caso, você pode definir enable.auto.commit para `false` e chamar o `commit` método do consumidor.
* **AutoOffSetReset** (auto.offset.reset): Quando não houver deslocamento inicial no Kafka ou se o deslocamento atual não existir mais no servidor (por exemplo, porque esses dados foram excluídos):
    * mais cedo: redefine automaticamente o deslocamento para o deslocamento mais antigo
    * mais recente: redefine automaticamente o deslocamento para o mais recente deslocamento
    * none: lançar exceção para o consumidor se nenhum offset anterior for encontrado para o grupo do consumidor
    * qualquer outra coisa: lançar exceção ao consumidor.
* **EnablePartionEof** (enable.auto.commit): Automaticamente e periodicamente consolide offsets em segundo plano. Nota: definir isso como false não impede que o consumidor busque compensações iniciais comprometidas anteriormente. Para contornar esse comportamento, defina deslocamentos iniciais específicos por partição na chamada. 

 **Observação:**

 A quantidade de configurações e seus respectivos valores podem aumentar com o decorrer da evolução da POC.