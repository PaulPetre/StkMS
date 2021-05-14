using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StkMS.Library;
using StkMS.Library.Contracts;
using StkMS.Library.Models;

namespace StkMS.Services
{
    public class Stock : IStock
    {
        public async ValueTask<IEnumerable<ProductStock>> GetAllAsync()
        {
            var response = await HTTP.GetAsync(Constants.API_BASE_URL + "getAll").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<ProductStock>>().ConfigureAwait(false);
            if (result == null)
                throw new Exception("Could not deserialize the result of the /getAll API.");

            return result;
        }

        public async ValueTask<ProductStock?> FindProductAsync(string productCode)
        {
            var response = await HTTP.GetAsync(Constants.API_BASE_URL + "findProduct/" + productCode).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductStock?>().ConfigureAwait(false);
        }

        public async Task AddOrUpdateAsync(ProductStock stock)
        {
            var response = await HTTP.PostAsJsonAsync(Constants.API_BASE_URL + "addOrUpdate", stock).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        //

        private static readonly HttpClient HTTP = new();
    }
}