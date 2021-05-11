using StkMS.Library.Models;
using StkMS.ViewModels;

namespace StkMS.Contracts
{
    public interface IMapper
    {
        StockViewModel MapToViewModel(ProductStock model);
        ProductStock MapToDomain(StockViewModel model);
    }
}