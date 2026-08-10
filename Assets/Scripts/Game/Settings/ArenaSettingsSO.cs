using UnityEngine;

namespace SlimesToRiches.Arena.Settings
{
    [CreateAssetMenu(fileName = "ArenaSettingsSO", menuName = "Arena/ArenaSettingsSO")]
    public class ArenaSettingsSO : ScriptableObject
    {
        public int columnsCount = 0;
        public int rowsCount = 0;
    }
}
