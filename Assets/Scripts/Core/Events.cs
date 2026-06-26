using UnityEngine;

namespace Farm
{
    // Struct payloads for EventBus — value types, no heap allocation.

    public struct GameReadyEvent { }

    public struct ConstructionBuiltEvent
    {
        public int SlotIndex;
    }

    public struct ConstructionUpgradedEvent
    {
        public int SlotIndex;
        public int NewLevel;
    }

    public struct HarvestCompletedEvent
    {
        public int SlotIndex;
        public double Value;
    }

    public struct SaleCompletedEvent
    {
        public double Amount;
        public Vector3 WorldPos;
    }

    public struct UpgradePurchasedEvent
    {
        public string Id;
    }

    public struct StatsChangedEvent
    {
        public int SlotIndex;   // -1 = global
    }
}
