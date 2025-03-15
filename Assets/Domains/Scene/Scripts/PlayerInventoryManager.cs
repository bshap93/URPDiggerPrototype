using System;
using System.Collections.Generic;
using System.Linq;
using Domains.Items;
using Domains.Items.Events;
using Domains.UI;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

namespace Domains.Scene.Scripts
{
#if UNITY_EDITOR
    public static class InventoryPersistenceManagerDebug
    {
        [MenuItem("Debug/Reset/Reset Inventory")]
        public static void ResetInventory()
        {
            PlayerInventoryManager.ResetInventory();
        }
    }
#endif

    [RequireComponent(typeof(Inventory))]
    public class PlayerInventoryManager : MonoBehaviour, MMEventListener<InventoryEvent>
    {
        private const string InventoryKey = "InventoryContentData";
        private const string ResourcesPath = "Items";
        public static Inventory PlayerInventory;

        public static List<InventoryEntryData> InventoryContentData = new();

        public InventoryBarUpdater inventoryBarUpdater;

        private string _savePath;

        private void Start()
        {
            PlayerInventory = FindFirstObjectByType<Inventory>();
            _savePath = GetSaveFilePath();

            if (!ES3.FileExists(_savePath))
            {
                UnityEngine.Debug.Log("[PlayerHealthManager] No save file found, forcing initial save...");
                ResetInventory(); // Ensure default values are set
            }

            PlayerInventory.SetWeightLimit(PlayerInfoSheet.WeightLimit);

            LoadInventory();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F5)) // Press F5 to force save
            {
                SaveInventory();
                UnityEngine.Debug.Log("Player inventory saved");
            }
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(InventoryEvent eventType)
        {
            throw new NotImplementedException();
        }


        private static string GetSaveFilePath()
        {
            return SaveManager.SaveFileName;
        }

        public static void SaveInventory()
        {
            var saveFilePath = GetSaveFilePath();
            if (PlayerInventory == null)
            {
                UnityEngine.Debug.LogError("InventoryPersistenceManager: No Inventory Assigned!");
                return;
            }

            var inventoryData = new List<InventoryEntryData>();

            foreach (var entry in PlayerInventory.Content)
                inventoryData.Add(new InventoryEntryData(entry.UniqueID, entry.BaseItem.ItemID));

            ES3.Save(InventoryKey, inventoryData, saveFilePath);
            UnityEngine.Debug.Log($"✅ Inventory saved at {saveFilePath}");
        }

        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath());
        }

        public void Initialize()
        {
            ResetInventory();
            inventoryBarUpdater.Initialize();
        }

        public void LoadInventory()
        {
            var saveFilePath = GetSaveFilePath();

            if (ES3.FileExists(saveFilePath))
            {
                InventoryContentData = ES3.Load<List<InventoryEntryData>>(InventoryKey, saveFilePath);
                inventoryBarUpdater.Initialize();
                PlayerInventory.Content.Clear();

                foreach (var itemData in InventoryContentData)
                {
                    var item = GetItemByID(itemData.ItemID);
                    if (item != null)
                    {
                        var entry = new Inventory.InventoryEntry(itemData.UniqueID, item);
                        PlayerInventory.Content.Add(entry);
                    }
                }

                UnityEngine.Debug.Log($"✅ Loaded inventory data from {saveFilePath}");
            }
            else
            {
                UnityEngine.Debug.LogError($"No saved inventory data found at {saveFilePath}");
                ResetInventory();
            }
        }

        public static void ResetInventory()
        {
            InventoryContentData.Clear();
            PlayerInventory.Content.Clear();

            SaveInventory();
        }

        private static void RestoreInventory(Inventory inventory, List<InventoryEntryData> inventoryContentData)
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

        private static BaseItem GetItemByID(string itemID)
        {
            return Resources.LoadAll<BaseItem>(ResourcesPath).FirstOrDefault(i => i.ItemID == itemID);
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