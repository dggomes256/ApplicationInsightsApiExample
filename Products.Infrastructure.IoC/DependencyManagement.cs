using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Products.Domain.Interfaces;
using Products.Infrastructure.Data;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Products.Infrastructure.IoC
{
    public static class DependencyManagement
    {
        public static void MapDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var path = Path.GetDirectoryName(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                path = string.Concat(path, "/");
            }
            else
            {
                path = string.Concat(path, "\\");
            }
            path = path.Replace("file:\\", "");
            
            var connection = configuration["ProductsDB:ConnectionStrings:SQLite"];
            connection = string.Format(connection, path);

            services.AddDbContext<ContextDB>(options =>
                options.UseSqlite(connection)
            );

            services.AddApplicationInsightsTelemetry();
            services.AddScoped<IProductRepository, ProductRepository>();
            ;
        }
    }
}