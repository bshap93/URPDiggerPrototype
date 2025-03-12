using System;
using Domains.Player.Scripts;
using Gameplay.Player.Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Domains.Scene.Scripts
{
    [Serializable]
    public class SaveManager : MonoBehaviour
    {
        const string SaveFilePrefix = "GameSave_";
        const string SaveFileExtension = ".es3";

        // [Header("Persistence Managers")] [SerializeField]
        // InventoryPersistenceManager inventoryManager;
        [FormerlySerializedAs("playerMutableStatsManager")]
        [FormerlySerializedAs("playerStatsManager")]
        [FormerlySerializedAs("resourcesManager")]
        [SerializeField]
        PlayerStaminaManager playerStaminaManager;
        [SerializeField] PlayerHealthManager playerHealthManager;

        [Header("Item & Container Persistence")]
        public PickableManager pickableManager;

        public int currentSlot;

        public static SaveManager Instance { get; private set; }

        void Awake()
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

        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F5)) // Press F5 to force save
            {
                PlayerHealthManager.SavePlayerHealth();
                PlayerStaminaManager.SavePlayerStamina();
                UnityEngine.Debug.Log("Manual Save Triggered!");
            }
        }


        string GetSaveFileName(int slot)
        {
            return $"{SaveFilePrefix}{slot}{SaveFileExtension}";
        }

        public void SaveAll()
        {
            // inventoryManager?.SaveInventory();
            PlayerStaminaManager.SavePlayerStamina();
            PlayerHealthManager.SavePlayerHealth();
            UnityEngine.Debug.Log($"Save Path: {Application.persistentDataPath}");
        }

        public bool LoadAll()
        {
            // var inventoryLoaded = inventoryManager != null && inventoryManager.HasSavedData();
            var staminaLoaded = playerStaminaManager != null && playerStaminaManager.HasSavedData();
            var healthLoaded = playerHealthManager != null && playerHealthManager.HasSavedData();

            // if (inventoryLoaded) inventoryManager.LoadInventory();
            if (staminaLoaded) playerStaminaManager.LoadPlayerStamina();
            if (healthLoaded) playerHealthManager.LoadPlayerHealth();

            // Load pickable items and destroyed containers
            pickableManager?.LoadPickedItems();


            return staminaLoaded || healthLoaded;
            // || inventoryLoaded;
        }
    }
}
