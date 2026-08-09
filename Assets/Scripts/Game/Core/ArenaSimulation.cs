using Arena;
using Arena.Entity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.Pool;

public class ArenaSimulation : MonoBehaviour
{
    [SerializeField]
    private ArenaSettingsSO settings;

    [SerializeField]
    private SlimeGeneratorSettingsSO generatorSettings;

    [SerializeField]
    private GeneralArenaSettingsSO generalArenaSettings;

    private ObjectPool<SlimeRuntimeState> slimesPool;
    private SlimeGenerator generator;
    private ArenaWorld world;

    private void Start()
    {
        world = new ArenaWorld(settings.columnsCount, settings.rowsCount);
        generator = new SlimeGenerator(generatorSettings);

        slimesPool = new ObjectPool<SlimeRuntimeState>(
            createFunc: () => new SlimeRuntimeState()
            , actionOnRelease: ReleaseSlime
            , actionOnGet: null
            , actionOnDestroy: null
            , collectionCheck: true
            , defaultCapacity: generalArenaSettings.DefaultPoolCapacity
            , maxSize: generalArenaSettings.MaxPoolCapacity
        );
    }

    private void Update()
    {
        world.Update(Time.deltaTime);
        TryGenerateSlimes();
    }

    private void TryGenerateSlimes()
    {
        int requiredCount = generator.GetSpawnCount(world.ActiveSlimesCount);
        if (requiredCount == 0)
        {
            return;
        }

        while (requiredCount > 0)
        {
            SlimeRuntimeState slimeState = slimesPool.Get();
            if (slimeState == null)
            {
                Debug.Log("[ArenaSimulation::TryGenerateSlimes]: pool was unable to generate SlimeRuntimeState");
                return;
            }

            generator.Initialize(slimeState);
            world.AddSlime(slimeState);

            --requiredCount;
        }
    }

    private void ReleaseSlime(SlimeRuntimeState slimeState)
    {
        world.RemoveSlime(slimeState);
    }
}
