namespace SlimesToRiches.Arena.Entities.Slimes
{
    public enum SlimeState
    {
        Idling
        , Wandering
        , Fleeing
    }

    public sealed class SlimeRuntimeState
    {
        public int CurrentHP = 0;
        public SlimeState State = SlimeState.Idling;
        public float CurrentTimer = 0.0f;
        public int Size = 0;
        public SlimeDescriptionSO DescriptionSO;
    }
}
