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
        public StockController(IRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("~/getAll")]
        public IEnumerable<ProductStock> GetAll() => repository.GetStock();

        [HttpGet("~/findStock/{productCode?}")]
        public ProductStock? FindStock(string? productCode)
        {
            if (productCode is null)
                throw new ArgumentNullException(nameof(productCode));

            return repository.FindStockByProductCode(productCode);
        }

        [HttpGet("~/findProduct/{productCode?}")]
        public Product? FindProduct(string? productCode)
        {
            if (productCode is null)
                throw new ArgumentNullException(nameof(productCode));

            return repository.FindProductByCode(productCode);
        }

        [HttpPost("~/addOrUpdate")]
        public ProductStock AddOrUpdate([FromBody] ProductStock stock)
        {
            if (stock is null)
                throw new ArgumentNullException(nameof(stock));

            repository.UpdateStock(stock);
            return stock;
        }

        [HttpPost("~/sell")]
        public ProductStock Sell([FromBody] Sale sale)
        {
            if (sale is null)
                throw new ArgumentNullException(nameof(sale));

            var stock = repository.FindStockByProductCode(sale.ProductCode);
            if (stock == null)
                throw new KeyNotFoundException("The product with the given code does not exist.");

            if (stock.Quantity < sale.Quantity)
                throw new InvalidOperationException("Cannot sell more than the available quantity.");

            stock.Quantity -= sale.Quantity;
            repository.UpdateStock(stock);
            repository.AddSale(sale);

            return stock;
        }

        //

        private readonly IRepository repository;
    }
}