using SlimesToRiches.Arena.Entities;
using SlimesToRiches.Arena.Entities.Slimes;
using SlimesToRiches.Arena.Settings;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace SlimesToRiches.Arena.Core
{
    public class SlimeView : MonoBehaviour
    {
        [SerializeField]
        private SizeScaleTableSO sizeScaleSettings = null;

        [SerializeField]
        private GeneralArenaSettingsSO generalArenaSettings = null;

        [SerializeField]
        private ArenaViewSettingsSO settings = null;

        [SerializeField]
        private RectTransform arenaRectTransform = null;

        private ObjectPool<EntityView> slimesPool;
        private Dictionary<SlimeRuntimeState, EntityView> slimes = new();
        // Later:
        // private Dictionary<SlimeRuntimeState, EntityView> projectiles = new(); projectiles
        // private List<KeyValuePair<GunRuntimeState, EntityView>> guns = new(); pair of gun->view
        // private KeyValuePair<Mouse Pointer, EntityView> guns = new(); mouse

        private void Awake()
        {
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

        public Vector2 NormalizedToAnchoredPosition(Vector2 normalizedPosition)
        {
            if (arenaRectTransform == null)
            {
                throw new ArgumentException(
                    "[SlimeView::NormalizedToAnchoredPosition]: arenaRectTransform not set.",
                    nameof(settings)
                );
            }

            return new Vector2(
                Mathf.Lerp(
                    arenaRectTransform.rect.xMin,
                    arenaRectTransform.rect.xMax,
                    normalizedPosition.x
                ),
                Mathf.Lerp(
                    arenaRectTransform.rect.yMin,
                    arenaRectTransform.rect.yMax,
                    normalizedPosition.y
                )
            );
        }

        private void Sync(EntityView view, SlimeRuntimeState slimeState)
        {
            view.RectTransform.anchoredPosition = NormalizedToAnchoredPosition(slimeState.NormalizedPosition);
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

            view.Image.sprite = slimeState.DescriptionSO.Sprite;
            view.RectTransform.localPosition = Vector3.zero;

            float scale = sizeScaleSettings.GetScaleFor(slimeState.Size);
            view.RectTransform.localScale = new Vector3(scale, scale, 1.0f);

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
                arenaRectTransform,
                false
            );

            EntityView view = new EntityView();

            view.RectTransform = instance.GetComponent<RectTransform>();
            if (view.RectTransform == null)
            {
                throw new ArgumentException(
                    "[SlimeView::Create]: prefab must contain RectTransform.",
                    nameof(settings)
                );
            }

            view.Image = instance.GetComponent<Image>();
            if (view.Image == null)
            {
                throw new ArgumentException(
                    "[SlimeView::Create]: prefab must contain Image.",
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

            view.RectTransform.gameObject.SetActive(false);
        }

        private void OnGet(EntityView view)
        {
            view.RectTransform.gameObject.SetActive(true);
        }

        private void OnPoolEntityDestroy(EntityView view)
        {
            Destroy(view.RectTransform.gameObject);
        }
    }
}
