using System;
using System.Collections.Generic;
using System.Linq;
using Domains.Items;
using Domains.Items.Events;
using UnityEditor;
using UnityEngine;

namespace Domains.Scene.Scripts
{
#if UNITY_EDITOR
    public static class InventoryPersistenceManagerDebug
    {
        [MenuItem("Debug/Reset Inventory")]
        public static void ResetInventory()
        {
            PlayerInventoryManager.ResetInventory();
        }
    }
#endif

    public class PlayerInventoryManager : MonoBehaviour
    {
        const string INVENTORY_KEY = "InventoryContent";
        const string RESOURCES_PATH = "Items";
        public Inventory playerInventory;


        static string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            var path = string.IsNullOrEmpty(slotPath) ? "PlayerInventory.es3" : $"{slotPath}/PlayerStamina.es3";

            UnityEngine.Debug.Log($"[PlayerInventoryManager] Save file path resolved to: {path}");
            return path;
        }

        public void SaveInventory()
        {
            var saveFilePath = GetSaveFilePath();
            if (playerInventory == null)
            {
                UnityEngine.Debug.LogError("InventoryPersistenceManager: No Inventory Assigned!");
                return;
            }

            var inventoryData = new List<InventoryEntryData>();

            foreach (var entry in playerInventory.Content)
                inventoryData.Add(new InventoryEntryData(entry.UniqueID, entry.BaseItem.ItemID));

            ES3.Save(INVENTORY_KEY, inventoryData, saveFilePath);
            UnityEngine.Debug.Log($"✅ Inventory saved at {saveFilePath}");
        }
        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath());
        }
        public void LoadInventory()
        {
            var saveFilePath = GetSaveFilePath();

            if (!ES3.FileExists(saveFilePath))
            {
                UnityEngine.Debug.LogWarning($"❌ No saved inventory data found at {saveFilePath}");
                return;
            }

            var loadedItems = ES3.Load<List<InventoryEntryData>>(INVENTORY_KEY, saveFilePath);
            playerInventory.Content.Clear();

            foreach (var itemData in loadedItems)
            {
                var item = GetItemByID(itemData.ItemID);
                if (item != null)
                {
                    var entry = new Inventory.InventoryEntry(itemData.UniqueID, item);
                    playerInventory.Content.Add(entry);
                }
            }

            InventoryEvent.Trigger(InventoryEventType.InventoryLoaded, playerInventory);
            UnityEngine.Debug.Log($"✅ Loaded inventory data from {saveFilePath}");
        }

        public static void ResetInventory()
        {
            UnityEngine.Debug.Log("[PlayerInventoryManager] Resetting Inventory");

            ES3.Save(INVENTORY_KEY, new List<BaseItem>(), GetSaveFilePath());
        }

        static void RestoreInventory(Inventory inventory, List<InventoryEntryData> inventoryContentData)
        {
            foreach (var itemData in inventoryContentData)
            {
                var item = GetItemByID(itemData.ItemID);
                if (item != null)
                {
                    var entry = new Inventory.InventoryEntry(itemData.UniqueID, item); // Use saved UniqueID
                    inventory.AddItem(entry);
                }
            }
        }

        static BaseItem GetItemByID(string itemID)
        {
            return Resources.LoadAll<BaseItem>(RESOURCES_PATH).FirstOrDefault(i => i.ItemID == itemID);
        }

        [Serializable]
        public class InventoryEntryData
        {
            public string UniqueID;
            public string ItemID;
            public InventoryEntryData(string itemUniqueID, string baseItemItemID)
            {
                UniqueID = itemUniqueID;
                ItemID = baseItemItemID;
            }
        }
    }
}
