#nullable disable

namespace StkMS.Library.Models
{
    public class ProductSaleDetails
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductUnit { get; set; }
        public decimal ProductUnitPrice { get; set; }
        public decimal Quantity { get; set; }

        public decimal Value => Quantity * ProductUnitPrice;
    }
}