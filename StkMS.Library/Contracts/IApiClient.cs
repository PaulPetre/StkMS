using StkMS.Library.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StkMS.Library.Contracts
{
    public interface IApiClient
    {
        ValueTask<IEnumerable<ProductStock>> GetAllAsync();
        ValueTask<IEnumerable<Customer>> GetAllCustomerAsync();
        ValueTask<ProductStock?> FindStockAsync(string productCode);
        ValueTask<Product?> FindProductAsync(string productCode);
        ValueTask<Customer> CreateCustomerAsync(Customer customer);
        ValueTask<Sale?> GetLastCompleteSaleAsync();

        Task AddOrUpdateAsync(ProductStock stock);
        Task SellProductAsync(ProductSale productSale);
        Task CompleteSaleAsync();
    }
}
