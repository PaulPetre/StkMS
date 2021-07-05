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
                    return "Nivel critic";
                if (Quantity > 100)
                    return "Suprastocare";
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