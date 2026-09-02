using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows
{
    public class Window : MonoBehaviour
    {
        [SerializeField] private bool _closePanel = true;
        [SerializeField] private bool _uniqueWindow = false;
        [SerializeField] private bool _useDarkBackground;
        [SerializeField] private Color backgroundColor = new Color(0, 0, 0, 0.5f);
        private GameObject _background;
        protected bool _opened;
        
        public Window UniqueWindowPrefab { get; set; }
        public bool UniqueWindow => _uniqueWindow;

        public event Action OnOpenEvent;
        public event Action OnCloseEvent;
        
        public virtual void OnOpen()
        {
            OnOpenEvent?.Invoke();
            _opened = true;
        }

        public void CreateBackground()
        {
            if (!_useDarkBackground) return;
            
            _background = new GameObject();
            _background.transform.parent = transform.parent;
            _background.name = gameObject.name + "_dark_background";
            _background.transform.SetSiblingIndex(transform.GetSiblingIndex());
            
            Image img = _background.AddComponent<Image>();
            img.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, 0);
            
            if (_closePanel) _background.AddComponent<Button>().onClick.AddListener(ClosePanel);
            
            img.rectTransform.localPosition = new Vector3(0, 0, 400);
            img.rectTransform.sizeDelta = new Vector2(3000, 3000);
            
            Sequence seq = DOTween.Sequence();
            seq.Append(img.DOFade(backgroundColor.a, 0.2f));
            seq.AppendCallback(() =>
            {
                _background.transform.parent = transform;
                _background.transform.SetAsFirstSibling();
                img.rectTransform.localPosition = new Vector3(0, 0, 400);
                img.rectTransform.sizeDelta = new Vector2(3000, 3000);
                _background.transform.localScale = Vector3.one;
            });
        }
        
        public virtual void OnClose()
        {
            OnCloseEvent?.Invoke();
        }
        
        public void ClosePanel()
        {
            if (!_closePanel) return;
            if (_useDarkBackground && _background != null)
            {
                Destroy(_background);
            }
            Close();
        }

        public void Close()
        {
            if (_useDarkBackground && _background != null)
            {
                Destroy(_background);
            }
            WindowManager.Instance.Close(this);
        }

        public IEnumerator CloseAfterTime(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (this != null)
                Close();
        }
    }
}