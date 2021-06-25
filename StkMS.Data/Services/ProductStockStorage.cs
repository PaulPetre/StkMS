using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StkMS.Data.Contracts;
using StkMS.Data.Models;
using StkMS.Library.Contracts;
using StkMS.Library.Models;
using Product = StkMS.Library.Models.Product;

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
    }
}