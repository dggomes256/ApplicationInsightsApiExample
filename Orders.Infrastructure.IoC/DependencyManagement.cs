using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orders.Domain.Interfaces;
using Orders.Infrastructure.Data;
using System;

namespace Orders.Infrastructure.IoC
{
    public static class DependencyManagement
    {
        public static void MapDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration["OrdersApi:ConnectionStrings:SQLite"];
            services.AddDbContext<ContextDB>(options =>
                options.UseSqlite(connection)
            );

            services.AddApplicationInsightsTelemetry();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddLogging(configure =>
                configure.AddConsole()
                .AddDebug()
                .AddApplicationInsights()
                ).Configure<LoggerFilterOptions>(options => options.MinLevel = Enum.TryParse(configuration.GetSection("OrdersApi:LogLevel").Value, out LogLevel logLevel) ? logLevel : LogLevel.Error);

            ;
        }
    }
}