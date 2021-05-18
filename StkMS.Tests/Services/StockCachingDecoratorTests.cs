using System;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("ReSharper", "PrivateFieldCanBeConvertedToLocalVariable")]
    public class StockCachingDecoratorTests
    {
        private readonly Mock<IStock> decorated;
        private readonly Mock<ICache> cache;

        private readonly StockCachingDecorator sut;

        public StockCachingDecoratorTests()
        {
            decorated = new Mock<IStock>();
            cache = new Mock<ICache>();

            sut = new StockCachingDecorator(decorated.Object, cache.Object);
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
        }
    }
}