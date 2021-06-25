using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoBogus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StkMS.Library.Contracts;
using StkMS.Library.Models;
using StkMS.Library.Services;

namespace StkMS.Tests.Services
{
    [TestClass]
    public class StockCachingDecoratorTests
    {
        private readonly Mock<IApiClient> decorated;
        private readonly Mock<ICache> cache;

        private readonly ApiClientCachingDecorator sut;

        public StockCachingDecoratorTests()
        {
            decorated = new Mock<IApiClient>();
            cache = new Mock<ICache>();

            sut = new ApiClientCachingDecorator(decorated.Object, cache.Object);
        }

        [TestClass]
        public class GetAllAsync : StockCachingDecoratorTests
        {
            //
        }

        [TestClass]
        public class FindProductAsync : StockCachingDecoratorTests
        {
            //
        }

        [TestClass]
        public class AddOrUpdateAsync : StockCachingDecoratorTests
        {
            [TestMethod("Calls the decorated method")]
            public async Task Test1()
            {
                var productStock = AutoFaker.Generate<ProductStock>();

                await sut.AddOrUpdateAsync(productStock).ConfigureAwait(false);

                decorated.Verify(it => it.AddOrUpdateAsync(productStock));
            }

            [TestMethod("Calls the decorated method for all enqueued items")]
            public async Task Test2()
            {
                var productStocks = AutoFaker.Generate<ProductStock>(4);
                decorated.Setup(it => it.AddOrUpdateAsync(It.IsAny<ProductStock>())).Throws<Exception>();
                await sut.AddOrUpdateAsync(productStocks[0]).ConfigureAwait(false);
                await sut.AddOrUpdateAsync(productStocks[1]).ConfigureAwait(false);
                await sut.AddOrUpdateAsync(productStocks[2]).ConfigureAwait(false);
                decorated.Setup(it => it.AddOrUpdateAsync(It.IsAny<ProductStock>())).Returns(Task.CompletedTask);

                await sut.AddOrUpdateAsync(productStocks[3]).ConfigureAwait(false);

                decorated.Verify(it => it.AddOrUpdateAsync(productStocks[3]));
                decorated.Verify(it => it.AddOrUpdateAsync(productStocks[0]), Times.Exactly(2));
                decorated.Verify(it => it.AddOrUpdateAsync(productStocks[1]), Times.Exactly(2));
                decorated.Verify(it => it.AddOrUpdateAsync(productStocks[2]), Times.Exactly(2));
            }

            [TestMethod("Calls FindStockAsync for the given product")]
            public async Task Test3A()
            {
                var productStock = AutoFaker.Generate<ProductStock>();

                await sut.AddOrUpdateAsync(productStock).ConfigureAwait(false);

                decorated.Verify(it => it.FindStockAsync(productStock.ProductCode));
            }

            [TestMethod("Does not call FindStockAsync if there's an error")]
            public async Task Test3B()
            {
                var productStock = AutoFaker.Generate<ProductStock>();
                decorated.Setup(it => it.AddOrUpdateAsync(It.IsAny<ProductStock>())).Throws<Exception>();

                await sut.AddOrUpdateAsync(productStock).ConfigureAwait(false);

                decorated.Verify(it => it.FindStockAsync(It.IsAny<string>()), Times.Never);
            }

            [TestMethod("Calls FindStockAsync for the enqueued items")]
            public async Task Test4()
            {
                var productStocks = AutoFaker.Generate<ProductStock>(2);
                decorated.Setup(it => it.AddOrUpdateAsync(It.IsAny<ProductStock>())).Throws<Exception>();
                await sut.AddOrUpdateAsync(productStocks[0]).ConfigureAwait(false);
                decorated.Setup(it => it.AddOrUpdateAsync(It.IsAny<ProductStock>())).Returns(Task.CompletedTask);

                await sut.AddOrUpdateAsync(productStocks[1]).ConfigureAwait(false);

                decorated.Verify(it => it.FindStockAsync(productStocks[0].ProductCode));
                decorated.Verify(it => it.FindStockAsync(productStocks[1].ProductCode));
            }

            [TestMethod("Calls GetAll")]
            public async Task Test5A()
            {
                var productStock = AutoFaker.Generate<ProductStock>();

                await sut.AddOrUpdateAsync(productStock).ConfigureAwait(false);

                decorated.Verify(it => it.GetAllAsync());
            }

            [TestMethod("Does not call GetAll if there's an error")]
            public async Task Test5B()
            {
                var productStock = AutoFaker.Generate<ProductStock>();
                decorated.Setup(it => it.AddOrUpdateAsync(It.IsAny<ProductStock>())).Throws<Exception>();

                await sut.AddOrUpdateAsync(productStock).ConfigureAwait(false);

                decorated.Verify(it => it.GetAllAsync(), Times.Never);
            }

            [TestMethod("Updates the cache for the given stock if there's an error")]
            public async Task Test6()
            {
                var productStock = AutoFaker.Generate<ProductStock>();
                decorated.Setup(it => it.AddOrUpdateAsync(It.IsAny<ProductStock>())).Throws<Exception>();

                await sut.AddOrUpdateAsync(productStock).ConfigureAwait(false);

                cache.VerifySet(it => it["STOCK:" + productStock.ProductCode] = JsonSerializer.Serialize(productStock));
            }

            [TestMethod("Adds a new product to the overall cache if there's an error")]
            public async Task Test7()
            {
                var productStocks = AutoFaker.Generate<ProductStock>(3);
                decorated.Setup(it => it.AddOrUpdateAsync(It.IsAny<ProductStock>())).Throws<Exception>();
                cache.Setup(it => it["ALL"]).Returns(JsonSerializer.Serialize(productStocks.AsEnumerable()));
                var newProductStock = AutoFaker.Generate<ProductStock>();
                var newProductStocks = productStocks.Concat(new[] { newProductStock }).ToList();

                await sut.AddOrUpdateAsync(newProductStock).ConfigureAwait(false);

                cache.VerifySet(it => it["ALL"] = JsonSerializer.Serialize(newProductStocks));
            }

            [TestMethod("Updates an existing product in the overall cache if there's an error")]
            public async Task Test8()
            {
                var productStocks = AutoFaker.Generate<ProductStock>(3);
                decorated.Setup(it => it.AddOrUpdateAsync(It.IsAny<ProductStock>())).Throws<Exception>();
                cache.Setup(it => it["ALL"]).Returns(JsonSerializer.Serialize(productStocks.AsEnumerable()));
                var newProductStocks = new List<ProductStock>(productStocks);
                productStocks[1].Quantity = AutoFaker.Generate<decimal>();
                productStocks[1].Product.UnitPrice = AutoFaker.Generate<decimal>();

                await sut.AddOrUpdateAsync(productStocks[1]).ConfigureAwait(false);

                cache.VerifySet(it => it["ALL"] = JsonSerializer.Serialize(newProductStocks));
            }
        }
    }
}