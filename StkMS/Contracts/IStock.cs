using System.Collections.Generic;
using StkMS.DomainModels;

namespace StkMS.Contracts
{
    public interface IStock
    {
        IEnumerable<ProductStock> GetAll();

        ProductStock? FindProduct(string productCode);
        void AddOrUpdate(ProductStock stock);
    }
}