using System;
using System.Collections.Generic;
using Domains.Items.Events;
using Domains.UI.Events;
using Gameplay.Events;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Items
{
    public class Inventory : MonoBehaviour, MMEventListener<ItemEvent>
    {
        public List<BaseItem> Content;

        public float WeightLimit;

        public MMFeedbacks InventoryFullFeedbacks;

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }
        public void OnMMEvent(ItemEvent eventType)
        {
            if (eventType.EventType == ItemEventType.Picked)
            {
                var added = AddItem(eventType.Item);
                if (added)
                {
                    InventoryEvent.Trigger(InventoryEventType.ContentChanged, this);
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Item not added to inventory");
                    AlertEvent.Trigger(AlertType.ItemScrapped, eventType.Item.ItemName + " scrapped");
                }
            }
        }

        public float CurrentWeight()
        {
            float weight = 0;
            foreach (var item in Content) weight += item.ItemWeight;

            return weight;
        }

        public float RemainingWeight()
        {
            return WeightLimit - CurrentWeight();
        }

        public virtual bool AddItem(BaseItem item)
        {
            var existingItem = Content.Find(i => i.ItemID == item.ItemID);

            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
                return true;
            }

            if (CurrentWeight() + item.ItemWeight > WeightLimit)
            {
                UnityEngine.Debug.LogWarning("Inventory is full");
                InventoryFullFeedbacks?.PlayFeedbacks();
                AlertEvent.Trigger(AlertType.InventoryFull, "Inventory is full");
                return false;
            }

            Content.Add(item);
            return true;
        }

        public virtual bool RemoveItem(BaseItem item)
        {
            if (!Content.Contains(item))
            {
                UnityEngine.Debug.LogWarning("Item not found in inventory");
                return false;
            }

            return Content.Remove(item);
        }

        public virtual bool RemoveItem(string itemID)
        {
            var item = Content.Find(i => i.ItemID == itemID);
            if (item == null)
            {
                UnityEngine.Debug.LogWarning("Item not found in inventory");
                return false;
            }

            return Content.Remove(item);
        }

        public virtual int GetQuantity(string itemID)
        {
            return Content.FindAll(i => i.ItemID == itemID).Count;
        }

        public virtual BaseItem GetItem(string itemID)
        {
            return Content.Find(i => i.ItemID == itemID) ?? null;
        }

        public virtual List<int> InventoryContains(string searchedItemID)
        {
            var list = new List<int>();

            for (var i = 0; i < Content.Count; i++)
                if (!BaseItem.IsNull(Content[i]))
                    if (Content[i].ItemID == searchedItemID)
                        list.Add(i);

            return list;
        }

        public virtual bool IsFull()
        {
            return CurrentWeight() >= WeightLimit;
        }

        public virtual void SaveInventory()
        {
            throw new NotImplementedException();
        }
        public void EmptyInventory()
        {
            Content = new List<BaseItem>();

            InventoryEvent.Trigger(InventoryEventType.ContentChanged, this);
        }
    }
}
