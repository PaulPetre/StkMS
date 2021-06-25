using System.Collections.Generic;
using System.Threading.Tasks;
using StkMS.Library.Contracts;
using StkMS.Library.Models;

namespace StkMS.Library.Services
{
    public class ApiClientThreadSafeDecorator : IApiClient
    {
        public ApiClientThreadSafeDecorator(IApiClient decorated)
        {
            this.decorated = decorated;
        }

        public ValueTask<IEnumerable<ProductStock>> GetAllAsync()
        {
            lock (GATE)
            {
                return decorated.GetAllAsync();
            }
        }

        public ValueTask<ProductStock?> FindStockAsync(string productCode)
        {
            lock (GATE)
            {
                return decorated.FindStockAsync(productCode);
            }
        }

        public ValueTask<Product?> FindProductAsync(string productCode)
        {
            lock (GATE)
            {
                return decorated.FindProductAsync(productCode);
            }
        }

        public Task AddOrUpdateAsync(ProductStock stock)
        {
            lock (GATE)
            {
                return decorated.AddOrUpdateAsync(stock);
            }
        }

        public Task SellProductAsync(Sale sale)
        {
            lock (GATE)
            {
                return decorated.SellProductAsync(sale);
            }
        }

        public Task CompleteSaleAsync()
        {
            lock (GATE)
            {
                return decorated.CompleteSaleAsync();
            }
        }

        //

        private static readonly object GATE = new();

        private readonly IApiClient decorated;
    }
}