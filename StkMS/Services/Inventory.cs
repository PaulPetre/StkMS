using System;
using StkMS.Contracts;
using StkMS.Library.Models;

namespace StkMS.Services
{
    public class Inventory : IInventory
    {
        public InventoryState State { get; private set; }

        public string Id { get; private set; } = "";
        public DateTimeOffset? StartedAt { get; private set; }
        public DateTimeOffset? CompletedAt { get; private set; }

        public void Begin()
        {
            if (State == InventoryState.InProgress)
                return;

            State = InventoryState.InProgress;
            Id = Guid.NewGuid().ToString();
            StartedAt = DateTimeOffset.Now;
            CompletedAt = null;
        }

        public void Complete()
        {
            if (State == InventoryState.None)
                return;

            State = InventoryState.None;
            CompletedAt = DateTimeOffset.Now;
        }
    }
}