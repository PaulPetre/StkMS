using Microsoft.EntityFrameworkCore;
using StkMS.Data.Contracts;
using StkMS.Data.Models;
using StkMS.Library.Contracts;
using StkMS.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Product = StkMS.Library.Models.Product;
using Sale = StkMS.Library.Models.Sale;

namespace StkMS.Data.Services
{
    internal class ProductStockStorage : IRepository
    {
        public ProductStockStorage(IStkMSContext context, IDataMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IEnumerable<ProductStock> GetStock() => context
            .Stocks
            .Include(it => it.Product)
            .AsEnumerable()
            .Select(mapper.MapStockToDomain)
            .ToArray()!;

        public ProductStock? FindStockByProductCode(string productCode) => context
            .Stocks
            .Include(it => it.Product)
            .Where(it => it.Product.Code == productCode)
            .AsEnumerable()
            .Select(mapper.MapStockToDomain)
            .FirstOrDefault();

        public Product? FindProductByCode(string productCode) => context
            .Products
            .Where(it => it.Code == productCode)
            .AsEnumerable()
            .Select(mapper.MapProductToDomain)
            .FirstOrDefault();

        public IEnumerable<Customer> GetAllCustomers() => context
            .Customers
            .AsEnumerable()
            .Select(mapper.MapCustomers)
            .ToArray();

        public Sale? GetLastCompleteSale()
        {
            var sale = context
                .Sales
                .Include(it => it.SaleItems)
                .ThenInclude(it => it.Product)
                .Where(it => it.IsComplete)
                .OrderByDescending(it => it.DateTime)
                .FirstOrDefault();
            return sale == null ? null : mapper.MapSaleToDomain(sale);
        }

        public void UpdateStock(ProductStock stock)
        {
            var existing = InternalFindStockByProductCode(stock.ProductCode);
            if (existing != null)
            {
                existing.Quantity = stock.Quantity;
                context.Stocks.Update(existing);
                context.SaveChanges();
                return;
            }

            var product = InternalFindProductByCode(stock.ProductCode) ?? mapper.MapProductToData(stock.Product);
            context.Products.Update(product);
            context.SaveChanges();

            context.Stocks.Add(mapper.MapStockToData(stock, product.Id));
            context.SaveChanges();
        }

        public void AddSale(ProductSale productSale)
        {
            if (productSale is null)
                throw new ArgumentNullException(nameof(productSale));

            var product = InternalFindProductByCode(productSale.ProductCode);
            if (product == null)
                throw new KeyNotFoundException("Could not find the product with the given code.");

            var currentSale = GetOrCreateCurrentSale();
            context.Sales.Update(currentSale);
            context.SaveChanges();

            var item = mapper.MapSaleItemToData(productSale, currentSale.Id, product.Id);
            context.SaleItems.Add(item);
            context.SaveChanges();
        }

        public int CompleteSale()
        {
            var currentSale = GetCurrentSale();
            if (currentSale == null)
                return 0;

            currentSale.IsComplete = true;
            currentSale.DateTime = DateTime.Now;
            context.Sales.Update(currentSale);
            context.SaveChanges();

            return currentSale.Id;
        }

        //

        private readonly IStkMSContext context;
        private readonly IDataMapper mapper;

        private Stock? InternalFindStockByProductCode(string productCode) => context
            .Stocks
            .Include(it => it.Product)
            .Where(it => it.Product.Code == productCode)
            .FirstOrDefault();

        private Models.Product? InternalFindProductByCode(string productCode) => context
            .Products
            .Where(it => it.Code == productCode)
            .FirstOrDefault();

        private Models.Sale GetOrCreateCurrentSale() => GetCurrentSale() ?? new Models.Sale { DateTime = DateTime.Now };

        private Models.Sale? GetCurrentSale() => context
            .Sales
            .Where(it => !it.IsComplete)
            .OrderBy(it => it.Id)
            .FirstOrDefault();
    }
}
