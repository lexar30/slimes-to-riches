using UnityEngine;
using UnityEngine.UI;

namespace SlimesToRiches.UI
{
    [System.Serializable]
    public sealed class OrientationLayoutSettings
    {
        [SerializeField]
        private GameObject root = null;

        [SerializeField]
        private GameObject infoPanelSlot = null;

        [SerializeField]
        private GameObject gameFieldSlot = null;

        [SerializeField]
        private GameObject upgradesPanelSlot = null;

        public GameObject Root => root;
        public GameObject InfoPanelSlot => infoPanelSlot;
        public GameObject GameFieldSlot => gameFieldSlot;
        public GameObject UpgradesPanelSlot => upgradesPanelSlot;
    }

    public class UILayoutSetter : MonoBehaviour
    {
        [Header("Canvas Settings")]
        [SerializeField]
        private CanvasScaler scaler = null;

        [Space(20)]
        [Header("Layouts Settings")]
        [SerializeField]
        private OrientationLayoutSettings portraitSettings = null;

        [SerializeField]
        private OrientationLayoutSettings landscapeSettings = null;

        private OrientationLayoutSettings currentSettings = null;

        [Space(20)]
        [Header("InGame UI Panels")]
        [SerializeField]
        private GameObject infoPanel = null;

        [SerializeField]
        private GameObject gameField = null;

        [SerializeField]
        private GameObject upgradesPanel = null;

        private float prevAspectRatio = 0.0f;

        private void ResetTransform(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            RectTransform rect = go.GetComponent<RectTransform>();
            if (rect == null)
            {
                return;
            }

            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private bool IsCorrectSettings(OrientationLayoutSettings settings)
        {
            if (settings == null)
            {
                return false;
            }

            if (settings.Root == null || settings.InfoPanelSlot == null || settings.GameFieldSlot == null || settings.UpgradesPanelSlot == null)
            {
                return false;
            }

            return true;
        }

        private void UpdateLayout()
        {
            if (currentSettings != null)
            {
                currentSettings.Root.SetActive(false);
            }

            if (prevAspectRatio > 1.0f)
            {
                scaler.matchWidthOrHeight = 1.0f;

                currentSettings = landscapeSettings;
            }
            else
            {
                scaler.matchWidthOrHeight = 0.0f;

                currentSettings = portraitSettings;
            }

            currentSettings.Root.SetActive(true);

            infoPanel.transform.SetParent(currentSettings.InfoPanelSlot.transform, false);
            ResetTransform(infoPanel);

            gameField.transform.SetParent(currentSettings.GameFieldSlot.transform, false);
            ResetTransform(gameField);

            upgradesPanel.transform.SetParent(currentSettings.UpgradesPanelSlot.transform, false);
            ResetTransform(upgradesPanel);
        }

        private void Start()
        {
            bool isSettingsCorrect = IsCorrectSettings(portraitSettings) && IsCorrectSettings(landscapeSettings);
            bool isPanelsCorrect = infoPanel != null && gameField != null && upgradesPanel != null;

            if (scaler == null || !isSettingsCorrect || !isPanelsCorrect)
            {
                this.enabled = false;
                Debug.LogError("[UILayoutSetter] CanvasScaler, layout sets or panels are not assigned.", this);
                return;
            }

            prevAspectRatio = (float)Screen.width / Screen.height;

            UpdateLayout();
        }

        private void Update()
        {
            float currentAspectRatio = (float)Screen.width / Screen.height;
            bool hasBecomeVertical = prevAspectRatio > 1.0f && currentAspectRatio <= 1.0f;
            bool hasBecomeHorizontal = prevAspectRatio <= 1.0f && currentAspectRatio > 1.0f;

            if (hasBecomeVertical || hasBecomeHorizontal)
            {
                prevAspectRatio = currentAspectRatio;

                UpdateLayout();
            }
        }
    }
}
