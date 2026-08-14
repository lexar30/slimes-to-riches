using SlimesToRiches.Arena.Entities.Slimes;
using SlimesToRiches.Arena.Systems;
using System.Collections.Generic;
using UnityEngine;

namespace SlimesToRiches.Arena.Core
{
    public sealed class SlimePhysicsProcessor : MonoBehaviour
    {
        private readonly List<SlimeRuntimeState> activeSlimes = new();
        private readonly Dictionary<SlimeRuntimeState, SlimePhysicsBody> bodies = new();

        public int ActiveSlimesCount => activeSlimes.Count;

        public void ProcessPhysics(float dt)
        {
            foreach (SlimeRuntimeState slimeState in activeSlimes)
            {
                if (bodies.TryGetValue(slimeState, out SlimePhysicsBody body))
                {
                    SlimeMovementProcessor.Update(slimeState, body.Rigidbody, dt);
                }
            }
        }

        public bool Add(SlimeRuntimeState slimeState, SlimePhysicsBody body)
        {
            if (slimeState == null || body == null || bodies.ContainsKey(slimeState))
            {
                return false;
            }

            activeSlimes.Add(slimeState);
            bodies.Add(slimeState, body);

            if (slimeState.State == SlimeState.Wandering)
            {
                SlimeMovementProcessor.ApplyRandomImpulse(slimeState, body.Rigidbody);
            }

            return true;
        }

        public void Remove(SlimeRuntimeState slimeState)
        {
            if (slimeState == null)
            {
                return;
            }

            activeSlimes.Remove(slimeState);
            bodies.Remove(slimeState);
        }
    }
}
