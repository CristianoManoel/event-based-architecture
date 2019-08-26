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
        private readonly List<IEventSubscriber<T>> _eventConsumers = new List<IEventSubscriber<T>>();
        private readonly List<string> _brokerList = new List<string>();

        private Action<Exception> _error;
        private string _topicName;
        private string _groupId;
        private bool _enableAutoCommit;
        private AutoOffsetReset _autoOffSetReset;
        private bool? _enablePartionEof;
        private int _maxPollIntervalMs;

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

        public KafkaConsumerFluent<T> Success(IEventSubscriber<T> eventConsumer)
        {
            this._eventConsumers.Add(eventConsumer);
            return this;
        }

        public KafkaConsumerFluent<T> Success(IEnumerable<IEventSubscriber<T>> eventConsumers)
        {
            this._eventConsumers.AddRange(eventConsumers);
            return this;
        }

        public KafkaConsumerFluent<T> Success(Action<T> success)
        {
            this._eventConsumers.Add(new InternalEvent(success));
            return this;
        }

        public KafkaConsumerFluent<T> Error(Action<Exception> error)
        {
            this._error = error;
            return this;
        }

        #endregion

        public void Subscribe(string topicName = null)
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
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume();
                            
                            if (!cr.IsPartitionEOF)
                            {
                                var args = new SubscribeEventArgs();
                                foreach (var ec in _eventConsumers)
                                {
                                    //var eventGenericType = GetEventGenericType(ec);
                                    //var data = JsonConvert.DeserializeObject(cr.Value, eventGenericType);
                                    //InvokeEvent(cr, ec, data);
                                    var data = JsonConvert.DeserializeObject<T>(cr.Value);
                                    ec.Subscribe(data, args);
                                }

                                if (args.Commit)
                                    c.Commit();
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
        }

        //private void InvokeEvent(ConsumeResult<string, string> cr, IEventSubscriber instance, object data)
        //{
        //    var methodName = nameof(IEventSubscriber<object>.Subscribe);
        //    var args = new object[] { data };
        //    instance.GetType().GetMethod(methodName).Invoke(instance, args);
        //}

        //private Type GetEventGenericType(IEventSubscriber ec)
        //{
        //    return ((Type[])((TypeInfo)ec.GetType()).ImplementedInterfaces)[0].GenericTypeArguments[0];
        //}

        private class InternalEvent : IEventSubscriber<T>
        {
            private Action<T> _success;

            public InternalEvent(Action<T> success)
            {
                _success = success;
            }

            public void Subscribe(T data, SubscribeEventArgs args)
            {
                _success(data);
            }
        }
    }
}
