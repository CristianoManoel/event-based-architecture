using ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Notification.Core.Entities;
using Notification.Infrastructure.Configuration;
using Notification.Infrastructure.Configurations;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Notification.Infrastructure.Workers
{
    public class QuotesWorker : BackgroundService
    {
        private readonly ILogger<QuotesWorker> _logger;
        private readonly IEventProducer<Quote> _quoteProducer;
        private readonly WorkersOptions.QuoteOptions _workersOptions;
        private readonly HttpClient _httpClient;

        public QuotesWorker(
            ILogger<QuotesWorker> logger,
            IOptions<ConfigurationOptions> configuration,
            IEventProducer<Quote> quoteProducer
        )
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _workersOptions = configuration.Value.Workers.Quotes;
            _quoteProducer = quoteProducer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"Quote workers is starting.");

            //stoppingToken.Register(() => _logger.LogDebug($"Quote workers background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                //var result = await _httpClient.GetAsync(_workersOptions.Url);
                //var json = await result.Content.ReadAsStringAsync();
                //var obj = JObject.Parse(json);
                //var value = obj["results"]["time"].ToString();

                _quoteProducer.Produce(new Quote
                {
                    Date = DateTime.Now,
                    QuoteName = "PTBR4",
                    Value = DateTime.Now.Second
                });

                await Task.Delay(_workersOptions.Time, stoppingToken);
            }

            _logger.LogDebug($"Quote workers is stopping.");
        }
    }
}
