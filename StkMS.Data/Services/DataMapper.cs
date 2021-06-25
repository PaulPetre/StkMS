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

        public SaleItem MapSaleItemToData(Sale sale, int saleId, int productId) => new()
        {
            SaleId = saleId,
            ProductId = productId,
            Quantity = sale.Quantity,
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