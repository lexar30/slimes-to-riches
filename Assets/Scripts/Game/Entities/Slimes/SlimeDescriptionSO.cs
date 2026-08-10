using UnityEngine;

namespace SlimesToRiches.Arena.Entities.Slimes
{
    [CreateAssetMenu(fileName = "SlimeDescriptionSO", menuName = "Arena/SlimeDescriptionSO")]
    public class SlimeDescriptionSO : ScriptableObject
    {
        public float SpeedMin = 0.0f;
        public float SpeedMax = 0.0f;

        public int MaxHP = 0;

        public float MovingTimeMin = 0.0f;
        public float MovingTimeMax = 0.0f;

        public float IdlingTimeMin = 0.0f;
        public float IdlingTimeMax = 0.0f;

        public int SizeMin = 0;
        public int SizeMax = 0;

        public Sprite Sprite = null;

        public float CollisionRadius = 0.0f;
        public Vector2 CollisionOffset = Vector2.zero;
    }
}
