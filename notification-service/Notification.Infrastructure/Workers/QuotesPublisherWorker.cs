using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notification.Core.Entities;
using Notification.Infrastructure.Configuration;
using Notification.Infrastructure.Configurations;
using System;
using System.Threading;
using System.Threading.Tasks;
using ClientService.Core.UseCases;

namespace Notification.Infrastructure.Workers
{
    public class QuotesPublisherWorker : BackgroundService
    {
        private readonly ILogger<QuotesPublisherWorker> _logger;
        private readonly IQuoteRegistrationUseCase _quoteRegistrationUseCase;
        private readonly WorkersOptions.QuoteOptions _workersOptions;
        //private readonly HttpClient _httpClient;

        public QuotesPublisherWorker(
            ILogger<QuotesPublisherWorker> logger,
            IOptions<AppConfigurationOptions> configuration,
            IQuoteRegistrationUseCase quoteRegistrationUseCase
        )
        {
            //_httpClient = new HttpClient();
            _logger = logger;
            _workersOptions = configuration.Value.Workers.Quotes;
            _quoteRegistrationUseCase = quoteRegistrationUseCase;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"Quotes import is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                //var result = await _httpClient.GetAsync(_workersOptions.Url);
                //var json = await result.Content.ReadAsStringAsync();
                //var obj = JObject.Parse(json);
                //var value = obj["results"]["time"].ToString();

                _quoteRegistrationUseCase.Register(new Quote
                {
                    Date = DateTime.Now,
                    QuoteName = "PTBR4",
                    Value = DateTime.Now.Second
                });

                await Task.Delay(_workersOptions.Time, stoppingToken);
            }

            _logger.LogDebug($"Quotes import is stopping.");
        }
    }
}
