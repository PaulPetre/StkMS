using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StkMS.Library.Contracts;
using StkMS.Library.Models;

namespace StkMS.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors]
    public class StockController : ControllerBase
    {
        public StockController(IStorage<ProductStock> storage)
        {
            this.storage = storage;
        }

        [HttpGet("~/getAll")]
        public IEnumerable<ProductStock> GetAll() => //storage.GetAll();
            new[] { new ProductStock { Product = new Product { Code = "123", Name = "abc", Unit = "kg", UnitPrice = 12.34m }, Quantity = 56.78m } };

        [HttpGet("~/findProduct")]
        public ProductStock? FindProduct(string productCode)
        {
            if (productCode is null)
                throw new ArgumentNullException(nameof(productCode));

            return storage[productCode];
        }

        [HttpPost("~/addOrUpdate")]
        public ProductStock AddOrUpdate(ProductStock stock)
        {
            if (stock is null)
                throw new ArgumentNullException(nameof(stock));

            storage[stock.Product.Code] = stock;
            return stock;
        }

        //

        private readonly IStorage<ProductStock> storage;
    }
}