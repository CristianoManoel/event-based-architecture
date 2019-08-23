using Confluent.Kafka;
using ServiceBus.Configurations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus.Kafka
{
    public class KafkaConsumer : IConsumer
    {
        private readonly List<IEventConsumer> _eventConsumers = new List<IEventConsumer>();
        private readonly List<string> _brokerList = new List<string>();

        private Action<Exception> _error;
        private string _topicName;
        private string _groupId;
        private bool _enableAutoCommit;
        private AutoOffsetReset _autoOffSetReset;
        private bool? _enablePartionEof;
        private int _maxPollIntervalMs;

        public KafkaConsumer()
        {
            this._maxPollIntervalMs = 60000;
        }

        #region Fluent setters

        public IConsumer AddBroker(params string[] brokers)
        {
            _brokerList.AddRange(Helper.ParseHosts(brokers));
            return this;
        }

        public IConsumer WithConfig(ConsumerSettings config)
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

        public IConsumer WithGroupId(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
                throw new ArgumentNullException(nameof(groupId));

            this._groupId = groupId;
            return this;
        }

        public IConsumer EnableAutoCommit(bool enabled)
        {
            this._enableAutoCommit = enabled;
            return this;
        }

        public IConsumer AutoOffSetReset(int autoOffSetReset) => AutoOffSetReset((AutoOffsetReset)autoOffSetReset);

        public IConsumer AutoOffSetReset(AutoOffsetReset autoOffSetReset)
        {
            this._autoOffSetReset = autoOffSetReset;
            return this;
        }

        public IConsumer EnablePartionEof(bool? enablePartionEof)
        {
            this._enablePartionEof = enablePartionEof;
            return this;
        }

        public IConsumer MaxPollIntervalMs(int maxPollIntervalMs)
        {
            this._maxPollIntervalMs = maxPollIntervalMs;
            return this;
        }

        public IConsumer Success(IEventConsumer eventConsumer)
        {
            this._eventConsumers.Add(eventConsumer);
            return this;
        }

        public IConsumer Success(IEnumerable<IEventConsumer> eventConsumers)
        {
            this._eventConsumers.AddRange(eventConsumers);
            return this;
        }

        public IConsumer Success<T>(Action<T> success)
        {
            this._eventConsumers.Add(new InternalEvent<T>(success));
            return this;
        }

        public IConsumer Error(Action<Exception> error)
        {
            this._error = error;
            return this;
        }

        #endregion

        public async void Subscribe(string topicName = null)
        {
            var config = new Confluent.Kafka.ConsumerConfig
            {
                BootstrapServers = string.Join(", ", _brokerList.ToArray()),
                GroupId = _groupId,
                EnableAutoCommit = _enableAutoCommit,
                AutoOffsetReset = _autoOffSetReset,
                EnablePartitionEof = _enablePartionEof,
                MaxPollIntervalMs = _maxPollIntervalMs
            };

            await Task.Run(() =>
            {
                using (var c = new ConsumerBuilder<string, string>(config).Build())
                {

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

                    CancellationTokenSource cts = new CancellationTokenSource();
                    Console.CancelKeyPress += (_, e) =>
                    {
                        e.Cancel = true; // prevent the process from terminating.
                        cts.Cancel();
                    };

                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var cr = c.Consume();

                                if (!cr.IsPartitionEOF)
                                {
                                    foreach (var ec in _eventConsumers)
                                    {
                                        var eventGenericType = GetEventGenericType(ec);
                                        var data = JsonConvert.DeserializeObject(cr.Value, eventGenericType);
                                        InvokeEvent(cr, ec, data);
                                    }
                                }

                                //else
                                //{
                                //Console.WriteLine($"End of Partion Reached.");
                                //}
                            }
                            catch (ConsumeException e)
                            {
                                //Console.WriteLine($"Error occured: {e.Error.Reason}");
                                _error?.Invoke(e);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Ensure the consumer leaves the group cleanly and final offsets are committed.
                        c.Close();
                    }
                }
            });
        }

        private void InvokeEvent(ConsumeResult<string, string> cr, IEventConsumer instance, object data)
        {
            var methodName = nameof(IEventConsumer<object>.Consume);
            var args = new object[] { data };
            instance.GetType().GetMethod(methodName).Invoke(instance, args);
        }

        private Type GetEventGenericType(IEventConsumer ec)
        {
            return ((Type[])((TypeInfo)ec.GetType()).ImplementedInterfaces)[0].GenericTypeArguments[0];
        }

        private class InternalEvent<T> : IEventConsumer<T>
        {
            private Action<T> _success;

            public InternalEvent(Action<T> success)
            {
                _success = success;
            }

            public void Consume(T data)
            {
                _success(data);
            }
        }
    }
}
