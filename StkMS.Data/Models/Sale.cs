using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace StkMS.Data.Models
{
    internal class Sale
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateTime { get; set; }
        public bool IsComplete { get; set; }

        public virtual ICollection<SaleItem> SaleItems { get; set; }
    }
}