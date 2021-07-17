using StkMS.Library.Contracts;
using StkMS.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace StkMS.Library.Services
{
    public class ApiClientCachingDecorator : IApiClient
    {
        public ApiClientCachingDecorator(IApiClient decorated, ICache cache)
        {
            this.decorated = decorated;
            this.cache = cache;
        }

        public async ValueTask<IEnumerable<ProductStock>> GetAllAsync()
        {
            try
            {
                await PostFromQueueAsync().ConfigureAwait(false);

                var result = (await decorated.GetAllAsync().ConfigureAwait(false)).ToArray();
                cache[ALL_KEY] = JsonSerializer.Serialize(result);
                return result;
            }
            catch
            {
                return SafeDeserialize(cache[ALL_KEY], Array.Empty<ProductStock>());
            }
        }

        public async ValueTask<IEnumerable<Customer>> GetAllCustomerAsync()
        {
            try
            {
                var result = (await decorated.GetAllCustomerAsync().ConfigureAwait(true)).ToArray();
                cache[ALL_CUSTOMERS_KEY] = JsonSerializer.Serialize(result);
                return result;
            }
            catch
            {
                return SafeDeserialize(cache[ALL_CUSTOMERS_KEY], Array.Empty<Customer>());
            }
        }


        public async ValueTask<ProductStock?> FindStockAsync(string productCode)
        {
            try
            {
                await PostFromQueueAsync().ConfigureAwait(false);

                var result = await decorated.FindStockAsync(productCode).ConfigureAwait(false);
                if (result != null)
                    cache[STOCK_KEY + productCode] = JsonSerializer.Serialize(result);
                return result;
            }
            catch
            {
                return SafeDeserialize<ProductStock?>(cache[STOCK_KEY + productCode], null);
            }
        }

        public async ValueTask<ProductStock?> FindCustomerAsync(string cui)
        {
            try
            {
                await PostFromQueueCustomersAsync().ConfigureAwait(false);

                var result = await decorated.FindStockAsync(cui).ConfigureAwait(false);
                if (result != null)
                    cache[STOCK_KEY + cui] = JsonSerializer.Serialize(result);
                return result;
            }
            catch
            {
                return SafeDeserialize<ProductStock?>(cache[STOCK_KEY + cui], null);
            }
        }

        public async ValueTask<Product?> FindProductAsync(string productCode)
        {
            try
            {
                await PostFromQueueAsync().ConfigureAwait(false);

                var result = await decorated.FindProductAsync(productCode).ConfigureAwait(false);
                if (result != null)
                    cache[PRODUCT_KEY + productCode] = JsonSerializer.Serialize(result);
                return result;
            }
            catch
            {
                return SafeDeserialize<Product?>(cache[PRODUCT_KEY + productCode], null);
            }
        }

        public ValueTask<Customer> CreateCustomerAsync(Customer customer) => decorated.CreateCustomerAsync(customer);

        public ValueTask<Sale?> GetLastCompleteSaleAsync() => decorated.GetLastCompleteSaleAsync();
        public ValueTask<Inventory?> GetMostRecentInventoryAsync() => decorated.GetMostRecentInventoryAsync();

        public async Task AddOrUpdateAsync(ProductStock stock)
        {
            try
            {
                await decorated.AddOrUpdateAsync(stock).ConfigureAwait(false);
                await FindStockAsync(stock.ProductCode).ConfigureAwait(false);

                await PostFromQueueAsync().ConfigureAwait(false);

                await GetAllAsync().ConfigureAwait(false);
            }
            catch
            {
                cache[STOCK_KEY + stock.ProductCode] = JsonSerializer.Serialize(stock);
                cache[PRODUCT_KEY + stock.ProductCode] = JsonSerializer.Serialize(stock.Product);

                var cachedResult = cache[ALL_KEY] ?? "[]";
                var all = SafeDeserialize(cachedResult, new List<ProductStock>());

                var existing = all.Where(it => it.ProductCode == stock.ProductCode).FirstOrDefault();
                if (existing == null)
                    all.Add(stock);
                else
                    existing.CopyFrom(stock);

                cache[ALL_KEY] = JsonSerializer.Serialize(all);

                queue.Enqueue(stock);
            }
        }

        public async Task RegisterInventoryAsync(ProductStock stock)
        {
            try
            {
                await decorated.RegisterInventoryAsync(stock).ConfigureAwait(false);
                await FindStockAsync(stock.ProductCode).ConfigureAwait(false);

                await PostFromQueueAsync().ConfigureAwait(false);

                await GetAllAsync().ConfigureAwait(false);
            }
            catch
            {
                cache[STOCK_KEY + stock.ProductCode] = JsonSerializer.Serialize(stock);
                cache[PRODUCT_KEY + stock.ProductCode] = JsonSerializer.Serialize(stock.Product);

                var cachedResult = cache[ALL_KEY] ?? "[]";
                var all = SafeDeserialize(cachedResult, new List<ProductStock>());

                var existing = all.Where(it => it.ProductCode == stock.ProductCode).FirstOrDefault();
                if (existing == null)
                    all.Add(stock);
                else
                    existing.CopyFrom(stock);

                cache[ALL_KEY] = JsonSerializer.Serialize(all);

                queue.Enqueue(stock);
            }
        }

        public Task SellProductAsync(ProductSale productSale) => decorated.SellProductAsync(productSale);
        public Task CompleteSaleAsync() => decorated.CompleteSaleAsync();

        public Task BeginInventoryAsync() => decorated.BeginInventoryAsync();
        public Task CompleteInventoryAsync() => decorated.CompleteInventoryAsync();

        public Task DeleteProductAsync(string productCode) => decorated.DeleteProductAsync(productCode);

        public async Task AddOrUpdateCustomerAsync(Customer customer)
        {
            try
            {
                await decorated.AddOrUpdateCustomerAsync(customer).ConfigureAwait(false);
                await FindStockAsync(customer.CUI).ConfigureAwait(false);

                await PostFromQueueCustomersAsync().ConfigureAwait(false);

                await GetAllCustomerAsync().ConfigureAwait(false);
            }
            catch
            {
                cache[CUSTOMER_KEY + customer.CUI] = JsonSerializer.Serialize(customer.CUI);

                var cachedResult = cache[ALL_KEY] ?? "[]";
                var all = SafeDeserialize(cachedResult, new List<Customer>());

                var existing = all.Where(it => it.CUI == customer.CUI).FirstOrDefault();
                if (existing == null)
                    all.Add(customer);
                // else 
                // existing.CopyFrom(customer);

                cache[ALL_KEY] = JsonSerializer.Serialize(all);

                queueCustomers.Enqueue(customer);
            }
        }

        public Task DeleteCustomerAsync(string customerCui) => decorated.DeleteCustomerAsync(customerCui);
        //

        private const string ALL_CUSTOMERS_KEY = "CUSTOMERS";
        private const string ALL_KEY = "ALL";
        private const string STOCK_KEY = "STOCK:";
        private const string PRODUCT_KEY = "PRODUCT:";
        private const string CUSTOMER_KEY = "CUSTOMER:";

        private readonly Queue<ProductStock> queue = new();
        private readonly Queue<Customer> queueCustomers = new();

        private readonly IApiClient decorated;
        private readonly ICache cache;

        private static T SafeDeserialize<T>(string? serialized, T defaultValue)
        {
            try
            {
                return serialized == null ? defaultValue : JsonSerializer.Deserialize<T>(serialized) ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private async Task PostFromQueueAsync()
        {
            while (queue.Any())
            {
                var item = queue.Dequeue();
                try
                {
                    await decorated.RegisterInventoryAsync(item).ConfigureAwait(false);
                    await FindStockAsync(item.ProductCode).ConfigureAwait(false);
                }
                catch
                {
                    queue.Enqueue(item);
                    break;
                }
            }
        }

        private async Task PostFromQueueCustomersAsync()
        {
            while (queueCustomers.Any())
            {
                var item = queueCustomers.Dequeue();
                try
                {
                    //  await decorated.RegisterInventoryAsync(item).ConfigureAwait(false);
                    await FindStockAsync(item.CUI).ConfigureAwait(false);
                }
                catch
                {
                    queueCustomers.Enqueue(item);
                    break;
                }
            }
        }
    }
}
