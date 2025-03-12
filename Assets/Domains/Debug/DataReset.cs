using Domains.Player.Scripts;
using Gameplay.Player.Stats;
using UnityEngine;

namespace Gameplay.Config
{
    public class DataReset : MonoBehaviour
    {
        void Awake()
        {
            Debug.Log("PurePrototypeReset: Awake() called.");
            ClearAllSaveData();
        }
        public static void ClearAllSaveData()
        {
            // Reset Pickables
            PickableManager.ResetPickedItems();


            // Reset Dispenser States
            // DispenserManager.ResetDispenserStates();

            // // Reset Inventory System
            // InventoryPersistenceManager.Instance?.ResetInventory();

            // // Reset  Mutable Stats
            PlayerStaminaManager.ResetPlayerStamina();
            PlayerHealthManager.ResetPlayerHealth();
            //
            // PlayerAttributesProgressionManager.ResetPlayerAttributesProgression();

            Debug.Log("Destuctable containers reset.");


            Debug.Log("All save data cleared successfully.");
        }
    }
}
