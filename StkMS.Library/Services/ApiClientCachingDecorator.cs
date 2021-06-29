using StkMS.Library.Contracts;
using StkMS.Library.Models;
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
                var cachedResult = cache[ALL_KEY];
                return cachedResult == null
                    ? Enumerable.Empty<ProductStock>()
                    : JsonSerializer.Deserialize<ProductStock[]>(cachedResult) ?? Enumerable.Empty<ProductStock>();
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
                var cachedResult = cache[ALL_CUSTOMERS_KEY];
                return cachedResult == null
                    ? Enumerable.Empty<Customer>()
                    : JsonSerializer.Deserialize<Customer[]>(cachedResult) ?? Enumerable.Empty<Customer>();
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
                var cachedResult = cache[STOCK_KEY + productCode];
                return cachedResult == null
                    ? null
                    : JsonSerializer.Deserialize<ProductStock>(cachedResult) ?? null;
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
                var cachedResult = cache[PRODUCT_KEY + productCode];
                return cachedResult == null
                    ? null
                    : JsonSerializer.Deserialize<Product>(cachedResult) ?? null;
            }
        }

        public ValueTask<Customer> CreateCustomerAsync(Customer customer) => decorated.CreateCustomerAsync(customer);

        public ValueTask<Sale?> GetLastCompleteSaleAsync() => decorated.GetLastCompleteSaleAsync();

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
                var all = (JsonSerializer.Deserialize<ProductStock[]>(cachedResult) ?? Enumerable.Empty<ProductStock>()).ToList();

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

        //

        private const string ALL_CUSTOMERS_KEY = "CUSTOMERS";
        private const string ALL_KEY = "ALL";
        private const string STOCK_KEY = "STOCK:";
        private const string PRODUCT_KEY = "PRODUCT:";

        private readonly Queue<ProductStock> queue = new();

        private readonly IApiClient decorated;
        private readonly ICache cache;

        private async Task PostFromQueueAsync()
        {
            while (queue.Any())
            {
                var item = queue.Dequeue();
                try
                {
                    await decorated.AddOrUpdateAsync(item).ConfigureAwait(false);
                    await FindStockAsync(item.ProductCode).ConfigureAwait(false);
                }
                catch
                {
                    queue.Enqueue(item);
                    break;
                }
            }
        }
    }
}
