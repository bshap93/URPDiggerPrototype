using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace Domains.Items
{
    public class Inventory : MonoBehaviour
    {
        public List<BaseItem> Content;

        [Header("Debug")]
        /// If true, will draw the contents of the inventory in its inspector
        [Tooltip(
            "The Inventory component is like the database and controller part of your inventory. It won't show anything on screen, you'll need also an InventoryDisplay for that. Here you can decide whether or not you want to output a debug content in the inspector (useful for debugging).")]
        public bool DrawContentInInspector;

        public float WeightLimit;

        public MMFeedbacks InventoryFullFeedbacks;

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
            if (CurrentWeight() + item.ItemWeight > WeightLimit)
            {
                UnityEngine.Debug.LogWarning("Inventory is full");
                InventoryFullFeedbacks?.PlayFeedbacks();
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
            return Content.Find(i => i.ItemID == itemID);
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
    }
}
