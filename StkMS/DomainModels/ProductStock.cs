namespace StkMS.DomainModels
{
    public class ProductStock
    {
        public Product Product { get; set; } = new();
        public decimal Quantity { get; set; }
    }
}