using Core.Events;
using Domains.Player.Events;
using Gameplay.Character;
using Gameplay.Character.Stamina;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

namespace Domains.Player.Scripts
{
#if UNITY_EDITOR
    public static class PlayerStaminaManagerDebug
    {
        [MenuItem("Debug/Reset Stamina")]
        public static void ResetStamina()
        {
            PlayerStaminaManager.ResetPlayerStamina();
        }
    }
#endif
    public class PlayerStaminaManager : MonoBehaviour, MMEventListener<StaminaEvent>
    {
        public static float StaminaPoints;
        public static float MaxStaminaPoints;
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static float InitialCharacterStamina;
        public StaminaBarUpdater staminaBarUpdater;
        public CharacterStatProfile characterStatProfile;


        string _savePath;


        void Awake()
        {
            if (characterStatProfile != null)
                InitialCharacterStamina = characterStatProfile.InitialMaxStamina;
            else
                UnityEngine.Debug.LogError("CharacterStatProfile not set in PlayerStaminaManager");
        }


        void Start()
        {
            _savePath = GetSaveFilePath();

            if (!ES3.FileExists(_savePath))
            {
                UnityEngine.Debug.Log("[PlayerStaminaManager] No save file found, forcing initial save...");
                ResetPlayerStamina(); // Ensure default values are set
            }

            LoadPlayerStamina();
        }


        void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F5)) // Press F5 to force save
            {
                SavePlayerStamina();
                UnityEngine.Debug.Log("Manual Save Triggered!");
            }
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(StaminaEvent staminaEvent)
        {
            switch (staminaEvent.EventType)
            {
                case StaminaEventType.ConsumeStamina:
                    ConsumeStamina(staminaEvent.ByValue);
                    break;
                case StaminaEventType.RecoverStamina:
                    RecoverStamina(staminaEvent.ByValue);
                    break;
                case StaminaEventType.FullyRecoverStamina:
                    FullyRecoverStamina();
                    break;
                case StaminaEventType.IncreaseMaximumStamina:
                    IncreaseMaximumStamina(staminaEvent.ByValue);
                    break;
            }
        }

        public void Initialize()
        {
            ResetPlayerStamina();
            staminaBarUpdater.Initialize();
        }

        public static void ConsumeStamina(float amount)
        {
            if (StaminaPoints - amount < 0)
            {
                StaminaPoints = 0;
                PlayerStatusEvent.Trigger(PlayerStatusEventType.OutOfStamina);
                AlertManager.ShowAlert("Stamina Depleted");
            }
            else
            {
                StaminaPoints -= amount;
            }

            SavePlayerStamina();
        }

        public static void RecoverStamina(float amount)
        {
            if (StaminaPoints == 0 && amount > 0) PlayerStatusEvent.Trigger(PlayerStatusEventType.RegainedStamina);
            StaminaPoints += amount;
            SavePlayerStamina();
        }

        public static void FullyRecoverStamina()
        {
            StaminaPoints = MaxStaminaPoints;
            PlayerStatusEvent.Trigger(PlayerStatusEventType.RegainedStamina);

            SavePlayerStamina();
        }

        public static void IncreaseMaximumStamina(float amount)
        {
            MaxStaminaPoints += amount;
            SavePlayerStamina();
        }

        public static void DecreaseMaximumStamina(float amount)
        {
            MaxStaminaPoints -= amount;
            SavePlayerStamina();
        }

        static string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            var path = string.IsNullOrEmpty(slotPath) ? "PlayerStamina.es3" : $"{slotPath}/PlayerStamina.es3";

            UnityEngine.Debug.Log($"[PlayerStaminaManager] Save file path resolved to: {path}");
            return path;
        }

        public void LoadPlayerStamina()
        {
            var saveFilePath = GetSaveFilePath();
            UnityEngine.Debug.Log($"[PlayerStaminaManager] Checking for saved stamina data at: {saveFilePath}");

            if (ES3.FileExists(saveFilePath))
            {
                StaminaPoints = ES3.Load<float>("StaminaPoints", saveFilePath);
                MaxStaminaPoints = ES3.Load<float>("MaxStaminaPoints", saveFilePath);
                staminaBarUpdater.Initialize();
                UnityEngine.Debug.Log(
                    $"✅ Loaded stamina data: StaminaPoints={StaminaPoints}, MaxStaminaPoints={MaxStaminaPoints}");
            }
            else
            {
                UnityEngine.Debug.LogError($"❌ No saved stamina data found at {saveFilePath}");
                ResetPlayerStamina();
                staminaBarUpdater.Initialize();
            }
        }

        public static void ResetPlayerStamina()
        {
            var characterStatProfile =
                Resources.Load<CharacterStatProfile>(CharacterResourcePaths.CharacterStatProfileFilePath);

            if (characterStatProfile == null)
            {
                UnityEngine.Debug.LogError("\u274c CharacterStatProfile not found! Using default values.");
                StaminaPoints = 100f;
                MaxStaminaPoints = 100f;
            }
            else
            {
                StaminaPoints = characterStatProfile.InitialMaxStamina;
                MaxStaminaPoints = characterStatProfile.InitialMaxStamina;
            }


            SavePlayerStamina();
        }

        public static void SavePlayerStamina()
        {
            var saveFilePath = GetSaveFilePath();

            ES3.Save("StaminaPoints", StaminaPoints, saveFilePath);
            ES3.Save("MaxStaminaPoints", MaxStaminaPoints, saveFilePath);

            if (ES3.FileExists(saveFilePath))
                UnityEngine.Debug.Log($"✅ Successfully saved stamina data at {saveFilePath}");
            else
                UnityEngine.Debug.LogError($"❌ Failed to save stamina data at {saveFilePath}");
        }
        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath());
        }
        public static bool IsPlayerOutOfStamina()
        {
            return StaminaPoints <= 0;
        }
    }
}
