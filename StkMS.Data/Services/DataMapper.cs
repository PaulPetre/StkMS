using System.Linq;
using StkMS.Data.Contracts;
using StkMS.Data.Models;
using StkMS.Library.Models;
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

        public Sale MapSaleToDomain(Models.Sale sale) => new(sale.Id, sale.DateTime, sale.SaleItems.Select(MapSaleItemToDomain).ToArray());

        private static ProductSaleDetails MapSaleItemToDomain(SaleItem item) => new()
        {
            ProductCode = item.Product.Code,
            ProductName = item.Product.Name,
            ProductUnit = item.Product.Unit,
            ProductUnitPrice = item.Product.UnitPrice,
            Quantity = item.Quantity,
        };

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
        public Customer MapCustomers(Customers model) => model == null
            ? null
            : new Customer
            {
                CustomerId = model.CustomerId,
                Name = model.Name,
                Address = model.Address,
                Description = model.Description,
                City = model.City,
            };

        //

        private static Product MapProductToDomain(Models.Product product) => new()
        {
            Code = product.Code,
            Name = product.Name,
            Unit = product.Unit,
            UnitPrice = product.UnitPrice,
        };
    }
}
