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

        public BaseItem Item;
        public ItemEventType EventType;
        public Transform ItemTransform;
        public int Amount;

        public static void Trigger(ItemEventType eventType, BaseItem inventoryItem, Transform itemTransform,
            int amount = 1)
        {
            _e.EventType = eventType;
            _e.Amount = amount;
            _e.Item = inventoryItem;
            _e.ItemTransform = itemTransform;

            MMEventManager.TriggerEvent(_e);
        }
    }
}
