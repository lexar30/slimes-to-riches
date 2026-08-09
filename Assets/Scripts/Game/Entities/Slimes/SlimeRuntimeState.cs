using UnityEngine;

namespace Arena.Entity
{
    public sealed class SlimeRuntimeState : MovableRuntimeState
    {
        public int CurrentHP = 0;

        public float MovingTimer = 0.0f;
        public float IdlingTimer = 0.0f;
    }
}