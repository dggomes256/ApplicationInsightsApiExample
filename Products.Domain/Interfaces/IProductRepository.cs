using Products.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Products.Domain.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
    }
}
