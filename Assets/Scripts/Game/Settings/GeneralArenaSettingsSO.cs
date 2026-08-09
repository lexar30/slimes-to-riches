using UnityEngine;

[CreateAssetMenu(fileName = "GeneralArenaSettingsSO", menuName = "Arena")]
public class GeneralArenaSettingsSO : ScriptableObject
{
    public int DefaultPoolCapacity = 32;
    public int MaxPoolCapacity = 256;
}
