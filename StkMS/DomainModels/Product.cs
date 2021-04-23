namespace StkMS.DomainModels
{
    public class Product
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Unit { get; set; } = "";
        public decimal UnitPrice { get; set; }
    }
}