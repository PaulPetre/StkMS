using StkMS.Library.Models;
using StkMS.ViewModels;

namespace StkMS.Contracts
{
    public interface IMapper
    {
        StockViewModel MapToStockViewModel(ProductStock model);
        SaleViewModel MapToSaleViewModel(Product product);
        SaleDetailsViewModel MapToSaleDetailsViewModel(Sale sale);

        ProductStock MapToDomain(StockViewModel model);
    }
}