using Arena.Entity;
using System;
using System.Collections.Generic;

namespace Arena
{
    public sealed class ArenaWorld
    {
        private List<SlimeRuntimeState> activeSlimes;
        private SlimeUniformGrid grid;

        public int ActiveSlimesCount => activeSlimes.Count;

        public ArenaWorld(int columnsCount, int rowsCount)
        {
            if (columnsCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columnsCount));
            }

            if (rowsCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowsCount));
            }

            grid = new SlimeUniformGrid(columnsCount, rowsCount);
            activeSlimes = new List<SlimeRuntimeState>();
        }

        public void Update(float dt)
        {
            if (activeSlimes.Count == 0)
            {
                return;
            }

            foreach (SlimeRuntimeState slime in activeSlimes)
            {
                MovementProcessor.Update(slime, dt);
            }
        }

        public void AddSlime(SlimeRuntimeState slimeState)
        {
            activeSlimes.Add(slimeState);
            grid.Add(slimeState);
        }

        public void RemoveSlime(SlimeRuntimeState slimeState)
        {
            activeSlimes.Remove(slimeState);
            grid.Remove(slimeState);
        }
    }
}