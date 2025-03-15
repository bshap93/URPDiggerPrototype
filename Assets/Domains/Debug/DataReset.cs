using Domains.Player.Scripts;
using Domains.Scene.Scripts;
using UnityEngine;

namespace Gameplay.Config
{
    public class DataReset : MonoBehaviour
    {
        private void Awake()
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


            // // Reset  Mutable Stats
            PlayerStaminaManager.ResetPlayerStamina();
            PlayerHealthManager.ResetPlayerHealth();
            PlayerInventoryManager.ResetInventory();
            //
            // PlayerAttributesProgressionManager.ResetPlayerAttributesProgression();

            Debug.Log("Destuctable containers reset.");


            Debug.Log("All save data cleared successfully.");
        }
    }
}