using StkMS.Library;
using StkMS.Library.Contracts;
using StkMS.Library.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace StkMS.Services
{
    public class ApiClient : IApiClient
    {
        public async ValueTask<IEnumerable<ProductStock>> GetAllAsync()
        {
            var response = await GetAsync("getAll").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<ProductStock>>().ConfigureAwait(false);
            if (result == null)
                throw new Exception("Could not deserialize the result of the /getAll API.");

            return result;
        }

        public async ValueTask<IEnumerable<Customer>> GetAllCustomerAsync()
        {
            var response = await GetAsync("getCustomers").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<Customer>>().ConfigureAwait(false);
            if (result == null)
                throw new Exception("Could nots deserialize the result of the /getCustomers API.");

            return result;
        }

        public async Task AddOrUpdateAsync(ProductStock stock)
        {
            var response = await PostAsync("addOrUpdate", stock).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task AddOrUpdateCustomerAsync(Customer customer)
        {
            var response = await PostAsync("addOrUpdateCostumer", customer).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async ValueTask<ProductStock?> FindStockAsync(string productCode)
        {
            var response = await GetAsync("findStock/" + productCode).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductStock?>().ConfigureAwait(false);
        }

        public async ValueTask<Product?> FindProductAsync(string productCode)
        {
            var response = await GetAsync("findProduct/" + productCode).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Product?>().ConfigureAwait(false);
        }

        public ValueTask<Customer> CreateCustomerAsync(Customer customer) => throw new NotImplementedException();

        public async ValueTask<Sale?> GetLastCompleteSaleAsync()
        {
            var response = await GetAsync("getLastCompleteSale").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Sale?>().ConfigureAwait(false);
        }

        public async ValueTask<Inventory?> GetMostRecentInventoryAsync()
        {
            var response = await GetAsync("getMostRecentInventory").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Inventory?>().ConfigureAwait(false);
        }

        public async Task RegisterInventoryAsync(ProductStock stock)
        {
            var response = await PostAsync("registerInventory", stock).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task SellProductAsync(ProductSale productSale)
        {
            var response = await PostAsync("sell", productSale).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task CompleteSaleAsync()
        {
            var response = await PostAsync<object>("completeSale", null!).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task BeginInventoryAsync()
        {
            var response = await PostAsync<object>("beginInventory", null!).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task CompleteInventoryAsync()
        {
            var response = await PostAsync<object>("completeInventory", null!).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProductAsync(string productCode)
        {
            var response = await PostAsync("deleteProduct", new Product { Code = productCode }).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCustomerAsync(string customerCui)
        {
            var response = await PostAsync("deleteCostumer", new Customer { CUI = customerCui }).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();//todo: implementeaza deleteCustomer
        }

        public ValueTask<ProductStock?> FindCustomerAsync(string cui)
        {
            throw new NotImplementedException(); //todo si asta de implementat
        }

        //

        private static readonly HttpClient HTTP = new();

        private static Task<HttpResponseMessage> GetAsync(string method) => HTTP.GetAsync(Constants.API_BASE_URL + "/" + method);

        private static Task<HttpResponseMessage> PostAsync<T>(string method, T value) => HTTP.PostAsJsonAsync(Constants.API_BASE_URL + "/" + method, value);


    }
}
