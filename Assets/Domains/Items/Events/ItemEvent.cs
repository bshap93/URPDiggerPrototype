using Domains.Items;
using MoreMountains.Tools;
using UnityEngine;

public enum ItemEventType
{
    PickupRangeEntered,
    Picked,
    Sold,
    Dropped,
    PickupRangeExited
}

namespace Gameplay.Events
{
    public struct ItemEvent
    {
        static ItemEvent _e;

        // Amount is embedded in the item
        public Inventory.InventoryEntry Item;
        public ItemEventType EventType;
        public Transform ItemTransform;

        public static void Trigger(ItemEventType eventType, Inventory.InventoryEntry inventoryItem, Transform transform)
        {
            _e.EventType = eventType;
            _e.Item = inventoryItem;
            _e.ItemTransform = transform;

            MMEventManager.TriggerEvent(_e);
        }
    }
}
