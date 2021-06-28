using StkMS.Library.Contracts;
using StkMS.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public ValueTask<IEnumerable<Customer>> GetAllCustomerAsync()
        {
            lock (GATE)
            {
                return decorated.GetAllCustomerAsync();
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

        public Task<Customer> CreateCustomer(Customer customer) => throw new System.NotImplementedException();

        //

        private static readonly object GATE = new();

        private readonly IApiClient decorated;
    }
}
