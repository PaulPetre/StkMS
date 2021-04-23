namespace StkMS.ViewModels
{
    public class StockViewModel
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Unit { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}