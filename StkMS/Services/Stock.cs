using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using StkMS.Contracts;
using StkMS.Library;
using StkMS.Library.Models;

namespace StkMS.Services
{
    public class Stock : IStock
    {
        public Stock(ISyncLocalStorageService localStorage)
        {
            this.localStorage = localStorage;

            stocks = localStorage.GetItem<List<ProductStock>>(KEY) ?? new List<ProductStock>();

            if (!stocks.Any())
                stocks.Add(new ProductStock { Product = new Product { Code = "123", Name = "Fake Product", Unit = "Kg", UnitPrice = 3.45m }, Quantity = 120.00m });
        }

        public async ValueTask<IEnumerable<ProductStock>> GetAllAsync()
        {
            var response = await HTTP.GetAsync(Constants.API_BASE_URL + "getAll").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<IEnumerable<ProductStock>>().ConfigureAwait(false) ?? Enumerable.Empty<ProductStock>();

            return Enumerable.Empty<ProductStock>();
        }

        public ProductStock? FindProduct(string productCode) => stocks.Where(it => it.Product.Code == productCode).FirstOrDefault();

        public void AddOrUpdate(ProductStock stock)
        {
            var existing = FindProduct(stock.Product.Code);
            if (existing != null)
            {
                existing.Product.Name = stock.Product.Name;
                existing.Product.Unit = stock.Product.Unit;
                existing.Product.UnitPrice = stock.Product.UnitPrice;
                existing.Quantity = stock.Quantity;
            }
            else
            {
                stocks.Add(stock);
            }

            localStorage.SetItem(KEY, stocks);
        }

        //

        private static readonly HttpClient HTTP = new();

        private const string KEY = "stocks";

        private readonly ISyncLocalStorageService localStorage;

        private readonly List<ProductStock> stocks;
    }
}