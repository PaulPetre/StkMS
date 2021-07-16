using System;
using System.Globalization;
using System.Linq;
using StkMS.Helpers;

#nullable disable

namespace StkMS.ViewModels
{
    public class SaleDetailsViewModel
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public SaleViewModel[] Items { get; set; }
        public string TotalValue => Items.Select(it => it.Value).Sum().ToString("F2");

        public DateTime? FormatDateTime => DateTime.Now.ToString().ParseDateTimeWithCulture();

    }
}
