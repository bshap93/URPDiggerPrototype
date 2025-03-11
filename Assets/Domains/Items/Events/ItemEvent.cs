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
        static ItemEvent e;

        public BaseItem Item;
        public ItemEventType EventType;
        public Transform ItemTransform;
        public int Amount;

        public static void Trigger(ItemEventType eventType, BaseItem inventoryItem, Transform itemTransform,
            int amount = 1)
        {
            e.EventType = eventType;
            e.Amount = amount;
            e.Item = inventoryItem;
            e.ItemTransform = itemTransform;

            MMEventManager.TriggerEvent(e);
        }
    }
}
