using System;
using System.Linq;

namespace StkMS.Library.Models
{
    public class Sale
    {
        public int Id { get; }
        public DateTime DateTime { get; }
        public ProductSale[] Items { get; }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local
        public Sale(int id, DateTime dateTime, ProductSale[] items)
        {
            Id = id;
            DateTime = dateTime;
            Items = items.ToArray();
        }
    }
}