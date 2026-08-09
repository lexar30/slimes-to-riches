using Arena;
using Arena.Entity;
using System;
using UnityEngine;
using UnityEngine.Pool;

public sealed class SlimeGenerator
{
    private SlimeGeneratorSettingsSO settings;

    public SlimeGenerator(SlimeGeneratorSettingsSO settings)
    {
        if (settings == null)
        {
            throw new ArgumentNullException(nameof(settings));
        }

        this.settings = settings;
    }

    public int GetSpawnCount(int currentSlimesCount)
    {
        // some logic here

        return 0;
    }

    public void Initialize(SlimeRuntimeState slimeState)
    {
        if (slimeState == null)
        {
            Debug.Log("[SlimeRuntimeState::Initialize]: Unable to initialize");
            return;
        }

        // some logic here
    }
}
