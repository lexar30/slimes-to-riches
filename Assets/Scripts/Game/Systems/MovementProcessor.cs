using SlimesToRiches.Arena.Entities.Slimes;
using UnityEngine;

namespace SlimesToRiches.Arena.Systems
{
    public static class MovementProcessor
    {
        public static Vector2 GenerateRandomVelocity(float speedMin, float speedMax)
        {
            float angle = UnityEngine.Random.Range(0, Mathf.PI * 2.0f);
            float speed = UnityEngine.Random.Range(speedMin, speedMax);

            return new Vector2(
                    speed * Mathf.Cos(angle)
                    , speed * Mathf.Sin(angle)
                );
        }

        private static void UpdateIdle(SlimeRuntimeState slimeState)
        {
            if (slimeState.CurrentTimer > 0.01f)
            {
                return;
            }

            slimeState.State = SlimeState.Wandering;

            slimeState.Velocity =
                GenerateRandomVelocity(
                    slimeState.DescriptionSO.SpeedMin
                    , slimeState.DescriptionSO.SpeedMax
                );

            slimeState.CurrentTimer =
                UnityEngine.Random.Range(
                    slimeState.DescriptionSO.MovingTimeMin
                    , slimeState.DescriptionSO.MovingTimeMax
                );
        }

        private static void UpdateMoving(SlimeRuntimeState slimeState, float dt)
        {
            if (slimeState.CurrentTimer <= 0.01f)
            {
                slimeState.Velocity = Vector2.zero;
                slimeState.State = SlimeState.Idling;
                slimeState.CurrentTimer =
                    UnityEngine.Random.Range(
                        slimeState.DescriptionSO.IdlingTimeMin
                        , slimeState.DescriptionSO.IdlingTimeMax
                    );

                return;
            }

            Vector2 nextPosition = slimeState.NormalizedPosition + slimeState.Velocity * dt;

            slimeState.NormalizedPosition = new Vector2(
                Mathf.Clamp01(nextPosition.x),
                Mathf.Clamp01(nextPosition.y)
            );
        }

        public static void Update(SlimeRuntimeState slimeState, float dt)
        {
            if (slimeState == null)
            {
                return;
            }

            if (slimeState.State == SlimeState.Wandering)
            {
                UpdateMoving(slimeState, dt);
            }
            else
            {
                UpdateIdle(slimeState);
            }

            slimeState.CurrentTimer -= dt;
        }
    }
}
