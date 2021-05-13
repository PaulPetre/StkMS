using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using StkMS.Contracts;
using StkMS.Library;
using StkMS.Library.Models;

namespace StkMS.Services
{
    public class Stock : IStock
    {
        public async ValueTask<IEnumerable<ProductStock>> GetAllAsync()
        {
            try
            {
                var response = await HTTP.GetAsync(Constants.API_BASE_URL + "getAll").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<IEnumerable<ProductStock>>().ConfigureAwait(false) ?? Enumerable.Empty<ProductStock>();

                return Enumerable.Empty<ProductStock>();
            }
            catch
            {
                return Enumerable.Empty<ProductStock>();
            }
        }

        public async ValueTask<ProductStock?> FindProductAsync(string productCode)
        {
            try
            {
                var response = await HTTP.GetAsync(Constants.API_BASE_URL + "findProduct/" + productCode).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadFromJsonAsync<ProductStock?>().ConfigureAwait(false);

                return null;
            }
            catch
            {
                return null;
            }
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