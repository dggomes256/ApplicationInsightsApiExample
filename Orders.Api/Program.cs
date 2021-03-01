using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace Orders.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var settings = config.Build();
                config.AddAzureAppConfiguration(options =>
                {
                    options.Connect(settings["AppConfigurationConnectionString"])
                           .ConfigureRefresh(refresh =>
                           {
                               refresh.Register("OrdersApi:Sentinel", refreshAll: true)
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
                .AddApplicationInsights(hostingContext.Configuration.GetSection("OrdersApi:ApplicationInsights:InstrumentationKey").Value);
            });
    }
}