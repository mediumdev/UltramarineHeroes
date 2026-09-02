using UnityEngine;

namespace Packages.CoreUtils.Utils
{
    [RequireComponent(typeof(RectTransform))]
    public class CanvasSafeArea : MonoBehaviour
    {
        [SerializeField, HideInInspector] private RectTransform _rectTransform;

        private void Start()
        {
            var safeArea = Screen.safeArea;
            var screenSize = new Vector2Int(Screen.width, Screen.height);

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;

            if (anchorMin.x <= screenSize.x - anchorMax.x)
            {
                anchorMin.x = screenSize.x - anchorMax.x;
            }
            else
            {
                anchorMax.x = screenSize.x - anchorMin.x;
            }
            anchorMin.x /= screenSize.x;
            anchorMin.y /= screenSize.y;
            anchorMax.x /= screenSize.x;
            anchorMax.y /= screenSize.y;

            _rectTransform.anchorMax = anchorMax;
            _rectTransform.anchorMin = anchorMin;
        }

        private void OnValidate()
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
        }
    }
}