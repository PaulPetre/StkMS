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

        public ValueTask<Customer> CreateCustomerAsync(Customer customer)
        {
            lock (GATE)
            {
                return decorated.CreateCustomerAsync(customer);
            }
        }

        public ValueTask<Sale?> GetLastCompleteSaleAsync()
        {
            lock (GATE)
            {
                return decorated.GetLastCompleteSaleAsync();
            }
        }

        public Task AddOrUpdateAsync(ProductStock stock)
        {
            lock (GATE)
            {
                return decorated.AddOrUpdateAsync(stock);
            }
        }

        public Task SellProductAsync(ProductSale productSale)
        {
            lock (GATE)
            {
                return decorated.SellProductAsync(productSale);
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