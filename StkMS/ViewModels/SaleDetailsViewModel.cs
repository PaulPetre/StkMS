using System;
using System.Linq;

#nullable disable

namespace StkMS.ViewModels
{
    public class SaleDetailsViewModel
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public SaleViewModel[] Items { get; set; }
        public string TotalValue => Items.Select(it => it.Value).Sum().ToString("F2");
    }
}
