using System.Collections.Generic;
using System.Threading.Tasks;
using StkMS.Library.Models;

namespace StkMS.Contracts
{
    public interface IStock
    {
        ValueTask<IEnumerable<ProductStock>> GetAllAsync();

        ProductStock? FindProduct(string productCode);
        void AddOrUpdate(ProductStock stock);
    }
}