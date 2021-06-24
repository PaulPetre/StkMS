using System.ComponentModel.DataAnnotations;

#nullable disable

namespace StkMS.Data.Models
{
    internal class Product
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
    }
}