using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class ChanceToSpawn
{
    public float weight = 0.0f;
    public SlimeDescriptionSO element = null;
}

[Serializable]
public sealed class HardnessLevelDescription
{
    public int hardnessLevel = -1;
    public List<ChanceToSpawn> spawnRates = null;
}

[CreateAssetMenu(fileName = "SlimeGeneratorSettingsSO", menuName = "Arena")]
public class SlimeGeneratorSettingsSO : ScriptableObject
{
    public List<HardnessLevelDescription> hardnessLevelDescriptions = null;
}
