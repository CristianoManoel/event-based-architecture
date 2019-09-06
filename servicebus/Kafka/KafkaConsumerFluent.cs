using Confluent.Kafka;
using ServiceBus.Configurations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using ServiceBus.Events;
using ServiceBus.Extensions;
using System.Threading.Tasks;

namespace ServiceBus.Kafka
{
    public class KafkaConsumerFluent<T>
    {
        private const string ORIGINAL_TOPIC = "original.topic";
        private const string GROUP_ID = "group.id";
        private const string RETRY_TIMESTAMP = "retry.timestamp";
        private const string RETRY_COUNT = "retry.count";
        private const string DLQ = "dlq";
        private const string DELAY = "delay";
        private const int MAX_RETRY = 3;

        private readonly List<string> _brokerList = new List<string>();

        private Action<Exception> _error;
        private string _topicName;
        private string _topicNameRepublish;
        private string _groupId;
        private bool _enableAutoCommit;
        private AutoOffsetReset _autoOffSetReset;
        private bool? _enablePartionEof;
        private int _maxPollIntervalMs;
        private IEventProcessor<T> _eventConsumer;
        private int _delay;

        public KafkaConsumerFluent()
        {
            this._maxPollIntervalMs = 60000;
            // this._maxPollIntervalMs = 10000;
        }

        #region Fluent setters

        public KafkaConsumerFluent<T> AddBroker(params string[] brokers)
        {
            _brokerList.AddRange(Helper.ParseHosts(brokers));
            return this;
        }

        public KafkaConsumerFluent<T> WithConfig(ConsumerSettings config)
        {
            _topicName = config.Topic;

            AddBroker(config.BootstrapServers);
            WithGroupId(config.GroupId);
            EnableAutoCommit(config.EnableAutoCommit);
            AutoOffSetReset(config.AutoOffSetReset);
            EnablePartionEof(config.EnablePartionEof);
            TopicRepublish(config.TopicRepublish);
            Delay(config.Delay);

            if (config.MaxPollIntervalMs > 0)
                MaxPollIntervalMs(config.MaxPollIntervalMs);

            return this;
        }

        public KafkaConsumerFluent<T> WithGroupId(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
                throw new ArgumentNullException(nameof(groupId));

            this._groupId = groupId;
            return this;
        }

        public KafkaConsumerFluent<T> TopicRepublish(string topicName)
        {
            this._topicNameRepublish = topicName;
            return this;
        }

        public KafkaConsumerFluent<T> Delay(int delay)
        {
            this._delay = delay;
            return this;
        }

        public KafkaConsumerFluent<T> EnableAutoCommit(bool enabled)
        {
            this._enableAutoCommit = enabled;
            return this;
        }

        public KafkaConsumerFluent<T> AutoOffSetReset(int autoOffSetReset) => AutoOffSetReset((AutoOffsetReset)autoOffSetReset);

        public KafkaConsumerFluent<T> AutoOffSetReset(AutoOffsetReset autoOffSetReset)
        {
            this._autoOffSetReset = autoOffSetReset;
            return this;
        }

        public KafkaConsumerFluent<T> EnablePartionEof(bool? enablePartionEof)
        {
            this._enablePartionEof = enablePartionEof;
            return this;
        }

        public KafkaConsumerFluent<T> MaxPollIntervalMs(int maxPollIntervalMs)
        {
            this._maxPollIntervalMs = maxPollIntervalMs;
            return this;
        }

        public KafkaConsumerFluent<T> Success(IEventProcessor<T> eventConsumer)
        {
            this._eventConsumer = eventConsumer;
            return this;
        }

        //public KafkaConsumerFluent<T> Success(Action<T> success)
        //{
        //    this._eventConsumers.Add(new InternalEvent(success));
        //    return this;
        //}

        public KafkaConsumerFluent<T> Error(Action<Exception> error)
        {
            this._error = error;
            return this;
        }

