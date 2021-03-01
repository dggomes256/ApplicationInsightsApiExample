using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Products.Domain.Entities;
using Products.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Products.Infrastructure.Data
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ContextDB db, IConfiguration configuration, ILogger<BaseRepository<Product>> logger) : base(db, configuration, logger)
        {
        }
    }
}
