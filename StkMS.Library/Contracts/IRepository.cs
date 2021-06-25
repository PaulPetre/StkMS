using System.Collections.Generic;
using StkMS.Library.Models;

namespace StkMS.Library.Contracts
{
    public interface IRepository
    {
        IEnumerable<ProductStock> GetStock();
        ProductStock? FindStockByProductCode(string productCode);
        Product? FindProductByCode(string productCode);

        void UpdateStock(ProductStock stock);
        void AddSale(Sale sale);
        int CompleteSale();
    }
}