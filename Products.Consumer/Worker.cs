using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Consumer
{
    internal class Worker:  IHostedService, IDisposable
    {
        public Worker()
        {
        }

        protected Task ExecuteAsync()
        {
            //Warmup das filas de envio
            while(true)
            {
                Thread.Sleep(500);
                Console.WriteLine(DateTime.Now);
            }
            

            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await ExecuteAsync();



            while (!cancellationToken.IsCancellationRequested) //Keep it alive
            {
                Thread.Sleep(500);
                Console.WriteLine(DateTime.Now);
            }


            Thread.Sleep(5000);
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Dispose();
            return Task.CompletedTask;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: set large fields to null.
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