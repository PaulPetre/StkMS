using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace StkMS.Data.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public decimal Quantity { get; set; }

        public virtual Product Product { get; set; }
    }
}