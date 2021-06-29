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

        [HttpGet("~/getCustomers")]
        public IEnumerable<Customer> GetAllCustomers() => repository.GetAllCustomers();

        [HttpGet("~/getLastCompleteSale")]
        public Sale? GetLastCompleteSale() => repository.GetLastCompleteSale();

        [HttpPost("~/addOrUpdate")]
        public ProductStock AddOrUpdate([FromBody] ProductStock stock)
        {
            if (stock is null)
                throw new ArgumentNullException(nameof(stock));

            repository.UpdateStock(stock);
            return stock;
        }

        [HttpPost("~/sell")]
        public ProductStock Sell([FromBody] ProductSale productSale)
        {
            if (productSale is null)
                throw new ArgumentNullException(nameof(productSale));

            var stock = repository.FindStockByProductCode(productSale.ProductCode);
            if (stock == null)
                throw new KeyNotFoundException("The product with the given code does not exist.");

            if (stock.Quantity < productSale.Quantity)
                throw new InvalidOperationException("Cannot sell more than the available quantity.");

            stock.Quantity -= productSale.Quantity;
            repository.UpdateStock(stock);
            repository.AddSale(productSale);

            return stock;
        }

        [HttpPost("~/completeSale")]
        public int CompleteSale() => repository.CompleteSale();

        //

        private readonly IRepository repository;
    }
}