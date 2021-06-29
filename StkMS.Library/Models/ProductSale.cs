namespace StkMS.Library.Models
{
    public class ProductSale
    {
        public string ProductCode { get; }
        public decimal Quantity { get; }

        public ProductSale(string productCode, decimal quantity)
        {
            ProductCode = productCode;
            Quantity = quantity;
        }
    }
}