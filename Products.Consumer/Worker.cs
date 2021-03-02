using Azure.Messaging.ServiceBus;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Products.Domain.Entities;
using Products.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Consumer
{
    internal class Worker : IHostedService, IDisposable
    {
        private ILogger _logger;
        private TelemetryClient _telemetryClient;
        private IProductRepository _productRepository;
        private IConfiguration _configuration;

        public Worker(ILogger logger, TelemetryClient telemetryClient, IProductRepository productRepository, IConfiguration configuration)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
            _productRepository = productRepository;
            _configuration = configuration;
        }

        private  async Task MessageHandler(ProcessMessageEventArgs args)
        {
            try
            {
                string body = args.Message.Body.ToString();
                _logger.LogInformation("Deserializing Object");
                Product product = JsonConvert.DeserializeObject<Product>(body);

                _logger.LogInformation("Adding object to Database");
                int result = await _productRepository.Add(product);


                _logger.LogInformation("Operation result: " + result.ToString().Replace("1", "Sucess").Replace("0", "Failed"));

            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception: {ex.Message}");
            }
            // complete the message. messages is deleted from the queue.
            await args.CompleteMessageAsync(args.Message);


        }

        // handle any errors when receiving messages
        private  Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task ReceiveMessagesAsync()
        {

            using (_telemetryClient.StartOperation<RequestTelemetry>("ServiceBus.ReceiveMessage"))
            {
                await using (ServiceBusClient client = new ServiceBusClient(_configuration.GetSection("ProductsServiceBus:ConnectionString").Value))
                {
                    // create a processor that we can use to process the messages
                    ServiceBusProcessor processor = client.CreateProcessor(_configuration.GetSection("ProductsServiceBus:CreateProductQueueName").Value, new ServiceBusProcessorOptions());

                    // add handler to process messages
                    processor.ProcessMessageAsync += MessageHandler;

                    // add handler to process any errors
                    processor.ProcessErrorAsync += ErrorHandler;

                    // start processing 
                    await processor.StartProcessingAsync();

                    Console.ReadKey();

                    // stop processing 
                    Console.WriteLine("\nStopping the receiver...");
                    await processor.StopProcessingAsync();
                    Console.WriteLine("Stopped receiving messages");
                }
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested) //Keep it alive
            {
                Thread.Sleep(500);
                await ReceiveMessagesAsync();
            }
            _telemetryClient.Flush();
            Thread.Sleep(5000);

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Dispose();
            return Task.CompletedTask;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: set large fields to null.
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}