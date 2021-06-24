using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StkMS.Library.Contracts;
using StkMS.Library.Models;

namespace StkMS.Api.Controllers
{
    [ApiController]
    [EnableCors]
    public class StockController : ControllerBase
    {
        public StockController(IStorage<ProductStock> storage)
        {
            this.storage = storage;
        }

        [HttpGet("~/getAll")]
        public IEnumerable<ProductStock> GetAll() => storage.GetAll();

        [HttpGet("~/findStock/{productCode?}")]
        public ProductStock? FindStock(string? productCode)
        {
            if (productCode is null)
                throw new ArgumentNullException(nameof(productCode));

            return storage[productCode];
        }

        [HttpGet("~/findProduct/{productCode?}")]
        public Product? FindProduct(string? productCode)
        {
            if (productCode is null)
                throw new ArgumentNullException(nameof(productCode));

            return storage[productCode]?.Product;
        }

        [HttpPost("~/addOrUpdate")]
        public ProductStock AddOrUpdate([FromBody] ProductStock stock)
        {
            if (stock is null)
                throw new ArgumentNullException(nameof(stock));

            storage[stock.ProductCode] = stock;
            return stock;
        }

        //

        private readonly IStorage<ProductStock> storage;
    }
}