using StkMS.Library.Models;
using System.Collections.Generic;

namespace StkMS.Library.Contracts
{
    public interface IRepository
    {
        IEnumerable<ProductStock> GetStock();
        ProductStock? FindStockByProductCode(string productCode);
        Product? FindProductByCode(string productCode);

        IEnumerable<Customer> GetAllCustomers();

        void UpdateStock(ProductStock stock);
        void AddSale(Sale sale);
        int CompleteSale();
    }
}
