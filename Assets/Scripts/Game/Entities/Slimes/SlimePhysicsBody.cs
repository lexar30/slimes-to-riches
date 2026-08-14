using System;
using UnityEngine;

namespace SlimesToRiches.Arena.Entities.Slimes
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public sealed class SlimePhysicsBody : MonoBehaviour
    {
        public SlimeRuntimeState State { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }
        public Rigidbody2D Rigidbody { get; private set; }
        public CircleCollider2D Collider { get; private set; }
        public Vector3 BaseScale { get; private set; }

        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            Rigidbody = GetComponent<Rigidbody2D>();
            Collider = GetComponent<CircleCollider2D>();
            BaseScale = transform.localScale;

            if (SpriteRenderer == null || Rigidbody == null || Collider == null)
            {
                throw new InvalidOperationException(
                    "[SlimePhysicsBody]: SpriteRenderer, Rigidbody2D and CircleCollider2D are required."
                );
            }

            Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            Rigidbody.gravityScale = 0.0f;
            Rigidbody.freezeRotation = true;
            Rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        public void Bind(
            SlimeRuntimeState state,
            Vector2 worldPosition,
            float scale,
            PhysicsMaterial2D physicsMaterial)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));

            Sprite sprite = state.DescriptionSO.Sprite;
            SpriteRenderer.sprite = sprite;

            transform.localScale = new Vector3(
                BaseScale.x * scale,
                BaseScale.y * scale,
                BaseScale.z
            );

            float pixelsPerUnit = sprite != null ? sprite.pixelsPerUnit : 1.0f;
            Collider.radius = state.DescriptionSO.CollisionRadius / pixelsPerUnit;
            Collider.offset = state.DescriptionSO.CollisionOffset / pixelsPerUnit;
            Collider.sharedMaterial = physicsMaterial;

            Rigidbody.position = worldPosition;
            Rigidbody.rotation = 0.0f;
            Rigidbody.linearVelocity = Vector2.zero;
            Rigidbody.angularVelocity = 0.0f;
            Rigidbody.WakeUp();
        }

        public void Unbind()
        {
            Rigidbody.linearVelocity = Vector2.zero;
            Rigidbody.angularVelocity = 0.0f;
            State = null;
        }
    }
}
