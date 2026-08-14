using UnityEngine;

namespace SlimesToRiches.Arena.Settings
{
    [CreateAssetMenu(fileName = "ArenaSettingsSO", menuName = "Arena/ArenaSettingsSO")]
    public class ArenaSettingsSO : ScriptableObject
    {
        public int columnsCount = 0;
        public int rowsCount = 0;

        public int borderSizeInCells = 1;

        public int TotalColumnsCount => columnsCount + borderSizeInCells * 2;
        public int TotalRowsCount => rowsCount + borderSizeInCells * 2;
    }
}
