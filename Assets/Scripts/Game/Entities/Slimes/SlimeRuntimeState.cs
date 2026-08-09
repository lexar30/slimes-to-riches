using UnityEngine;

namespace Arena.Entity
{
    public enum SlimeState
    {
        Idle
        , Moving
    }

    public sealed class SlimeRuntimeState : MovableRuntimeState
    {
        public int CurrentHP = 0;
        public SlimeState State = SlimeState.Idle;
        public float CurrentTimer = 0.0f;
        public int Size = 0;
        public SlimeDescriptionSO DescriptionSO;
    }
}