﻿using System;
using System.Collections.Generic;
using Products.Domain.Entities;
using System.Text;
using Products.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Products.Infrastructure.Data
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>, IDisposable where TEntity : class
    {
        protected ContextDB Db;
        protected IConfiguration Configuration;
        protected ILogger<BaseRepository<TEntity>> Logger;

        public BaseRepository(ContextDB db, IConfiguration configuration, ILogger<BaseRepository<TEntity>> logger)
        {
            Db = db;
            Configuration = configuration;
            Logger = logger;
        }

        public async Task<int> Add(TEntity obj)
        {
            Logger.LogInformation("Metodo Add");
            await Db.Set<TEntity>().AddAsync(obj);
            return await Db.SaveChangesAsync();
        }

        public async Task<IEnumerable<TEntity>> Get()
        {
            return await Db.Set<TEntity>().ToListAsync();
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.Db.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}
