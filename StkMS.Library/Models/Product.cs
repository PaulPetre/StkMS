namespace StkMS.Library.Models
{
    public class Product
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Unit { get; set; } = "";
        public decimal UnitPrice { get; set; }

        public void CopyFrom(Product other)
        {
            Code = other.Code;
            Name = other.Name;
            Unit = other.Unit;
            UnitPrice = other.UnitPrice;
        }
    }
}