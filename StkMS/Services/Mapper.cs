using System.Linq;
using StkMS.Contracts;
using StkMS.Library.Models;
using StkMS.ViewModels;

namespace StkMS.Services
{
    public class Mapper : IMapper
    {
        public StockViewModel MapToStockViewModel(ProductStock model) => new()
        {
            Code = model.Product.Code,
            Name = model.Product.Name,
            Unit = model.Product.Unit,
            UnitPrice = model.Product.UnitPrice,
            Quantity = model.Quantity,
        };

        public SaleViewModel MapToSaleViewModel(Product product) => new()
        {
            Code = product.Code,
            Name = product.Name,
            Unit = product.Unit,
            UnitPrice = product.UnitPrice,
            Quantity = 0m,
        };

        public SaleDetailsViewModel MapToSaleDetailsViewModel(Sale sale) => new()
        {
            Id = sale.Id,
            DateTime = sale.DateTime,
            Items = sale.Items.Select(MapToSaleItem).ToArray(),
        };

        public ProductStock MapToDomain(StockViewModel model) => new()
        {
            Product = new Product
            {
                Code = model.Code,
                Name = model.Name,
                Unit = model.Unit,
                UnitPrice = model.UnitPrice,
            },
            Quantity = model.Quantity,
        };

        //

        private static SaleViewModel MapToSaleItem(ProductSaleDetails item) => new()
        {
            Code = item.ProductCode,
            Name = item.ProductName,
            Unit = item.ProductUnit,
            UnitPrice = item.ProductUnitPrice,
            Quantity = item.Quantity,
        };
    }
}