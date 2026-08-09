using Arena;
using Arena.Entity;
using UnityEngine;
using UnityEngine.Pool;

public class ArenaSimulation : MonoBehaviour
{
    [SerializeField]
    private ArenaView view;

    [SerializeField]
    private ArenaSettingsSO settings;

    [SerializeField]
    private SlimeGeneratorSettingsSO generatorSettings;

    [SerializeField]
    private GeneralArenaSettingsSO generalArenaSettings;

    private ObjectPool<SlimeRuntimeState> slimesPool;
    private SlimeGenerator generator;
    private ArenaWorld world;

    private void Awake()
    {
        slimesPool = new ObjectPool<SlimeRuntimeState>(
            createFunc: () => new SlimeRuntimeState()
            , actionOnRelease: Release
            , actionOnGet: null
            , actionOnDestroy: null
            , collectionCheck: true
            , defaultCapacity: generalArenaSettings.DefaultPoolCapacity
            , maxSize: generalArenaSettings.MaxPoolCapacity
        );
    }

    private void Start()
    {
        world = new ArenaWorld(settings.columnsCount, settings.rowsCount);
        generator = new SlimeGenerator(generatorSettings);
    }

    private void Update()
    {
        TryGenerateSlimes();
        world.Update(Time.deltaTime);
        view.Sync();
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

            if (!generator.TryInitialize(slimeState))
            {
                slimesPool.Release(slimeState);
                return;
            }

            world.Add(slimeState);
            view.Add(slimeState);

            --requiredCount;
        }
    }

    private void Release(SlimeRuntimeState slimeState)
    {
        world.Remove(slimeState);
        view.Remove(slimeState);
    }
}
