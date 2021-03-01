using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orders.Domain.Entities;
using Orders.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Infrastructure.Data
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(ContextDB db, IConfiguration configuration, ILogger<BaseRepository<Order>> logger) : base(db, configuration, logger)
        {
        }
    }
}
