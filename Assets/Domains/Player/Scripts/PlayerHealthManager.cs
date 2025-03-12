using System;
using Core.Events;
using Domains.Player.Events;
using Domains.Scene.Events;
using Domains.UI;
using Gameplay.Character;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR


namespace Domains.Player.Scripts
{
    public static class PlayerHealthManagerDebug
    {
        [MenuItem("Debug/Reset Health")]
        public static void ResetHealth()
        {
            PlayerHealthManager.ResetPlayerHealth();
        }
    }
#endif

    public class PlayerHealthManager : MonoBehaviour, MMEventListener<SceneEvent>
    {
        public static float HealthPoints;
        public static float MaxHealthPoints;


        public static float InitialCharacterHealth;
        public HealthBarUpdater healthBarUpdater;
        public CharacterStatProfile characterStatProfile;

        public bool immuneToDamage;

        string _savePath;

        void Awake()
        {
            if (characterStatProfile != null)
                InitialCharacterHealth = characterStatProfile.InitialMaxHealth;
            else
                UnityEngine.Debug.LogError("CharacterStatProfile not set in PlayerHealthManager");
        }

        void Start()
        {
            _savePath = GetSaveFilePath();

            LoadPlayerHealth();
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }
        public void OnMMEvent(SceneEvent eventType)
        {
            throw new NotImplementedException();
        }


        public void Initialize()
        {
            UnityEngine.Debug.Log("Initializing player health");
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

            SavePlayerHealth();
        }

        public static void RecoverHealth(float amount)
        {
            if (HealthPoints == 0 && amount > 0) PlayerStatusEvent.Trigger(PlayerStatusEventType.RegainedHealth);
            HealthPoints += amount;
            HealthEvent.Trigger(HealthEventType.RecoverHealth, amount);
            SavePlayerHealth();
        }

        public static void FullyRecoverHealth()
        {
            HealthPoints = MaxHealthPoints;
            PlayerStatusEvent.Trigger(PlayerStatusEventType.RegainedHealth);
            HealthEvent.Trigger(HealthEventType.RecoverHealth, MaxHealthPoints - HealthPoints);
            SavePlayerHealth();
        }

        public static void IncreaseMaximumHealth(float amount)
        {
            MaxHealthPoints += amount;
            HealthEvent.Trigger(HealthEventType.IncreaseMaximumHealth, amount);
            SavePlayerHealth();
        }

        public static void DecreaseMaximumHealth(float amount)
        {
            MaxHealthPoints -= amount;
            HealthEvent.Trigger(HealthEventType.DecreaseMaximumHealth, amount);
            SavePlayerHealth();
        }


        static string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            return string.IsNullOrEmpty(slotPath) ? "PlayerHealth.es3" : $"{slotPath}/PlayerHealth.es3";
        }

        public void LoadPlayerHealth()
        {
            var saveFilePath = GetSaveFilePath();
            UnityEngine.Debug.Log($"Checking for saved health data at: {saveFilePath}");

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
            UnityEngine.Debug.Log($"Saving player health to: {saveFilePath}");

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
