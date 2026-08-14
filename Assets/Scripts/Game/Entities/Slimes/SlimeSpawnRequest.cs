using UnityEngine;

namespace SlimesToRiches.Arena.Entities.Slimes
{
    public sealed class SlimeSpawnRequest
    {
        public SlimeRuntimeState State { get; }
        public Vector2 ArenaPosition { get; }

        public SlimeSpawnRequest(SlimeRuntimeState state, Vector2 arenaPosition)
        {
            State = state;
            ArenaPosition = arenaPosition;
        }
    }
}
