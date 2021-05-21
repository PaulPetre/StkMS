using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using StkMS.Library.Contracts;
using StkMS.Library.Models;
using StkMS.Library.Services;

namespace StkMS.SqlData.Services
{
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public class ProductStockStorage : IStorage<ProductStock>
    {
        public ProductStockStorage()
        {
            var folder = Environment.GetEnvironmentVariable(FOLDER_ENV, EnvironmentVariableTarget.Machine)
                ?? throw new Exception($"The {FOLDER_ENV} environment variable must be defined and pointing to a folder with write permissions.");
            filename = Path.Combine(folder, "productStock.csv");
        }

        /// <inheritdoc />
        public IEnumerable<ProductStock> GetAll()
        {
            lock (GATE)
            {
                return ReadAllLines().Select(ToProductStock).ToArray();
            }
        }

        /// <inheritdoc />
        public ProductStock? this[string key]
        {
            get
            {
                lock (GATE)
                {
                    return FindByProductCode(GetAll(), key);
                }
            }
            set
            {
                lock (GATE)
                {
                    var all = GetAll().ToList();

                    var stock = FindByProductCode(all, key) ?? AddNewProduct(all);

                    if (value is { })
                        stock.CopyFrom(value);
                    else
                        all.Remove(stock);

                    File.WriteAllLines(filename, all.Select(ToDataString).ToArray());
                }
            }
        }

        //

        private static readonly object GATE = new();

        private const string FOLDER_ENV = "HOME";

        private readonly string filename;

        private IEnumerable<string> ReadAllLines()
        {
            try
            {
                return File.ReadAllLines(filename);
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        private static ProductStock ToProductStock(string line)
        {
            var parts = line.Split(',');
            return new ProductStock
            {
                Product = new Product
                {
                    Code = parts[0],
                    Name = parts[1],
                    Unit = parts[2],
                    UnitPrice = parts[3].ParseDecimal(),
                },
                Quantity = parts[4].ParseDecimal(),
            };
        }

        private static string ToDataString(ProductStock stock) =>
            string.Join(",", stock.Product.Code, stock.Product.Name, stock.Product.Unit, stock.Product.UnitPrice, stock.Quantity);

        private static ProductStock? FindByProductCode(IEnumerable<ProductStock> stocks, string key) =>
            stocks.Where(it => it.ProductCode == key).FirstOrDefault();

        private static ProductStock AddNewProduct(ICollection<ProductStock> stocks)
        {
            var newStock = new ProductStock();
            stocks.Add(newStock);
            return newStock;
        }
    }
}