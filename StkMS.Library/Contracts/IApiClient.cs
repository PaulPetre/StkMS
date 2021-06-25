using System.Collections.Generic;
using System.Threading.Tasks;
using StkMS.Library.Models;

namespace StkMS.Library.Contracts
{
    public interface IApiClient
    {
        ValueTask<IEnumerable<ProductStock>> GetAllAsync();
        ValueTask<ProductStock?> FindStockAsync(string productCode);
        ValueTask<Product?> FindProductAsync(string productCode);

        Task AddOrUpdateAsync(ProductStock stock);
        Task SellProductAsync(Sale sale);
        Task CompleteSaleAsync();
    }
}