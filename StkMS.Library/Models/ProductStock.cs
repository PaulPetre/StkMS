namespace StkMS.Library.Models
{
    public class ProductStock
    {
        public Product Product { get; set; } = new();
        public decimal Quantity { get; set; }

        public string ProductCode => Product.Code;

        public string QuantityLabel
        {
            get
            {
                if (Quantity < 10)
                    return "Danger";
                if (Quantity > 100)
                    return "Too many";
                return "";
            }
        }

        public void CopyFrom(ProductStock other)
        {
            Quantity = other.Quantity;
            Product.CopyFrom(other.Product);
        }
    }
}