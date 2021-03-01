using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Products.Domain.Interfaces;
using Products.Infrastructure.Data;
using System;

namespace Products.Infrastructure.IoC
{
    public static class DependencyManagement
    {
        public static void MapDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration["ProductsApi:ConnectionStrings:SQLite"];
            services.AddDbContext<ContextDB>(options =>
                options.UseSqlite(connection)
            );

            services.AddApplicationInsightsTelemetry();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddLogging(configure =>
                configure.AddConsole()
                .AddDebug()
                .AddApplicationInsights()
                ).Configure<LoggerFilterOptions>(options => options.MinLevel = Enum.TryParse(configuration.GetSection("ProductsApi:LogLevel").Value, out LogLevel logLevel) ? logLevel : LogLevel.Error);

            ;
        }
    }
}