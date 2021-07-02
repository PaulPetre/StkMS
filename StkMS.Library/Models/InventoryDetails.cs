namespace StkMS.Library.Models
{
    public class InventoryDetails
    {
        public string Code { get; }
        public string Name { get; }
        public string Unit { get; }

        public decimal OldPrice { get; }
        public decimal NewPrice { get; }
        public decimal OldQuantity { get; }
        public decimal NewQuantity { get; }

        public InventoryDetails(Product product, decimal oldPrice, decimal newPrice, decimal oldQuantity, decimal newQuantity)
        {
            Code = product.Code;
            Name = product.Name;
            Unit = product.Unit;

            OldPrice = oldPrice;
            NewPrice = newPrice;
            OldQuantity = oldQuantity;
            NewQuantity = newQuantity;
        }
    }
}