using Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orders.Domain.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
    }
}
