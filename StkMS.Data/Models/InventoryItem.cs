using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StkMS.Data.Models
{
    internal class InventoryItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Inventory")]
        public int InventoryId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public decimal OldPrice { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldQuantity { get; set; }
        public decimal NewQuantity { get; set; }

        public virtual Inventory Inventory { get; set; }
        public virtual Product Product { get; set; }
    }
}