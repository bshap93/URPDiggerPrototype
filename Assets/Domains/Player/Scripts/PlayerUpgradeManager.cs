using System;
using System.Collections.Generic;
using Domains.Player.Events;
using Domains.Player.Scripts.ScriptableObjects;
using MoreMountains.Tools;
using UnityEngine;

namespace Domains.Player.Scripts
{
    public class PlayerUpgradeManager : MonoBehaviour, MMEventListener<UpgradeEvent>
    {
        [SerializeField] private List<UpgradeData> availableUpgrades;
        private readonly Dictionary<string, int> _upgradeLevels = new();
        private readonly Dictionary<string, string> _upgradeNames = new();

        private void Awake()
        {
        }


        private void Start()
        {
            LoadUpgrades();
        }


        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(UpgradeEvent eventType)
        {
            throw new NotImplementedException();
        }

        public void BuyUpgrade(string upgradeTypeName)
        {
            if (!_upgradeLevels.ContainsKey(upgradeTypeName))
                _upgradeLevels[upgradeTypeName] = 0;

            var currentLevel = _upgradeLevels[upgradeTypeName];
            var upgrade = availableUpgrades.Find(u => u.upgradeTypeName == upgradeTypeName);

            if (upgrade == null || currentLevel >= upgrade.upgradeCosts.Length)
            {
                UnityEngine.Debug.Log("Max Level Reached");
                return;
            }

            var cost = upgrade.upgradeCosts[currentLevel];

            if (PlayerCurrencyManager.CompanyCredits >= cost)
            {
                PlayerCurrencyManager.RemoveCurrency(cost);
                _upgradeLevels[upgradeTypeName]++;
                SaveUpgrades();
                UpdateUI();
            }
            else
            {
                UnityEngine.Debug.Log("Not enough credits!");
            }
        }

        private void UpdateUI()
        {
            // Update UI
        }

        private void SaveUpgrades()
        {
            foreach (var upgrade in _upgradeLevels) ES3.Save(upgrade.Key, upgrade.Value, "UpgradeSave.es3");
        }

        public int GetUpgradeLevel(string upgradeName)
        {
            return _upgradeLevels.ContainsKey(upgradeName) ? _upgradeLevels[upgradeName] : 0;
        }

        public int GetUpgradeCost(string upgradeTypeName)
        {
            var level = GetUpgradeLevel(upgradeTypeName);
            return availableUpgrades.Find(u => u.upgradeTypeName == upgradeTypeName)?.upgradeCosts[level] ?? 9999;
        }

        private void LoadUpgrades()
        {
            foreach (var upgrade in availableUpgrades)
                if (ES3.KeyExists(upgrade.upgradeTypeName, "UpgradeSave.es3"))
                    _upgradeLevels[upgrade.upgradeTypeName] = ES3.Load<int>(upgrade.upgradeTypeName, "UpgradeSave.es3");
                else
                    _upgradeLevels[upgrade.upgradeTypeName] = 0;
        }

        public string GetUpgradeName(string upgradeTypeName)
        {
            throw new NotImplementedException();
        }
    }
}