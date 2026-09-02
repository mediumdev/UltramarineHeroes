using JetBrains.Annotations;
using UnityEngine;

namespace UI.Windows
{
    public class OpenWindowButton : MonoBehaviour
    {
        [SerializeField] private Window _window;
        [SerializeField] private GameObject _unitSpawnObject;

        public Camera _cam;

        [UsedImplicitly]
        public void Open()
        {
            var win = WindowManager.Instance.Open(_window);
            win.gameObject.SetActive(true);
        }

        [UsedImplicitly]
        public void OpenWindow()
        {
            if (WindowManager.Instance.LastOpened == null)
            {
                var win = WindowManager.Instance.Open(_window);
                var screenPos = _cam.WorldToScreenPoint(_unitSpawnObject.transform.position);
                var x = screenPos.x;
                var y = screenPos.y;
                if (win != null) win.transform.position = new Vector2(x, y);
            }
        }
    }
}
