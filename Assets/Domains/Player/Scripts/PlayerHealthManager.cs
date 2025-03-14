using Domains.Player.Events;
using Domains.UI;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

namespace Domains.Player.Scripts
{
#if UNITY_EDITOR
    public static class PlayerHealthManagerDebug
    {
        [MenuItem("Debug/Reset/Reset Health")]
        public static void ResetHealth()
        {
            PlayerHealthManager.ResetPlayerHealth();
        }
    }
#endif

    public class PlayerHealthManager : MonoBehaviour, MMEventListener<HealthEvent>
    {
        public static float HealthPoints;
        public static float MaxHealthPoints;


        public static float InitialCharacterHealth;
        public HealthBarUpdater healthBarUpdater;
        public CharacterStatProfile characterStatProfile;

        public bool immuneToDamage;

        private string _savePath;

        private void Awake()
        {
            if (characterStatProfile != null)
                InitialCharacterHealth = characterStatProfile.InitialMaxHealth;
            else
                UnityEngine.Debug.LogError("CharacterStatProfile not set in PlayerHealthManager");
        }

        private void Start()
        {
            _savePath = GetSaveFilePath();

            if (!ES3.FileExists(_savePath))
            {
                UnityEngine.Debug.Log("[PlayerHealthManager] No save file found, forcing initial save...");
                ResetPlayerHealth(); // Ensure default values are set
            }

            LoadPlayerHealth();
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F5)) // Press F5 to force save
            {
                SavePlayerHealth();
                UnityEngine.Debug.Log("Player health saved");
            }
        }

        public void OnMMEvent(HealthEvent eventType)
        {
            switch (eventType.EventType)
            {
                case HealthEventType.ConsumeHealth:
                    ConsumeHealth(eventType.ByValue);
                    break;
                case HealthEventType.RecoverHealth:
                    RecoverHealth(eventType.ByValue);
                    break;
                case HealthEventType.IncreaseMaximumHealth:
                    IncreaseMaximumHealth(eventType.ByValue);
                    break;
                case HealthEventType.DecreaseMaximumHealth:
                    DecreaseMaximumHealth(eventType.ByValue);
                    break;
                default:
                    UnityEngine.Debug.LogWarning($"Unknown HealthEventType: {eventType.EventType}");
                    break;
            }
        }


        public void Initialize()
        {
            ResetPlayerHealth();
            healthBarUpdater.Initialize();
        }

        public static void ConsumeHealth(float healthToConsume)
        {
            if (HealthPoints - healthToConsume < 0)
            {
                HealthPoints = 0;
                PlayerStatusEvent.Trigger(PlayerStatusEventType.OutOfHealth);
                HealthEvent.Trigger(HealthEventType.ConsumeHealth, healthToConsume);
                AlertManager.ShowAlert("Health Depleted");
            }
            else
            {
                HealthPoints -= healthToConsume;
                HealthEvent.Trigger(HealthEventType.ConsumeHealth, healthToConsume);
            }

            // SavePlayerHealth();
        }

        public static void RecoverHealth(float amount)
        {
            if (HealthPoints == 0 && amount > 0) PlayerStatusEvent.Trigger(PlayerStatusEventType.RegainedHealth);
            HealthPoints += amount;
            HealthEvent.Trigger(HealthEventType.RecoverHealth, amount);
        }

        public static void FullyRecoverHealth()
        {
            HealthPoints = MaxHealthPoints;
            PlayerStatusEvent.Trigger(PlayerStatusEventType.RegainedHealth);
            HealthEvent.Trigger(HealthEventType.RecoverHealth, MaxHealthPoints - HealthPoints);
        }

        public static void IncreaseMaximumHealth(float amount)
        {
            MaxHealthPoints += amount;
            HealthEvent.Trigger(HealthEventType.IncreaseMaximumHealth, amount);
        }

        public static void DecreaseMaximumHealth(float amount)
        {
            MaxHealthPoints -= amount;
            HealthEvent.Trigger(HealthEventType.DecreaseMaximumHealth, amount);
        }


        private static string GetSaveFilePath()
        {
            return "GameSave.es3"; // Single save file for everything
        }

        public void LoadPlayerHealth()
        {
            var saveFilePath = GetSaveFilePath();

            if (ES3.FileExists(saveFilePath))
            {
                HealthPoints = ES3.Load<float>("HealthPoints", saveFilePath);
                MaxHealthPoints = ES3.Load<float>("MaxHealthPoints", saveFilePath);
                healthBarUpdater.Initialize();
            }
            else
            {
                UnityEngine.Debug.LogError($"No saved health data found at {saveFilePath}");
                ResetPlayerHealth();
                healthBarUpdater.Initialize();
            }
        }

        public static void ResetPlayerHealth()
        {
            var characterStatProfile =
                Resources.Load<CharacterStatProfile>(CharacterResourcePaths.CharacterStatProfileFilePath);

            if (characterStatProfile == null)
            {
                UnityEngine.Debug.LogError("CharacterStatProfile not found! Using default values.");
                HealthPoints = 20f; // Default fallback
                MaxHealthPoints = 20f;
            }
            else
            {
                HealthPoints = characterStatProfile.InitialMaxHealth;
                MaxHealthPoints = characterStatProfile.InitialMaxHealth;
            }

            SavePlayerHealth();
        }


        public static void SavePlayerHealth()
        {
            var saveFilePath = GetSaveFilePath();

            ES3.Save("HealthPoints", HealthPoints, saveFilePath);
            ES3.Save("MaxHealthPoints", MaxHealthPoints, saveFilePath);

            if (ES3.FileExists(saveFilePath))
                UnityEngine.Debug.Log($"✅ Successfully saved health data at {saveFilePath}");
            else
                UnityEngine.Debug.LogError($"❌ Failed to save health data at {saveFilePath}");
        }


        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath());
        }

        public static bool IsPlayerOutOfHealth()
        {
            return HealthPoints <= 0;
        }

        // [Button(ButtonSizes.Medium)]
        public void HurtPlayer(float damage = 10)
        {
            if (immuneToDamage) return;
            ConsumeHealth(damage);
        }
    }
}