using System.Collections.Generic;
using System.Threading.Tasks;
using StkMS.Library.Models;

namespace StkMS.Library.Contracts
{
    public interface IStock
    {
        ValueTask<IEnumerable<ProductStock>> GetAllAsync();
        ValueTask<ProductStock?> FindStockAsync(string productCode);
        ValueTask<Product?> FindProductAsync(string productCode);

        Task AddOrUpdateAsync(ProductStock stock);
    }
}