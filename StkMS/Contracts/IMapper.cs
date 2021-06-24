using StkMS.Library.Models;
using StkMS.ViewModels;

namespace StkMS.Contracts
{
    public interface IMapper
    {
        StockViewModel MapToStockViewModel(ProductStock model);
        SalesViewModel MapToSalesViewModel(Product product);

        ProductStock MapToDomain(StockViewModel model);
    }
}