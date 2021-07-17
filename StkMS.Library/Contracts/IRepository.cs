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
        Sale? GetLastCompleteSale();

        Inventory? GetMostRecentInventory();

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

        /// <summary>
        ///     Deletes the product with the given code.
        /// </summary>
        /// <param name="productCode">The product code.</param>
        /// <returns>The Id of the deleted product or 0 if it was not found.</returns>
        int DeleteProduct(string productCode);

        Customer? FindCostumerByCUI(string cui);
    }
}
