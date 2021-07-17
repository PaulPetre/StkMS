using StkMS.Data.Contracts;
using StkMS.Data.Models;
using StkMS.Library.Models;
using System.Linq;
using Inventory = StkMS.Library.Models.Inventory;
using Product = StkMS.Library.Models.Product;
using Sale = StkMS.Library.Models.Sale;

namespace StkMS.Data.Services
{
    internal class DataMapper : IDataMapper
    {
        public ProductStock? MapStockToDomain(Stock? stock) => stock == null
            ? null
            : new ProductStock
            {
                Product = MapProductToDomain(stock.Product),
                Quantity = stock.Quantity,
            };

        Product? IDataMapper.MapProductToDomain(Models.Product? product) => product == null
            ? null
            : new Product
            {
                Code = product.Code,
                Name = product.Name,
                Unit = product.Unit,
                UnitPrice = product.UnitPrice,
            };

        public Sale MapSaleToDomain(Models.Sale sale) => new(sale.Id, sale.DateTime, sale.Items.Select(MapSaleItemToDomain).ToArray());

        public Inventory MapInventoryToDomain(Models.Inventory inventory) =>
            new(inventory.StartDate, inventory.EndDate, inventory.Items.Select(MapInventoryItemToDomain).ToArray());

        public Stock MapStockToData(ProductStock stock, int productId) => new()
        {
            ProductId = productId,
            Quantity = stock.Quantity,
        };

        public Models.Product MapProductToData(Product product) => new()
        {
            Code = product.Code,
            Name = product.Name,
            Unit = product.Unit,
            UnitPrice = product.UnitPrice,
        };

        public SaleItem MapSaleItemToData(ProductSale productSale, int saleId, int productId) => new()
        {
            SaleId = saleId,
            ProductId = productId,
            Quantity = productSale.Quantity,
        };

        public Customer MapCustomers(Customers model) => new()
        {
            CustomerId = model.CustomerId,
            Name = model.Name,
            Address = model.Address,
            CUI = model.CUI,
            City = model.City,
            Phone = model.Phone,
            Email = model.Email,
        };

        //

        private static Product MapProductToDomain(Models.Product product) => new()
        {
            Code = product.Code,
            Name = product.Name,
            Unit = product.Unit,
            UnitPrice = product.UnitPrice,
        };

        private static ProductSaleDetails MapSaleItemToDomain(SaleItem item) => new()
        {
            ProductCode = item.Product.Code,
            ProductName = item.Product.Name,
            ProductUnit = item.Product.Unit,
            ProductUnitPrice = item.Product.UnitPrice,
            Quantity = item.Quantity,
        };

        private static InventoryDetails MapInventoryItemToDomain(InventoryItem item) =>
            new(item.Product.Code, item.Product.Name, item.Product.Unit, item.OldPrice, item.NewPrice, item.OldQuantity, item.NewQuantity);
    }
}
