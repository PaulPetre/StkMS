using System.Collections.Generic;
using System.Linq;
using Blazored.LocalStorage;
using StkMS.Contracts;
using StkMS.DomainModels;

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

        public IEnumerable<ProductStock> GetAll() => stocks.AsEnumerable();

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

        private const string KEY = "stocks";

        private readonly ISyncLocalStorageService localStorage;

        private readonly List<ProductStock> stocks;
    }
}