using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class ChanceToSpawn
{
    public int weight = 0;
    public SlimeDescriptionSO description = null;
}

[Serializable]
public sealed class HardnessLevelDescription
{
    public int requiredSlimesCount = 0;
    public List<ChanceToSpawn> spawnRates = null;
}

[CreateAssetMenu(fileName = "SlimeGeneratorSettingsSO", menuName = "Arena/SlimeGeneratorSettingsSO")]
public class SlimeGeneratorSettingsSO : ScriptableObject
{
    public Rect NormalizedGenerationAreaConstraints = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
    public List<HardnessLevelDescription> hardnessLevelDescriptions = null;
}
