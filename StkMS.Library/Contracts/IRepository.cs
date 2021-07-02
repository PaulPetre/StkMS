using System.Collections.Generic;
using StkMS.Library.Models;

namespace StkMS.Library.Contracts
{
    public interface IRepository
    {
        IEnumerable<ProductStock> GetStock();
        ProductStock? FindStockByProductCode(string productCode);
        Product? FindProductByCode(string productCode);
        IEnumerable<Customer> GetAllCustomers();
        Sale? GetLastCompleteSale();

        void UpdateStock(ProductStock stock);
        void AddSale(ProductSale productSale);
        int CompleteSale();

        /// <summary>
        ///     If an inventory is already in progress, returns its key; otherwise begins a new one and returns the new key.
        /// </summary>
        /// <returns>The key (Id) of the current inventory.</returns>
        int BeginInventory();

        /// <summary>
        ///     Completes the current inventory and returns its key; if no inventory is in progress, returns 0.
        /// </summary>
        /// <returns>The key (Id) of the completed inventory or 0.</returns>
        int CompleteInventory();

        /// <summary>
        ///     Add an inventory item record with the old values from the existing product and new values from the argument.
        /// </summary>
        /// <param name="stock">The new price and quantity.</param>
        void RegisterInventory(ProductStock stock);
    }
}