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

        public SalesViewModel MapToSalesViewModel(Product product) => new()
        {
            Code = product.Code,
            Name = product.Name,
            Unit = product.Unit,
            UnitPrice = product.UnitPrice,
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
    }
}