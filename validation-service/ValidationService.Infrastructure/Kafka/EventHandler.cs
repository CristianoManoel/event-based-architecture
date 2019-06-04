using ValidationService.Infrastructure.Configurations;
using Newtonsoft.Json;
using Confluent.Kafka;
using System;
using System.Threading;
using ValidationService.Core.Interfaces.Events.Handlers;
using ValidationService.Core.Interfaces.Events.Processors;

namespace ValidationService.Infrastructure.Kafka
{
    public class EventHandler<T> : IEventHandler<T>
    where T : class
    {
        private readonly KafkaOptions KafkaOptions;

        public EventHandler(KafkaOptions KafkaOptions)
        {
            this.KafkaOptions = KafkaOptions;

        }
        public void ConsumeEvents(string topicName, IEventProcessor<T> eventProcessor)
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
                            
                            if(!cr.IsPartitionEOF){
                                // Console.WriteLine($"Event Data Consumed: '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                                var data = JsonConvert.DeserializeObject<T>(cr.Value);
                                eventProcessor.Process(data);
                            }
                            else
                            {
                                Console.WriteLine($"End of Partion Reached.");
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