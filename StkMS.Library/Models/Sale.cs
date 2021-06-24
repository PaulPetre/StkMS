namespace StkMS.Library.Models
{
    public class Sale
    {
        public string ProductCode { get; }
        public decimal Quantity { get; }

        public Sale(string productCode, decimal quantity)
        {
            ProductCode = productCode;
            Quantity = quantity;
        }
    }
}