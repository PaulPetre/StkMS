using StkMS.Library;

namespace StkMS.ViewModels
{
    public class SaleViewModel
    {
        public static SaleViewModel CreateInvalid(string code) => new()
        {
            Code = code,
            Name = Constants.INVALID_PRODUCT_CODE,
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

        public decimal Value => UnitPrice * Quantity;

        public bool IsValid() => !string.IsNullOrEmpty(Code) && Name != Constants.INVALID_PRODUCT_CODE && Quantity > 0m;
    }
}