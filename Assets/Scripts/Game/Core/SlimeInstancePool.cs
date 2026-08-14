using SlimesToRiches.Arena.Entities;
using SlimesToRiches.Arena.Entities.Slimes;
using SlimesToRiches.Arena.Settings;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace SlimesToRiches.Arena.Core
{
    public sealed class SlimeInstancePool : MonoBehaviour
    {
        [SerializeField]
        private SizeScaleTableSO sizeScaleSettings = null;

        [SerializeField]
        private GeneralArenaSettingsSO generalArenaSettings = null;

        [SerializeField]
        private ArenaViewSettingsSO settings = null;

        [SerializeField]
        private SlimePhysicsProcessor processor = null;

        [SerializeField, Range(0.0f, 1.0f)]
        private float physicsFriction = 0.0f;

        [SerializeField, Range(0.0f, 1.0f)]
        private float physicsBounciness = 0.5f;

        private ObjectPool<SlimePhysicsBody> slimesPool;
        private readonly Dictionary<SlimeRuntimeState, SlimePhysicsBody> slimes = new();
        private PhysicsMaterial2D physicsMaterial;

        private void Awake()
        {
            if (sizeScaleSettings == null)
            {
                throw new ArgumentNullException(nameof(sizeScaleSettings));
            }

            if (generalArenaSettings == null)
            {
                throw new ArgumentNullException(nameof(generalArenaSettings));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            if (settings.ViewPrefab == null)
            {
                throw new ArgumentNullException(nameof(settings.ViewPrefab));
            }

            if (processor == null)
            {
                throw new ArgumentNullException(nameof(processor));
            }

            physicsMaterial = new PhysicsMaterial2D("Slime Physics Material")
            {
                friction = physicsFriction,
                bounciness = physicsBounciness
            };

            slimesPool = new ObjectPool<SlimePhysicsBody>(
                createFunc: OnCreate
                , actionOnRelease: OnRelease
                , actionOnGet: OnGet
                , actionOnDestroy: OnPoolEntityDestroy
                , collectionCheck: true
                , defaultCapacity: generalArenaSettings.DefaultPoolCapacity
                , maxSize: generalArenaSettings.MaxPoolCapacity
            );
        }

        public void Add(SlimeSpawnRequest spawnRequest)
        {
            if (spawnRequest?.State == null)
            {
                return;
            }

            SlimePhysicsBody body = slimesPool.Get();
            if (body == null)
            {
                Debug.Log("[SlimeInstancePool::Add]: pool was unable to generate SlimePhysicsBody.");
                return;
            }

            SlimeRuntimeState slimeState = spawnRequest.State;
            float scale = sizeScaleSettings.GetScaleFor(slimeState.Size);
            Vector2 worldPosition = transform.TransformPoint(spawnRequest.ArenaPosition);

            body.Bind(
                slimeState,
                worldPosition,
                scale,
                physicsMaterial
            );

            if (!processor.Add(slimeState, body))
            {
                slimesPool.Release(body);
                return;
            }

            slimes.Add(slimeState, body);
        }

        public void Remove(SlimeRuntimeState slimeState)
        {
            if (slimes.TryGetValue(slimeState, out SlimePhysicsBody body))
            {
                slimes.Remove(slimeState);
                processor.Remove(slimeState);
                slimesPool.Release(body);
            }
        }

        private SlimePhysicsBody OnCreate()
        {
            GameObject instance = Instantiate(
                settings.ViewPrefab,
                transform,
                false
            );

            SlimePhysicsBody body = instance.GetComponent<SlimePhysicsBody>();
            if (body == null)
            {
                body = instance.AddComponent<SlimePhysicsBody>();
            }

            return body;
        }

        private void OnRelease(SlimePhysicsBody body)
        {
            if (body == null)
            {
                return;
            }

            body.Unbind();
            body.gameObject.SetActive(false);
        }

        private void OnGet(SlimePhysicsBody body)
        {
            body.gameObject.SetActive(true);
        }

        private void OnPoolEntityDestroy(SlimePhysicsBody body)
        {
            Destroy(body.gameObject);
        }

        private void OnDestroy()
        {
            if (physicsMaterial != null)
            {
                Destroy(physicsMaterial);
            }
        }
    }
}
