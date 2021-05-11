using System.Collections.Generic;
using StkMS.Library.Models;

namespace StkMS.Contracts
{
    public interface IStock
    {
        IEnumerable<ProductStock> GetAll();

        ProductStock? FindProduct(string productCode);
        void AddOrUpdate(ProductStock stock);
    }
}