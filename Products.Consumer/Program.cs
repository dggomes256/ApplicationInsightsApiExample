using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Products.Infrastructure.IoC;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Products.Consumer
{
    internal class Program
    {
        private static string instrumentationKey;
        private static IConfiguration configuration;
        private static async Task Main(string[] args)
        {
            var builder = new HostBuilder();

            builder.ConfigureAppConfiguration(config =>
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
            });
            builder.ConfigureLogging((hostingContext, logging) =>
            {
                instrumentationKey = hostingContext.Configuration.GetSection("ProductsApi:ApplicationInsights:InstrumentationKey").Value;
                configuration = hostingContext.Configuration;
                logging
                .AddConfiguration(hostingContext.Configuration.GetSection("Logging:LogLevel:Default"))
                .AddConfiguration(hostingContext.Configuration.GetSection("Logging:ApplicationInsights:LogLevel:Default"))
                .AddConsole()
                .AddDebug()
                .AddApplicationInsights(hostingContext.Configuration.GetSection("ProductsConsumer:ApplicationInsights:InstrumentationKey").Value);
            });
            builder.ConfigureServices(services =>
            {
                services.AddApplicationInsightsTelemetryWorkerService(instrumentationKey);
                services.AddSingleton<ILogger>((opt) => opt.GetService<ILoggerFactory>()?.CreateLogger<Worker>());
                services.AddAzureAppConfiguration();
                services.AddSingleton<ITelemetryInitializer, CloudRoleNameTelemetryInitializer>();
                services.AddSingleton(services);
                services.AddHostedService<Worker>();
                services.MapDependencies(configuration);

            });
            builder.RunConsoleAsync();
        }
    }
}