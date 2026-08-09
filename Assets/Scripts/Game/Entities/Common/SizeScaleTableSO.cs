using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class SizeScale
{
    public int Size = 0;
    public float Scale = 0.0f;
}

[CreateAssetMenu(fileName = "SizeScaleTableSO", menuName = "Arena/SizeScaleTableSO")]
public class SizeScaleTableSO : ScriptableObject
{
    public List<SizeScale> Table = null;

    public float GetScaleFor(int Size)
    {
        if (Table == null || Table.Count == 0)
        {
            Debug.Log("[SizeScaleTableSO::GetScaleFor]: Table is empty.");
            return 1.0f;
        }

        for (int i = 1; i < Table.Count; ++i)
        {
            if (Size < Table[i].Size)
            {
                return Table[i - 1].Scale;
            }
        }

        return Table[0].Scale;
    }
}
