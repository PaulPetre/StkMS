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

        Task AddOrUpdateAsync(ProductStock stock);
        Task SellProductAsync(Sale sale);
        Task CompleteSaleAsync();
        Task<Customer> CreateCustomer(Customer customer);
    }
}
