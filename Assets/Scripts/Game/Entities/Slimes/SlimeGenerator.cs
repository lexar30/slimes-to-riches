using SlimesToRiches.Arena.Core;
using SlimesToRiches.Arena.Settings;
using SlimesToRiches.Arena.Systems;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace SlimesToRiches.Arena.Entities.Slimes
{
    public sealed class SlimeGenerator : MonoBehaviour
    {
        [SerializeField]
        private SlimeGeneratorSettingsSO settings;

        [SerializeField]
        private GeneralArenaSettingsSO generalArenaSettings;

        [SerializeField]
        private ArenaWorld world;

        [SerializeField]
        private UnityEvent<SlimeRuntimeState> slimeCreated = new();

        private ObjectPool<SlimeRuntimeState> slimesPool;
        private HardnessLevelDescription currentHardnessLevel = null;

        private void Awake()
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (generalArenaSettings == null)
            {
                throw new ArgumentNullException(nameof(generalArenaSettings));
            }

            if (world == null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            ValidateSettings(settings);

            currentHardnessLevel = settings.hardnessLevelDescriptions[0];

            slimesPool = new ObjectPool<SlimeRuntimeState>(
                createFunc: () => new SlimeRuntimeState()
                , actionOnRelease: null
                , actionOnGet: null
                , actionOnDestroy: null
                , collectionCheck: true
                , defaultCapacity: generalArenaSettings.DefaultPoolCapacity
                , maxSize: generalArenaSettings.MaxPoolCapacity
            );
        }

        private static void ValidateSettings(SlimeGeneratorSettingsSO settings)
        {
            if (settings.hardnessLevelDescriptions == null || settings.hardnessLevelDescriptions.Count == 0)
            {
                throw new ArgumentException(
                    "[SlimeGenerator]: hardnessLevelDescriptions must contain at least one level.",
                    nameof(settings)
                );
            }

            for (int levelIndex = 0; levelIndex < settings.hardnessLevelDescriptions.Count; ++levelIndex)
            {
                HardnessLevelDescription level = settings.hardnessLevelDescriptions[levelIndex];
                if (level == null)
                {
                    throw new ArgumentException(
                        $"[SlimeGenerator]: Hardness level at index {levelIndex} is null.",
                        nameof(settings)
                    );
                }

                if (level.requiredSlimesCount < 0)
                {
                    throw new ArgumentException(
                        $"[SlimeGenerator]: requiredSlimesCount at level {levelIndex} cannot be negative.",
                        nameof(settings)
                    );
                }

                if (level.spawnRates == null)
                {
                    throw new ArgumentException(
                        $"[SlimeGenerator]: Spawn rates at level {levelIndex} are null.",
                        nameof(settings)
                    );
                }

                if (level.spawnRates.Count == 0)
                {
                    if (level.requiredSlimesCount > 0)
                    {
                        throw new ArgumentException(
                            $"[SlimeGenerator]: Level {levelIndex} requires slimes but has no spawn rates.",
                            nameof(settings)
                        );
                    }

                    continue;
                }

                long totalWeight = 0;
                for (int spawnRateIndex = 0; spawnRateIndex < level.spawnRates.Count; ++spawnRateIndex)
                {
                    ChanceToSpawn spawnRate = level.spawnRates[spawnRateIndex];
                    if (spawnRate == null)
                    {
                        throw new ArgumentException(
                            $"[SlimeGenerator]: Spawn rate {spawnRateIndex} at level {levelIndex} is null.",
                            nameof(settings)
                        );
                    }

                    if (spawnRate.weight < 0)
                    {
                        throw new ArgumentException(
                            $"[SlimeGenerator]: Weight at level {levelIndex}, entry {spawnRateIndex} cannot be negative.",
                            nameof(settings)
                        );
                    }

                    if (spawnRate.description == null)
                    {
                        throw new ArgumentException(
                            $"[SlimeGenerator]: Slime description at level {levelIndex}, entry {spawnRateIndex} is null.",
                            nameof(settings)
                        );
                    }

                    totalWeight += spawnRate.weight;
                }

                if (level.requiredSlimesCount > 0 && totalWeight <= 0)
                {
                    throw new ArgumentException(
                        $"[SlimeGenerator]: Total spawn weight at level {levelIndex} must be positive.",
                        nameof(settings)
                    );
                }

                if (totalWeight > int.MaxValue)
                {
                    throw new ArgumentException(
                        $"[SlimeGenerator]: Total spawn weight at level {levelIndex} exceeds Int32.MaxValue.",
                        nameof(settings)
                    );
                }
            }
        }

        public void IncreaseHardnessLevel()
        {
            int currentHardnessLevelIndex = settings.hardnessLevelDescriptions.IndexOf(currentHardnessLevel);
            if (currentHardnessLevelIndex + 1 >= settings.hardnessLevelDescriptions.Count)
            {
                return;
            }

            currentHardnessLevel = settings.hardnessLevelDescriptions[currentHardnessLevelIndex + 1];
        }

        public int GetSpawnCount(int currentSlimesCount)
        {
            int requiredCount = currentHardnessLevel.requiredSlimesCount - currentSlimesCount;
            if (requiredCount < 0)
            {
                return 0;
            }

            return requiredCount;
        }

        public void Generate()
        {
            int requiredCount = GetSpawnCount(world.ActiveSlimesCount);

            while (requiredCount > 0)
            {
                SlimeRuntimeState slimeState = slimesPool.Get();
                if (slimeState == null)
                {
                    Debug.Log("[SlimeGenerator::Generate]: pool was unable to generate SlimeRuntimeState.");
                    return;
                }

                if (!TryInitialize(slimeState))
                {
                    slimesPool.Release(slimeState);
                    return;
                }

                slimeCreated?.Invoke(slimeState);
                --requiredCount;
            }
        }

        public void Release(SlimeRuntimeState slimeState)
        {
            if (slimeState == null)
            {
                return;
            }

            slimesPool.Release(slimeState);
        }

        private SlimeDescriptionSO PickSlimeDescription()
        {
            int totalWeight = 0;
            foreach (ChanceToSpawn spawnRate in currentHardnessLevel.spawnRates)
            {
                totalWeight += spawnRate.weight;
            }

            if (totalWeight <= 0)
            {
                return null;
            }

            int rndValue = UnityEngine.Random.Range(0, totalWeight);
            int currentWeight = 0;
            foreach (ChanceToSpawn spawnRate in currentHardnessLevel.spawnRates)
            {
                currentWeight += spawnRate.weight;
                if (rndValue < currentWeight)
                {
                    return spawnRate.description;
                }
            }

            return null;
        }

        public bool TryInitialize(SlimeRuntimeState slimeState)
        {
            if (slimeState == null)
            {
                Debug.Log("[SlimeRuntimeState::Initialize]: Unable to initialize");
                return false;
            }

            SlimeDescriptionSO description = PickSlimeDescription();
            if (description == null)
            {
                Debug.Log("[SlimeRuntimeState::Initialize]: None slime description was picked. Check settings");
                return false;
            }

            Rect spawnArea = settings.NormalizedGenerationAreaConstraints;
            slimeState.NormalizedPosition =
                new Vector2(
                    UnityEngine.Random.Range(spawnArea.xMin, spawnArea.xMax)
                    , UnityEngine.Random.Range(spawnArea.yMin, spawnArea.yMax)
                );

            slimeState.Velocity = Vector2.zero;
            slimeState.CurrentHP = description.MaxHP;
            slimeState.Size = UnityEngine.Random.Range(description.SizeMin, description.SizeMax + 1);

            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                slimeState.State = SlimeState.Idle;
                slimeState.CurrentTimer = UnityEngine.Random.Range(description.IdlingTimeMin, description.IdlingTimeMax);
            }
            else
            {
                slimeState.State = SlimeState.Moving;
                slimeState.CurrentTimer = UnityEngine.Random.Range(description.MovingTimeMin, description.MovingTimeMax);
                slimeState.Velocity = MovementProcessor.GenerateRandomVelocity(description.SpeedMin, description.SpeedMax);
            }

            slimeState.DescriptionSO = description;

            return true;
        }
    }
}
