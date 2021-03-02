using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Products.Domain.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        public Task<int> Add(TEntity obj);
        public Task<IEnumerable<TEntity>> Get();

    }
}
