using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientService.Core.Entities;
using ClientService.Core.Enums;
using ClientService.Core.Interfaces;
using ClientService.Core.Interfaces.Events.Handlers;
using ClientService.Core.Interfaces.Events.Processors;
using ClientService.Core.Interfaces.UseCases;
using ClientService.Infrastructure.Configurations;
using ClientService.Infrastructure.Extensions.DependencyInjection;
using ClientService.Infrastructure.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClientService.ConsoleApp
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var configuration = BuildConfiguration();
            var serviceProvider = BuilderServiceProvider(configuration);
            //CustomerConsumer(serviceProvider);
            CustomerProducer2(serviceProvider);
            Console.Read();
        }

        private static void CustomerConsumer(ServiceProvider serviceProvider)
        {
            var customerRegistryValidationEventHandler = serviceProvider.GetService<Core.Interfaces.Events.Handlers.IEventHandler<Customer>>();
            var customerRegistryValidatedEventProcessor = serviceProvider.GetService<ICustomerRegistryValidatedEventProcessor>();
            customerRegistryValidationEventHandler.ConsumeEvents("Customer-Validation", customerRegistryValidatedEventProcessor);
        }

        private static void CustomerProducer2(ServiceProvider serviceProvider)
        {
            List<Customer> list;

            //registerCustomer = serviceProvider.GetService<IRegisterCustomer>();

            //while (true)
            {
                //Console.WriteLine("Entre com a quantidade de mensagens:");
                //var repeat = Convert.ToInt32(Console.ReadLine());
                var repeat = 1;
                list = new List<Customer>();

                for (var i = 1; i <= repeat; i++)
                {
                    var customerName = "customerName-" + i;
                    var customerCPF = "customerCPF-" + i;
                    var id = Guid.NewGuid();
                    list.Add(new Customer { Id = i, Cpf = customerCPF, Name = $"{customerName} - {id.ToString()}", RegisterStatus = RegisterStatus.Received });

                    var kafkaOptions = serviceProvider.GetService<KafkaOptions>();

                    var key = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
                    var topic = "B";

                    //var kafkaProducer = new KafkaProducer();
                    //kafkaProducer
                    //    .AddBroker(kafkaOptions.Producer.BootstrapServers)
                    //    .Success(dr =>
                    //    {
                    //        //Console.WriteLine($"Result Key: {dr.Key}|Offset: {dr.Offset}|Partition: {dr.Partition}|Status:{dr.Status}|TopicPartition: {dr.TopicPartition}|{dr.TopicPartitionOffset}");
                    //    })
                    //    .Error(e =>
                    //    {
                    //        Console.WriteLine(e.ToString());
                    //    })
                    //    .Produce(topic, key, list.Last());

                    //var kafkaConsumer = new KafkaConsumer<Customer>();
                    //kafkaConsumer
                    //    .AddBroker(kafkaOptions.Consumer.BootstrapServers)
                    //    .WithGroupId(topic)
                    //    //.EnableAutoCommit(kafkaOptions.Consumer.EnableAutoCommit)
                    //    .EnableAutoCommit(false)
                    //    //.EnablePartionEof(kafkaOptions.Consumer.EnablePartionEof)
                    //    .AutoOffSetReset(AutoOffsetReset.Earliest)
                    //    .Success((entity, cr) =>
                    //    {
                    //        Console.WriteLine(cr.Key);
                    //    })
                    //    .Error(e =>
                    //    {
                    //        if (e is ConsumeException ce)
                    //            Console.WriteLine($"Error occured: {ce.Error.Reason}");
                    //    })
                    //    .Subscribe(topic);

                    //var kafkaConsumer2 = new KafkaConsumer<Customer>();
                    //kafkaConsumer2
                    //    .AddBroker(kafkaOptions.Consumer.BootstrapServers)
                    //    .WithGroupId(topic)
                    //    //.EnableAutoCommit(kafkaOptions.Consumer.EnableAutoCommit)
                    //    .EnableAutoCommit(false)
                    //    //.EnablePartionEof(kafkaOptions.Consumer.EnablePartionEof)
                    //    .AutoOffSetReset(AutoOffsetReset.Earliest)
                    //    .Success((entity, cr) =>
                    //    {
                    //        Console.WriteLine(cr.Key);
                    //    })
                    //    .Error(e =>
                    //    {
                    //        if (e is ConsumeException ce)
                    //            Console.WriteLine($"Error occured: {ce.Error.Reason}");
                    //    })
                    //    .Subscribe(topic);

                    //key = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
                    //kafkaProducer.Publish("A", i.ToString(), list.Last());
                    //Console.ReadKey();
                }

                //registerCustomer.Register(list).Wait();


            }
        }


        private static void CustomerProducer(ServiceProvider serviceProvider)
        {
            IRegisterCustomer registerCustomer;
            List<Customer> list;

            registerCustomer = serviceProvider.GetService<IRegisterCustomer>();

            while (true)
            {
                Console.WriteLine("Entre com a quantidade de mensagens:");
                var repeat = Convert.ToInt32(Console.ReadLine());
                list = new List<Customer>();

                for (var i = 1; i <= repeat; i++)
                {
                    var customerName = "customerName-" + i;
                    var customerCPF = "customerCPF-" + i;
                    var id = Guid.NewGuid();
                    list.Add(new Customer { Id = i, Cpf = customerCPF, Name = $"{customerName} - {id.ToString()}", RegisterStatus = RegisterStatus.Received });
                }

                registerCustomer.Register(list).Wait();
            }
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", false, true)
                .Build();
        }

        private static ServiceProvider BuilderServiceProvider(IConfigurationRoot configuration)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.Register();
            serviceCollection.RegisterConfigurationOptions(configuration);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
