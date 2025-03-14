using UnityEditor;
using UnityEngine;

namespace Domains.Player.Scripts
{
#if UNITY_EDITOR
    public static class PlayerCurrencyManagerDebug
    {
        [MenuItem("Debug/Reset/Reset Health")]
        public static void ResetHealth()
        {
            PlayerHealthManager.ResetPlayerHealth();
        }
    }
#endif
    public class PlayerCurrencyManager : MonoBehaviour
    {
        public static int CompanyCredits;

        private static string GetSaveFilePath()
        {
            return "GameSave.es3"; // Single save file for everything
        }

        public static void AddCoins(int coinsToAdd)
        {
            CompanyCredits += coinsToAdd;
            SavePlayerCurrency();
        }


        public static void RemoveCoins(int coinsToRemove)
        {
            CompanyCredits -= coinsToRemove;
            SavePlayerCurrency();
        }

        public static void SavePlayerCurrency()
        {
            var saveFilePath = GetSaveFilePath();
            ES3.Save("CompanyCredits", CompanyCredits, saveFilePath);
        }
    }
}