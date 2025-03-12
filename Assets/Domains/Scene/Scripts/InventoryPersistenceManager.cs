#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace Domains.Scene.Scripts
{
    public static class InventoryPersistenceManagerDebug
    {
        [MenuItem("Debug/Reset Inventory")]
        public static void ResetInventory()
        {
            InventoryPersistenceManager.ResetInventory();
        }
    }
#endif

    public class InventoryPersistenceManager : MonoBehaviour
    {
        

        public void SaveInventory()
        {
            throw new NotImplementedException();
        }
        public bool HasSavedData()
        {
            throw new NotImplementedException();
        }
        public void LoadInventory()
        {
            throw new NotImplementedException();
        }
        public static void ResetInventory()
        {
            throw new NotImplementedException();
        }
    }
}
