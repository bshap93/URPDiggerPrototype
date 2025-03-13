using MoreMountains.Tools;

namespace Domains.Items.Events
{
    public enum InventoryEventType
    {
        ContentChanged,
        InventoryLoaded
    }

    public struct InventoryEvent
    {
        public static InventoryEvent _e;

        public InventoryEventType EventType;

        public Inventory Inventory;

        public static void Trigger(InventoryEventType eventType, Inventory inventory)
        {
            _e.EventType = eventType;
            _e.Inventory = inventory;

            MMEventManager.TriggerEvent(_e);
        }
    }
}
