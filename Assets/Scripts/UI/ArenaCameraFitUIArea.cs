using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public sealed class ArenaCameraFitUIArea : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<RectTransform> OnRectChange = new();

    private RectTransform RectTransform => (RectTransform)transform;

    private void NotifyRectChange()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }

        OnRectChange.Invoke(RectTransform);
    }

    private void OnRectTransformDimensionsChange()
    {
        NotifyRectChange();
    }

    private void OnEnable()
    {
        NotifyRectChange();
    }
}
