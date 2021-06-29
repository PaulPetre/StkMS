using System;

#nullable disable

namespace StkMS.ViewModels
{
    public class SaleDetailsViewModel
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public SaleViewModel[] Items { get; set; }
    }
}