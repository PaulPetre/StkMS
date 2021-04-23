using System;
using StkMS.DomainModels;

namespace StkMS.Contracts
{
    public interface IInventory
    {
        InventoryState State { get; }

        string Id { get; }
        DateTimeOffset? StartedAt { get; }
        DateTimeOffset? CompletedAt { get; }

        void Begin();
        void Complete();
    }
}