using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using StkMS.Library.Contracts;
using StkMS.Library.Models;

namespace StkMS.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StockController : ControllerBase
    {
        public StockController(IStorage<ProductStock> storage)
        {
            this.storage = storage;
        }

        [HttpGet]
        public IEnumerable<ProductStock> GetAll() => storage.GetAll();

        [HttpGet]
        public ProductStock? FindProduct(string productCode)
        {
            if (productCode is null)
                throw new ArgumentNullException(nameof(productCode));

            return storage[productCode];
        }

        [HttpPost]
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