        #endregion

        public void Subscribe(string topicName = null, CancellationToken cancellationToken = default)
        {
            if (_brokerList.Count == 0)
                throw new InvalidOperationException($"One broker must be added to build a consumer. Use the {nameof(AddBroker)} method to add a broker!");

            var config = new ConsumerConfig
            {
                BootstrapServers = string.Join(", ", _brokerList.ToArray()),
                GroupId = _groupId,
                EnableAutoCommit = _enableAutoCommit,
                AutoOffsetReset = _autoOffSetReset,
                EnablePartitionEof = _enablePartionEof,
                MaxPollIntervalMs = _maxPollIntervalMs
            };

            using (var c = new ConsumerBuilder<string, string>(config).Build())
            {
                if (cancellationToken != default)
                {
                    // Garante que ao cancelar a task, ocorra o unsubscribe, do contrário, 
                    // o cursor ficará preso na linha "c.Subscribe(...)" e o cancel() só terá efeito após retorno do kafka 
                    // que pode demorar minutos até o próximo looping do while.
                    cancellationToken.Register(() =>
                    {
                        c.Unsubscribe();
                    });
                }

                c.Subscribe(topicName ?? _topicName);

                //if (partition >= 0 && offset >= 0)
                //{
                //    var topicPartitionOffset = new TopicPartitionOffset(topicName, partition, offset);
                //    //c.Assign(new List<TopicPartitionOffset>() { topicPartitionOffset });

                //    if (assign)
                //        c.Assign(new List<TopicPartitionOffset>() { topicPartitionOffset });
                //    else 
                //        c.Seek(topicPartitionOffset);
                //}

                //CancellationTokenSource cts = new CancellationTokenSource();
                //Console.CancelKeyPress += (_, e) =>
                //{
                //    e.Cancel = true; // prevent the process from terminating.
                //    cts.Cancel();
                //};

                try
                {
                    while (cancellationToken == default || !cancellationToken.IsCancellationRequested)
                    {
                        ConsumeResult<string, string> cr = null;

                        try
                        {

                            cr = c.Consume();
                            // c.Pause(new List<TopicPartition>() { cr.TopicPartition });

                            if (cr.IsPartitionEOF)
                                continue;


                            /* 
                                Deve ler a mensagem após 10 segundos da sua produção. Exemplo

                                -> Delay: 10 segundos
                                -> Produzido: 1s 2s 3s 4s 5s 6s 7s 8s 9s 10s 11s 12s
                                -> Consumido:           ^
                                -> Após 10s :                                     ^

                                1) Obtem a diferença do tempo de consumo menos o tempo de produção
                                   (4s - 2s) = 2 (o tempo de consumo será sempre maior, pois é o tempo atual)
                                2) Descobre o tempo faltante para deixar a thead dormindo
                                   (10s - (4s - 2s)) = 8s (substrai o tempo do delay com o tempo gasto da msg para chegar até chegar aqui)
                            */
                            var delay = _delay;
                            // if (cr.Headers.TryGetValueInt32(DELAY, out int d))
                            //     delay = d;

                            if (delay > 0)
                            {
                                var currentUnixTs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                var sleep = (int)(delay - (currentUnixTs - cr.Timestamp.UnixTimestampMs));
                                if (sleep > 0)
                                {
                                    c.Pause(c.Assignment);

                                    // Caso utilize cancelation token, utilize o WaitOne.
                                    // garanta que saia do método após o cancelamento.
                                    if (cancellationToken == default)
                                        Thread.Sleep(sleep);
                                    else if (cancellationToken.WaitHandle.WaitOne(sleep))
                                        return;


                                    c.Resume(c.Assignment);
                                }
                            }

                            //cr.Headers.AddOrUpdate(RETRY_TIMESTAMP, _delay);

                            // Caso o grupo de consumo dessa mensagem exista (pois é uma mensagem com erro)
                            // verifica se o grupo de consumo atual é igual ao grupo de consumo original da mensagem
                            // caso não seja, então ignore a mensagem. Isso é feito para evitar que consumidores
                            // do mesmo tópico com grupo de consumo diferentes e que não geraram erro para está mensagem
                            // tenham que reprocessa-la.
                            var canProcess = true;
                            if (cr.Headers.TryGetValueString(GROUP_ID, out string messageGroupId) && messageGroupId != _groupId)
                                canProcess = false;

                            if (canProcess)
                            {
                                // Caso esse consumidor tenha tópico de republish, é recomendado não
                                // usar nenhum processor. Caso use, esse processor não pode gerar
                                // exception, do contrário não será feito o republish.  
                                if (_eventConsumer != null)
                                {
                                    var data = JsonConvert.DeserializeObject<T>(cr.Value);
                                    _eventConsumer.Subscribe(data);
                                }

                                // Republica a mensagem em outro topico. É usado em tópicos de retry
                                // que devem voltar as mensagens de volta para o tópico original.
                                // Cria uma proteção para não deixar que tópicos DLQ reenviem para o 
                                // tópico original, isso apenas evita problemas quando alguem se inscreve
                                // em tópicos DQL.
                                if (!string.IsNullOrWhiteSpace(this._topicNameRepublish)
                                    && !cr.Headers.ExistsKey(DLQ)
                                )
                                {
                                    var a = cr.Timestamp;
                                    Republish(cr, this._topicNameRepublish);
                                }
                            }

                            if (!config.EnableAutoCommit.Value)
                                c.Commit();

                        }
                        catch (Exception e)
                        {
                            if (_error == null)
                                if (e is ConsumeException ce)
                                    Console.WriteLine($"Error occured: {ce.Error.Reason}");
                                else
                                    Console.WriteLine($"Error occured: {e.Message}");
                            else
                                _error.Invoke(e);

                            if (cr != null)
                                this.AddRetry(cr);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed.
                    c.Close();
                }
            }
        }

        private async void AddRetry(ConsumeResult<string, string> cr)
        {
            // Se já está no tópico de mensagens mortas, então saia
            if (cr.Headers.ExistsKey(DLQ))
                return;

            // caso seja a primeira leitura com erro dessa mensagem, então
            // adiciona o nome original do grupo de consumidor para que somente esse grupo
            // de consumidor possa ler essa mensagem futuramente
            if (!cr.Headers.ExistsKey(GROUP_ID))
                cr.Headers.AddOrUpdate(GROUP_ID, _groupId);

            // caso seja a primeira leitura com erro dessa mensagem, então
            // adiciona o nome original do topico
            if (!cr.Headers.TryGetValueString(ORIGINAL_TOPIC, out string originalTopic))
            {
                originalTopic = cr.Topic;
                cr.Headers.AddOrUpdate(ORIGINAL_TOPIC, originalTopic);
            }

            cr.Headers.TryGetValueInt32(RETRY_COUNT, out int retryCount);

            string topic;
            if (retryCount == MAX_RETRY)
            {
                cr.Headers.AddOrUpdate(DLQ, true);
                topic = $"{originalTopic}.dlq";
            }
            else
            {
                cr.Headers.AddOrUpdate(RETRY_COUNT, ++retryCount);

                topic = $"{originalTopic}.retry.{retryCount}";
            }

            await Republish(cr, topic);
        }

        private async Task Republish(ConsumeResult<string, string> cr, string topic)
        {
            await new KafkaProducerFluent<string>()
                .AddBroker(_brokerList.ToArray())
                .ProduceAsync(cr.Key, cr.Value, topic, cr.Headers);
        }

        private class InternalEvent : IEventProcessor<T>
        {
            private Action<T> _success;

            public InternalEvent(Action<T> success)
            {
                _success = success;
            }

            public void Subscribe(T data)
            {
                _success(data);
            }
        }
    }
}
