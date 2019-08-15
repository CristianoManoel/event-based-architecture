using System;
using System.Threading;
using System.Threading.Tasks;
using ClientService.Core.Interfaces.Events.Handlers;
using ClientService.Core.Interfaces.Events.Processors;
using ClientService.Infrastructure.Configurations;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace ClientService.Infrastructure.Kafka
{
    public class EventHandler<T> : IEventHandler<T> where T : class
    {
        private readonly KafkaOptions KafkaOptions;

        public EventHandler(KafkaOptions KafkaOptions)
        {
            this.KafkaOptions = KafkaOptions;
        }

        public async Task ConsumeEvents(string topicName, IEventProcessor<T> eventProcessor)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = KafkaOptions.Consumer.BootstrapServers,
                GroupId = KafkaOptions.Consumer.GroupId,
                EnableAutoCommit = KafkaOptions.Consumer.EnableAutoCommit,
                AutoOffsetReset = (AutoOffsetReset)KafkaOptions.Consumer.AutoOffSetReset,
                EnablePartitionEof = KafkaOptions.Consumer.EnablePartionEof,
                MaxPollIntervalMs = 600000
            };

            await Task.Run(() =>
            {
                Execute(config, topicName, eventProcessor);
            });
        }

        private void Execute(ConsumerConfig config, string topicName, IEventProcessor<T> eventProcessor)
        {
            using (var c = new ConsumerBuilder<string, string>(config).Build())
            {
                c.Subscribe(topicName);

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
                                // Console.WriteLine($"Event Data Consumed: '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                                var data = JsonConvert.DeserializeObject<T>(cr.Value);
                                eventProcessor.Process(data);
                            }
                            else
                            {
                                //Console.WriteLine($"End of Partion Reached.");
                            }
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
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
    }
}