using Gameplay.Config;
using UnityEditor;

namespace Domains.Debug
{
    public static class DebugMenu
    {
        [MenuItem("Debug/Clear All Save Data")]
        public static void ClearAllSaveData()
        {
            DataReset.ClearAllSaveData();
        }
    }
}
