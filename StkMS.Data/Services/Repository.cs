using Microsoft.EntityFrameworkCore;
using StkMS.Data.Contracts;
using StkMS.Data.Models;
using StkMS.Library.Contracts;
using StkMS.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Inventory = StkMS.Data.Models.Inventory;
using Product = StkMS.Library.Models.Product;
using Sale = StkMS.Library.Models.Sale;

namespace StkMS.Data.Services
{
    internal class Repository : IRepository
    {
        public Repository(IStkMSContext context, IDataMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IEnumerable<ProductStock> GetStock() => context
            .Stocks
            .Include(it => it.Product)
            .Where(it => !it.Product.IsDisabled)
            .AsEnumerable()
            .Select(mapper.MapStockToDomain)
            .ToArray()!;

        public ProductStock? FindStockByProductCode(string productCode) => context
            .Stocks
            .Include(it => it.Product)
            .Where(it => it.Product.Code == productCode && !it.Product.IsDisabled)
            .AsEnumerable()
            .Select(mapper.MapStockToDomain)
            .FirstOrDefault();

        public Product? FindProductByCode(string productCode) => context
            .Products
            .Where(it => it.Code == productCode && !it.IsDisabled)
            .AsEnumerable()
            .Select(mapper.MapProductToDomain)
            .FirstOrDefault();

        public IEnumerable<Customer> GetAllCustomers() => context
            .Customers
            .AsEnumerable()
            .Select(mapper.MapCustomers)
            .ToArray()!;

        public Sale? GetLastCompleteSale()
        {
            var sale = context
                .Sales
                .Include(it => it.Items)
                .ThenInclude(it => it.Product)
                .Where(it => it.IsComplete)
                .OrderByDescending(it => it.DateTime)
                .FirstOrDefault();
            return sale == null ? null : mapper.MapSaleToDomain(sale);
        }

        public Library.Models.Inventory? GetMostRecentInventory()
        {
            var inventory = context
                .Inventory
                .Include(it => it.Items)
                .ThenInclude(it => it.Product)
                .OrderByDescending(it => it.StartDate)
                .FirstOrDefault();
            return inventory == null ? null : mapper.MapInventoryToDomain(inventory);
        }

        public void UpdateStock(ProductStock stock)
        {
            if (string.IsNullOrEmpty(stock.ProductCode))
                throw new Exception("Cannot add a product with an empty code.");

            var existing = InternalFindStockByProductCode(stock.ProductCode);
            if (existing != null)
            {
                existing.Quantity = stock.Quantity;
                context.Stocks.Update(existing);
                context.SaveChanges();

                var existingProduct = existing.Product;
                var newProduct = stock.Product;

                existingProduct.Name = newProduct.Name;
                existingProduct.Unit = newProduct.Unit;
                existingProduct.UnitPrice = newProduct.UnitPrice;
                context.Products.Update(existingProduct);
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
            if (product == null || product.IsDisabled)
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

        public int BeginInventory()
        {
            var currentInventory = GetCurrentInventory();
            if (currentInventory != null)
                return currentInventory.Id;

            var inventory = new Inventory { StartDate = DateTime.Now };
            context.Inventory.Add(inventory);
            context.SaveChanges();
            return inventory.Id;
        }

        public int CompleteInventory()
        {
            var currentInventory = GetCurrentInventory();
            if (currentInventory == null)
                return 0;

            currentInventory.EndDate = DateTime.Now;
            context.Inventory.Update(currentInventory);
            context.SaveChanges();

            return currentInventory.Id;
        }

        public void RegisterInventory(ProductStock stock)
        {
            var currentInventory = GetCurrentInventory();
            if (currentInventory == null)
                return;

            var existingStock = InternalFindStockByProductCode(stock.ProductCode);
            var product = existingStock == null ? CreateNewProduct(stock) : existingStock.Product;

            var item = new InventoryItem
            {
                InventoryId = currentInventory.Id,
                ProductId = product.Id,
                OldPrice = existingStock?.Product.UnitPrice ?? 0m,
                NewPrice = stock.Product.UnitPrice,
                OldQuantity = existingStock?.Quantity ?? 0m,
                NewQuantity = stock.Quantity,
            };
            context.InventoryItems.Add(item);
            context.SaveChanges();
        }

        public int DeleteProduct(string productCode)
        {
            var existing = InternalFindProductByCode(productCode);
            if (existing == null)
                return 0;

            existing.IsDisabled = true;
            context.Products.Update(existing);
            context.SaveChanges();

            return existing.Id;
        }

        public Customer? FindCostumerByCUI(string cui) => context
                .Customers
                .Where(it => it.CUI == cui)
                .AsEnumerable()
                .Select(mapper.MapCustomers)
                .FirstOrDefault();

        public void UpdateCustomer(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.CUI))
                throw new Exception("Cannot add a customer with an empty cui.");

            var existing = InternalFindCustomerByCUI(customer.CUI);
            if (existing != null)
            {
                // existing.Quantity = stock.Quantity;
                // context.Stocks.Update(existing);
                // context.SaveChanges();
                //
                // var existingProduct = existing.Product;
                // var newProduct = stock.Product;
                //
                // existingProduct.Name = newProduct.Name;
                // existingProduct.Unit = newProduct.Unit;
                // existingProduct.UnitPrice = newProduct.UnitPrice;
                // context.Products.Update(existingProduct);
                // context.SaveChanges();

                return;
            }

            // var product = InternalFindProductByCode(stock.ProductCode) ?? mapper.MapProductToData(stock.Product);
            // context.Products.Update(product);
            // context.SaveChanges();
            //
            // context.Stocks.Add(mapper.MapStockToData(stock, product.Id));
            context.SaveChanges();
        }

        public int DeleteCustomer(string customerCui)
        {
            var existing = InternalFindCustomerByCUI(customerCui);
            if (existing == null)
                return 0;

            //  existing.IsDisabled = true;
            context.Customers.Update(existing);
            context.SaveChanges();

            return existing.CustomerId;
        }


        //

        private readonly IStkMSContext context;
        private readonly IDataMapper mapper;

        private Stock? InternalFindStockByProductCode(string productCode) => context
            .Stocks
            .Include(it => it.Product)
            .Where(it => it.Product.Code == productCode)
            .FirstOrDefault();

        private Customers? InternalFindCustomerByCUI(string customerCui)
        {
            var dd = context.Customers.Where(it => it.CUI == customerCui).FirstOrDefault();
            return dd;
        }
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

        private Inventory? GetCurrentInventory() => context
            .Inventory
            .Where(it => it.EndDate == null)
            .FirstOrDefault();

        private Models.Product CreateNewProduct(ProductStock stock)
        {
            var product = mapper.MapProductToData(stock.Product);
            context.Products.Add(product);
            context.SaveChanges();
            return product;
        }
    }
}
