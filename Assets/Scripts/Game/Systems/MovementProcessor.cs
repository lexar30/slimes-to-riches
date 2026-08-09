using Arena.Entity;
using UnityEngine;

public static class MovementProcessor
{
    public static void Update(SlimeRuntimeState slimeState, float dt)
    {
        Vector2 nextPosition = slimeState.NormalizedPosition + slimeState.Velocity * dt;

        slimeState.NormalizedPosition = new Vector2(
            Mathf.Clamp01(nextPosition.x),
            Mathf.Clamp01(nextPosition.y)
        );
    }
}
