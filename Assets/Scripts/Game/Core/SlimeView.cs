using SlimesToRiches.Arena.Entities;
using SlimesToRiches.Arena.Entities.Slimes;
using SlimesToRiches.Arena.Settings;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace SlimesToRiches.Arena.Core
{
    public sealed class SlimeView : MonoBehaviour
    {
        [SerializeField]
        private SizeScaleTableSO sizeScaleSettings = null;

        [SerializeField]
        private GeneralArenaSettingsSO generalArenaSettings = null;

        [SerializeField]
        private ArenaViewSettingsSO settings = null;

        [SerializeField]
        private Vector2 arenaSize = new(8.0f, 8.0f);

        private ObjectPool<EntityView> slimesPool;
        private readonly Dictionary<SlimeRuntimeState, EntityView> slimes = new();

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

            if (arenaSize.x <= 0.0f || arenaSize.y <= 0.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(arenaSize));
            }

            slimesPool = new ObjectPool<EntityView>(
                createFunc: OnCreate
                , actionOnRelease: OnRelease
                , actionOnGet: OnGet
                , actionOnDestroy: OnPoolEntityDestroy
                , collectionCheck: true
                , defaultCapacity: generalArenaSettings.DefaultPoolCapacity
                , maxSize: generalArenaSettings.MaxPoolCapacity
            );
        }

        public Vector3 NormalizedToLocalPosition(Vector2 normalizedPosition)
        {
            return new Vector3(
                (normalizedPosition.x - 0.5f) * arenaSize.x,
                (normalizedPosition.y - 0.5f) * arenaSize.y,
                0.0f
            );
        }

        private void Sync(EntityView view, SlimeRuntimeState slimeState)
        {
            view.Transform.localPosition = NormalizedToLocalPosition(slimeState.NormalizedPosition);
        }

        public void Sync()
        {
            foreach (var (slimeState, view) in slimes)
            {
                Sync(view, slimeState);
            }
        }

        public void Add(SlimeRuntimeState slimeState)
        {
            EntityView view = slimesPool.Get();
            if (view == null)
            {
                Debug.Log("[SlimeView::Add]: pool was unable to generate EntityView for SlimeRuntimeState.");
                return;
            }

            view.SpriteRenderer.sprite = slimeState.DescriptionSO.Sprite;
            view.Transform.localPosition = Vector3.zero;

            float scale = sizeScaleSettings.GetScaleFor(slimeState.Size);
            view.Transform.localScale = new Vector3(
                view.BaseScale.x * scale,
                view.BaseScale.y * scale,
                view.BaseScale.z
            );

            slimes.Add(slimeState, view);

            Sync(view, slimeState);
        }

        public void Remove(SlimeRuntimeState slimeState)
        {
            EntityView view = null;

            if (slimes.TryGetValue(slimeState, out view))
            {
                slimes.Remove(slimeState);
                slimesPool.Release(view);
            }
        }

        private EntityView OnCreate()
        {
            GameObject instance = Instantiate(
                settings.ViewPrefab,
                transform,
                false
            );

            EntityView view = new EntityView
            {
                Transform = instance.transform,
                SpriteRenderer = instance.GetComponent<SpriteRenderer>(),
                BaseScale = instance.transform.localScale
            };

            if (view.SpriteRenderer == null)
            {
                throw new ArgumentException(
                    "[SlimeView::Create]: prefab must contain SpriteRenderer.",
                    nameof(settings)
                );
            }

            return view;
        }

        private void OnRelease(EntityView view)
        {
            if (view == null)
            {
                return;
            }

            view.Transform.gameObject.SetActive(false);
        }

        private void OnGet(EntityView view)
        {
            view.Transform.gameObject.SetActive(true);
        }

        private void OnPoolEntityDestroy(EntityView view)
        {
            Destroy(view.Transform.gameObject);
        }
    }
}
