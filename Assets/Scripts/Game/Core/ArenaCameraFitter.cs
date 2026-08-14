using UnityEngine;
using SlimesToRiches.Arena.Settings;

namespace SlimesToRiches.Arena
{
    public sealed class ArenaCameraFitter : MonoBehaviour
    {
        [SerializeField]
        private ArenaSettingsSO arenaSettings = null;

        [SerializeField]
        private Camera gameCamera = null;

        private readonly Vector3[] availableSpaceCorners = new Vector3[4];

        private Rect CalculateScreenRect(RectTransform availableSpace)
        {
            availableSpace.GetWorldCorners(availableSpaceCorners);

            Vector2 bottomLeft = RectTransformUtility.WorldToScreenPoint(
                null,
                availableSpaceCorners[0]
            );
            Vector2 topRight = RectTransformUtility.WorldToScreenPoint(
                null,
                availableSpaceCorners[2]
            );

            return Rect.MinMaxRect(
                bottomLeft.x,
                bottomLeft.y,
                topRight.x,
                topRight.y
            );
        }

        private float CalculatePixelsPerUnit(Rect availableScreenRect)
        {
            float pixelsPerUnitX = availableScreenRect.width / arenaSettings.TotalColumnsCount;
            float pixelsPerUnitY = availableScreenRect.height / arenaSettings.TotalRowsCount;

            return Mathf.Min(pixelsPerUnitX, pixelsPerUnitY);
        }

        private void UpdateCameraSize(float pixelsPerUnit)
        {
            gameCamera.orthographicSize = Screen.height / (pixelsPerUnit * 2.0f);
        }

        private void UpdateCameraPosition(Rect availableScreenRect, float pixelsPerUnit)
        {
            Vector2 screenCenter = new Vector2(
                Screen.width * 0.5f,
                Screen.height * 0.5f
            );

            Vector2 screenOffset = availableScreenRect.center - screenCenter;
            Vector2 worldOffset = screenOffset / pixelsPerUnit;

            Vector3 cameraPosition = gameCamera.transform.position;

            cameraPosition.x = transform.position.x - worldOffset.x;
            cameraPosition.y = transform.position.y - worldOffset.y;

            gameCamera.transform.position = cameraPosition;
        }

        public void OnRectTransformChange(RectTransform rectTransform)
        {
            if (rectTransform == null || arenaSettings == null || gameCamera == null)
            {
                return;
            }

            if (arenaSettings.TotalColumnsCount <= 0 || arenaSettings.TotalRowsCount <= 0)
            {
                return;
            }

            Rect availableScreenRect = CalculateScreenRect(rectTransform);
            float pixelsPerUnit = CalculatePixelsPerUnit(availableScreenRect);

            if (pixelsPerUnit <= 0.0f)
            {
                return;
            }

            UpdateCameraSize(pixelsPerUnit);
            UpdateCameraPosition(availableScreenRect, pixelsPerUnit);
        }
    }
}
