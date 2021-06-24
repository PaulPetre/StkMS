using StkMS.Data.Models;
using StkMS.Library.Models;
using Product = StkMS.Data.Models.Product;

namespace StkMS.Data.Contracts
{
    public interface IDataMapper
    {
        ProductStock? MapStockToDomain(Stock? stock);

        Stock MapStockToData(ProductStock stock, int productId);
        Product MapProductToData(Library.Models.Product product);
    }
}