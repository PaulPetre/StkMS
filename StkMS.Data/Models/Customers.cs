using System.ComponentModel.DataAnnotations;

namespace StkMS.Data.Models
{
    public class Customers
    {

        [Key]
        public int CustomerId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }
    }
}
