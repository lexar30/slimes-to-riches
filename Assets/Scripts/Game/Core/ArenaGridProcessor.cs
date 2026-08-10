using SlimesToRiches.Arena.Entities.Slimes;
using SlimesToRiches.Arena.Settings;
using SlimesToRiches.Arena.Spatial;
using SlimesToRiches.Arena.Systems;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlimesToRiches.Arena.Core
{
    public sealed class ArenaGridProcessor : MonoBehaviour
    {
        [SerializeField]
        private ArenaSettingsSO settings;

        private readonly List<SlimeRuntimeState> activeSlimes = new();
        private SlimeUniformGrid grid;

        public int ActiveSlimesCount => activeSlimes.Count;

        private void Awake()
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (settings.columnsCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.columnsCount));
            }

            if (settings.rowsCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(settings.rowsCount));
            }

            grid = new SlimeUniformGrid(settings.columnsCount, settings.rowsCount);
        }

        public void ProcessMovement(float dt)
        {
            if (activeSlimes.Count == 0)
            {
                return;
            }

            foreach (SlimeRuntimeState slime in activeSlimes)
            {
                MovementProcessor.Update(slime, dt);
                grid.UpdateCell(slime);
            }
        }

        public void Add(SlimeRuntimeState slimeState)
        {
            activeSlimes.Add(slimeState);
            grid.Add(slimeState);
        }

        public void Remove(SlimeRuntimeState slimeState)
        {
            activeSlimes.Remove(slimeState);
            grid.Remove(slimeState);
        }
    }
}
