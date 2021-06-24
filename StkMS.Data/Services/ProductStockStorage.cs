using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StkMS.Data.Contracts;
using StkMS.Data.Models;
using StkMS.Library.Contracts;
using StkMS.Library.Models;
using Product = StkMS.Data.Models.Product;

namespace StkMS.Data.Services
{
    public class ProductStockStorage : IStorage<ProductStock>
    {
        public ProductStockStorage(IStkMSContext context, IDataMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IEnumerable<ProductStock> GetAll() => context
            .Stocks
            .Include(it => it.Product)
            .AsEnumerable()
            .Select(mapper.MapStockToDomain)
            .ToArray()!;

        public ProductStock? this[string key]
        {
            get => mapper.MapStockToDomain(FindStockByProductCode(key));
            set
            {
                var stock = FindStockByProductCode(key);
                if (stock != null)
                {
                    stock.Quantity = value?.Quantity ?? 0m;
                    context.Stocks.Update(stock);
                    context.SaveChanges();
                    return;
                }

                if (value == null)
                    return;

                var product = FindProductByCode(key) ?? mapper.MapProductToData(value.Product);
                context.Products.Update(product);
                context.SaveChanges();

                context.Stocks.Add(mapper.MapStockToData(value, product.Id));
                context.SaveChanges();
            }
        }

        //

        private readonly IStkMSContext context;
        private readonly IDataMapper mapper;

        private Stock? FindStockByProductCode(string productCode) => context
            .Stocks
            .Include(it => it.Product)
            .Where(it => it.Product.Code == productCode)
            .FirstOrDefault();

        private Product? FindProductByCode(string productCode) => context
            .Products
            .Where(it => it.Code == productCode)
            .FirstOrDefault();
    }
}