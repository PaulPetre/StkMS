using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StkMS.Data.Models
{
    internal class SaleItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Sale")]
        public int SaleId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public decimal Quantity { get; set; }

        public virtual Sale Sale { get; set; }
        public virtual Product Product { get; set; }
    }
}