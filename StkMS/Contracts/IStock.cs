using System.Collections.Generic;
using System.Threading.Tasks;
using StkMS.Library.Models;

namespace StkMS.Contracts
{
    public interface IStock
    {
        ValueTask<IEnumerable<ProductStock>> GetAllAsync();
        ValueTask<ProductStock?> FindProductAsync(string productCode);

        Task AddOrUpdateAsync(ProductStock stock);
    }
}