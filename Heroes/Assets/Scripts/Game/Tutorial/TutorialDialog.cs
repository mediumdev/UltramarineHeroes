using System;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Tutorial
{
    public class TutorialDialog : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Vector3 _iconScale;
        [SerializeField] private GameObject _bubble;
        [SerializeField] private Vector3 _bubbleScale;
        [SerializeField] private TextMeshProUGUI _text;

        private bool _opened;
        private Sprite _currentIcon;

        public event Action OnCloseEvent;
        
        public void Open(Sprite icon, string text)
        {
            _bubble.transform.DOKill(true);
            _icon.transform.DOKill(true);
            
            if (_opened)
                _bubble.transform.DOScale(0f, 0.2f).OnComplete(() => { SetText(text); });
            else
            {
                gameObject.SetActive(true);
                SetText(text);
            }

            if (_currentIcon != null && icon != _currentIcon)
                _icon.transform.DOScale(0f, 0.2f).OnComplete(() => { SetIcon(icon); });
            else
                SetIcon(icon);
        }

        private void SetIcon(Sprite icon)
        {
            _icon.transform.DOScale(_iconScale, 0.2f);
            _icon.sprite = _currentIcon = icon;
        }

        private void SetText(string text)
        {
            _opened = true;
            _bubble.transform.DOScale(_bubbleScale, 0.2f);
            
            // TODO Add localization
            _text.text = text;
        }

        public void Close()
        {
            CloseVisual();
            OnCloseEvent?.Invoke();
        }

        public void CloseVisual()
        {
            _bubble.transform.DOScale(0f, 0.2f);
            _icon.transform.DOScale(0f, 0.2f);
            gameObject.SetActive(false);
            _opened = false;
        }
    }
}
