using System;

namespace StkMS.Library.Models
{
    public class Inventory
    {
        public DateTime StartDate { get; }
        public DateTime? EndDate { get; }

        public InventoryDetails[] Items { get; }

        public Inventory(DateTime startDate, DateTime? endDate, InventoryDetails[] items)
        {
            StartDate = startDate;
            EndDate = endDate;
            Items = items;
        }
    }
}