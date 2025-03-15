using System;
using Domains.Player.Scripts;
using Domains.SaveLoad;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Scene.Scripts
{
    [Serializable]
    public class SaveManager : MonoBehaviour, MMEventListener<SaveLoadEvent>
    {
        public const string SaveFileName = "GameSave.es3";

        // [Header("Persistence Managers")] [SerializeField]
        // InventoryPersistenceManager inventoryManager;
        [FormerlySerializedAs("playerMutableStatsManager")]
        [FormerlySerializedAs("playerStatsManager")]
        [FormerlySerializedAs("resourcesManager")]
        [SerializeField]
        private PlayerStaminaManager playerStaminaManager;

        [SerializeField] private PlayerHealthManager playerHealthManager;

        [Header("Item & Container Persistence")]
        public PickableManager pickableManager;

        public PlayerInventoryManager playerInventoryManager;

        public int currentSlot;

        public static SaveManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // Initialize managers if needed
            if (pickableManager == null)
            {
                pickableManager = GetComponentInChildren<PickableManager>(true);
                if (pickableManager == null)
                {
                    var pickableGO = new GameObject("PickableManager");
                    pickableManager = pickableGO.AddComponent<PickableManager>();
                    pickableGO.transform.SetParent(transform);
                }
            }


            if (playerStaminaManager == null)
            {
                playerStaminaManager = GetComponentInChildren<PlayerStaminaManager>(true);
                if (playerStaminaManager == null)
                    UnityEngine.Debug.LogError("PlayerStaminaManager not found in SaveManager");
            }

            if (playerHealthManager == null)
            {
                playerHealthManager = GetComponentInChildren<PlayerHealthManager>(true);
                if (playerHealthManager == null)
                    UnityEngine.Debug.LogError("PlayerHealthManager not found in SaveManager");
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

        public void OnMMEvent(SaveLoadEvent eventType)
        {
            if (eventType.EventType == SaveLoadEventType.Save)
                SaveAll();
            else if (eventType.EventType == SaveLoadEventType.Load) LoadAll();
        }


        private string GetSaveFileName()
        {
            return "GameSave.es3"; // Always use a single save file
        }

        public void SaveAll()
        {
            PlayerStaminaManager.SavePlayerStamina();
            PlayerHealthManager.SavePlayerHealth();
            PlayerInventoryManager.SaveInventory();
            PlayerCurrencyManager.SavePlayerCurrency();
            UnityEngine.Debug.Log($"Save Path: {Application.persistentDataPath}");
        }

        public bool LoadAll()
        {
            // var inventoryLoaded = inventoryManager != null && inventoryManager.HasSavedData();
            var staminaLoaded = playerStaminaManager != null && playerStaminaManager.HasSavedData();
            var healthLoaded = playerHealthManager != null && playerHealthManager.HasSavedData();
            var inventoryLoaded = playerInventoryManager != null && playerInventoryManager.HasSavedData();


            // if (inventoryLoaded) inventoryManager.LoadInventory();
            if (staminaLoaded) playerStaminaManager.LoadPlayerStamina();
            if (healthLoaded) playerHealthManager.LoadPlayerHealth();
            if (inventoryLoaded) playerInventoryManager.LoadInventory();

            // Load pickable items and destroyed containers
            pickableManager?.LoadPickedItems();


            return staminaLoaded || healthLoaded || inventoryLoaded;
        }
    }
}