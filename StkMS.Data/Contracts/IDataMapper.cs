using StkMS.Data.Models;
using StkMS.Library.Models;
using Product = StkMS.Library.Models.Product;
using Sale = StkMS.Library.Models.Sale;

namespace StkMS.Data.Contracts
{
    internal interface IDataMapper
    {
        ProductStock? MapStockToDomain(Stock? stock);
        Product? MapProductToDomain(Models.Product? product);
        Sale MapSaleToDomain(Models.Sale sale);

        Stock MapStockToData(ProductStock stock, int productId);
        Models.Product MapProductToData(Product product);
        SaleItem MapSaleItemToData(ProductSale productSale, int saleId, int productId);
        Customer MapCustomers(Customers model);
    }
}