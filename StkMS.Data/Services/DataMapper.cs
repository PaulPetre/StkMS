using StkMS.Data.Contracts;
using StkMS.Data.Models;
using StkMS.Library.Models;
using Product = StkMS.Data.Models.Product;

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

        public Stock MapStockToData(ProductStock stock, int productId) => new()
        {
            ProductId = productId,
            Quantity = stock.Quantity,
        };

        public Product MapProductToData(Library.Models.Product product) => new()
        {
            Code = product.Code,
            Name = product.Name,
            Unit = product.Unit,
            UnitPrice = product.UnitPrice,
        };

        //

        private static Library.Models.Product MapProductToDomain(Product product) => new()
        {
            Code = product.Code,
            Name = product.Name,
            Unit = product.Unit,
            UnitPrice = product.UnitPrice,
        };
    }
}