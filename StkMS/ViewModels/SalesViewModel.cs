namespace StkMS.ViewModels
{
    public class SalesViewModel
    {
        public static SalesViewModel CreateInvalid(string code) => new()
        {
            Code = code,
            Name = "INVALID PRODUCT CODE",
            Unit = "N/A",
            UnitPrice = 0m,
            Quantity = 0m,
        };

        //

        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public string Unit { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}