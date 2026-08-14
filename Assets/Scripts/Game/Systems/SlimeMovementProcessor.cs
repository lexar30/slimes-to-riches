using SlimesToRiches.Arena.Entities.Slimes;
using UnityEngine;

namespace SlimesToRiches.Arena.Systems
{
    public static class SlimeMovementProcessor
    {
        public static Vector2 GenerateRandomVelocity(float speedMin, float speedMax)
        {
            float angle = UnityEngine.Random.Range(0.0f, Mathf.PI * 2.0f);
            float speed = UnityEngine.Random.Range(speedMin, speedMax);

            return new Vector2(
                speed * Mathf.Cos(angle),
                speed * Mathf.Sin(angle)
            );
        }

        public static void ApplyRandomImpulse(SlimeRuntimeState slimeState, Rigidbody2D body)
        {
            Vector2 targetVelocity = GenerateRandomVelocity(
                slimeState.DescriptionSO.SpeedMin,
                slimeState.DescriptionSO.SpeedMax
            );

            Vector2 impulse = (targetVelocity - body.linearVelocity) * body.mass;
            body.AddForce(impulse, ForceMode2D.Impulse);
        }

        private static void UpdateIdle(SlimeRuntimeState slimeState, Rigidbody2D body)
        {
            if (slimeState.CurrentTimer > 0.0f)
            {
                return;
            }

            slimeState.State = SlimeState.Wandering;
            slimeState.CurrentTimer = UnityEngine.Random.Range(
                slimeState.DescriptionSO.MovingTimeMin,
                slimeState.DescriptionSO.MovingTimeMax
            );

            ApplyRandomImpulse(slimeState, body);
        }

        private static void UpdateMoving(SlimeRuntimeState slimeState, Rigidbody2D body)
        {
            if (slimeState.CurrentTimer > 0.0f)
            {
                return;
            }

            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0.0f;

            slimeState.State = SlimeState.Idling;
            slimeState.CurrentTimer = UnityEngine.Random.Range(
                slimeState.DescriptionSO.IdlingTimeMin,
                slimeState.DescriptionSO.IdlingTimeMax
            );
        }

        public static void Update(SlimeRuntimeState slimeState, Rigidbody2D body, float dt)
        {
            if (slimeState == null || body == null)
            {
                return;
            }

            slimeState.CurrentTimer -= dt;

            if (slimeState.State == SlimeState.Wandering)
            {
                UpdateMoving(slimeState, body);
            }
            else
            {
                UpdateIdle(slimeState, body);
            }
        }
    }
}
