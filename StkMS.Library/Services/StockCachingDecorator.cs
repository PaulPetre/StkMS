using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StkMS.Library.Contracts;
using StkMS.Library.Models;

namespace StkMS.Library.Services
{
    public class StockCachingDecorator : IStock
    {
        public StockCachingDecorator(IStock decorated, ICache cache)
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

        public async ValueTask<ProductStock?> FindProductAsync(string productCode)
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
                    : JsonSerializer.Deserialize<ProductStock>(cachedResult) ?? null;
            }
        }

        public async Task AddOrUpdateAsync(ProductStock stock)
        {
            try
            {
                await decorated.AddOrUpdateAsync(stock).ConfigureAwait(false);
                await FindProductAsync(stock.Product.Code).ConfigureAwait(false);

                await PostFromQueueAsync().ConfigureAwait(false);

                await GetAllAsync().ConfigureAwait(false);
            }
            catch
            {
                cache[PRODUCT_KEY + stock.Product.Code] = JsonSerializer.Serialize(stock);

                var cachedResult = cache[ALL_KEY] ?? "[]";
                var all = (JsonSerializer.Deserialize<ProductStock[]>(cachedResult) ?? Enumerable.Empty<ProductStock>()).ToList();

                var existing = all.Where(it => it.Product.Code == stock.Product.Code).FirstOrDefault();
                if (existing == null)
                {
                    all.Add(stock);
                }
                else
                {
                    existing.Quantity = stock.Quantity;
                    existing.Product.Name = stock.Product.Name;
                    existing.Product.Unit = stock.Product.Unit;
                    existing.Product.UnitPrice = stock.Product.UnitPrice;
                }

                cache[ALL_KEY] = JsonSerializer.Serialize(all);

                queue.Enqueue(stock);
            }
        }

        //

        private const string ALL_KEY = "ALL";
        private const string PRODUCT_KEY = "PRODUCT:";

        private readonly Queue<ProductStock> queue = new();

        private readonly IStock decorated;
        private readonly ICache cache;

        private async Task PostFromQueueAsync()
        {
            while (queue.Any())
            {
                var item = queue.Dequeue();
                try
                {
                    await decorated.AddOrUpdateAsync(item).ConfigureAwait(false);
                    await FindProductAsync(item.Product.Code).ConfigureAwait(false);
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