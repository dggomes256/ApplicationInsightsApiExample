using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Products.Consumer
{
    class Program
    {
        private static async Task Main(string[] args)
             => await new HostBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
                config.AddAzureAppConfiguration(options =>
                {
                    options.Connect(configuration["AppConfigurationConnectionString"])
                           .ConfigureRefresh(refresh =>
                           {
                               refresh.Register("ProductsConsumer:Sentinel", refreshAll: true)
                                      .SetCacheExpiration(new TimeSpan(0, 0, 5));
                           });
                });
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging
                .AddConfiguration(hostingContext.Configuration.GetSection("Logging:LogLevel:Default"))
                .AddConfiguration(hostingContext.Configuration.GetSection("Logging:ApplicationInsights:LogLevel:Default"))
                .AddConsole()
                .AddDebug()
                .AddApplicationInsights();
            })
            .ConfigureServices(services =>
            {
                services.AddAzureAppConfiguration();
                services.AddSingleton<ITelemetryInitializer, CloudRoleNameTelemetryInitializer>();
                services.AddApplicationInsightsTelemetryWorkerService();
                services.AddSingleton(services);
                services.AddApplicationInsightsTelemetryWorkerService();
                services.AddHostedService<Worker>();
            }
                )
            .RunConsoleAsync();
        


        
    }
}
