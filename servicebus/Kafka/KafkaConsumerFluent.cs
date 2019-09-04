using Confluent.Kafka;
using ServiceBus.Configurations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using ServiceBus.Events;

namespace ServiceBus.Kafka
{
    public class KafkaConsumerFluent<T>
    {
        private readonly List<string> _brokerList = new List<string>();

        private Action<Exception> _error;
        private string _topicName;
        private string _groupId;
        private bool _enableAutoCommit;
        private AutoOffsetReset _autoOffSetReset;
        private bool? _enablePartionEof;
        private int _maxPollIntervalMs;
        private IEventProcessor<T> _eventConsumer;

        public KafkaConsumerFluent()
        {
            this._maxPollIntervalMs = 60000;
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
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        ConsumeResult<string, string> cr = null;

                        try
                        {
                            cr = c.Consume();

                            if (!cr.IsPartitionEOF)
                            {
                                //var args = new SubscribeEventArgs();                                
                                var data = JsonConvert.DeserializeObject<T>(cr.Value);
                                _eventConsumer.Subscribe(data);

                                if (!config.EnableAutoCommit.Value)
                                    c.Commit();
                            }

                            //else
                            //{
                            //Console.WriteLine($"End of Partion Reached.");
                            //}
                        }
                        catch (Exception e)
                        {
                            if (_error == null)
                            {
                                if (e is ConsumeException ce)
                                    Console.WriteLine($"Error occured: {ce.Error.Reason}");
                                else
                                    Console.WriteLine($"Error occured: {e.Message}");
                            }
                            else
                            {
                                _error.Invoke(e);
                            }

                            if (cr != null)
                                this.AddRetry(cr);
                        }
                    }

                    // c.Unsubscribe();
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
            const string RETRY_HEADER = "retryCount";
            const string ORIGINAL_TOPIC_NAME = "originalTopic";
            const int RETRY_MAX = 3;

            var retryCount = 0;
            var originalTopic = cr.Topic;

            if (cr.Headers.TryGetLastBytes(RETRY_HEADER, out byte[] b))
                retryCount = BitConverter.ToInt32(b, 0);

            if (cr.Headers.TryGetLastBytes(ORIGINAL_TOPIC_NAME, out byte[] b2))
                originalTopic = System.Text.Encoding.UTF8.GetString(b2);

            if (retryCount == RETRY_MAX)
            {
                await new KafkaProducerFluent<T>()
                    .AddBroker(_brokerList.ToArray())
                    .ProduceAsStringAsync(cr.Key, cr.Value, $"{originalTopic}.dlq", cr.Headers);
            }
            else
            {
                cr.Headers.Remove(RETRY_HEADER);
                cr.Headers.Remove(ORIGINAL_TOPIC_NAME);

                cr.Headers.Add(RETRY_HEADER, BitConverter.GetBytes(++retryCount));
                cr.Headers.Add(ORIGINAL_TOPIC_NAME, System.Text.Encoding.UTF8.GetBytes(originalTopic));

                await new KafkaProducerFluent<T>()
                    .AddBroker(_brokerList.ToArray())
                    .ProduceAsStringAsync(cr.Key, cr.Value, $"{originalTopic}.retry.{retryCount}", cr.Headers);
            }
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
