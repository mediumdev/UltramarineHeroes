using System;
using CoreUtils.Utils;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Tutorial.Lobby
{
    public class LobbyTutorialHelper : MonoBehaviour
    {
        [SerializeField] private Image _fade;
        [SerializeField] private Transform _lock;
        [SerializeField] private TutorialDialog _dialogLeft;
        [SerializeField] private TutorialDialog _dialogRight;
        private TutorialDialog _currentDialog;
        [SerializeField] private Sprite _dialogIcon;
        [SerializeField] private GameObject _arrow;
        [SerializeField] private Button _defaultButton;
        [SerializeField] private GameObject _mask;
        [SerializeField] private GameObject _highlightMask;

        private Action _callback;
        private GameObject _currentArrow;

        public bool Faded => _fade.gameObject.activeSelf;

        private void OnEnable()
        {
            LobbyTutorialManager.Instance.Helper = this;
            _dialogLeft.OnCloseEvent += DialogOnOnCloseEvent;
            _dialogRight.OnCloseEvent += DialogOnOnCloseEvent;
        }

        private void OnDisable()
        {
            LobbyTutorialManager.Instance.Helper = null;
            _dialogLeft.OnCloseEvent -= DialogOnOnCloseEvent;
            _dialogRight.OnCloseEvent -= DialogOnOnCloseEvent;
        }

        public void FadeIn()
        {
            _fade.DOKill();
            _fade.gameObject.SetActive(true);
            _mask.SetActive(true);
            _fade.DOFade(0, 0f);
        }

        public void FadeOut()
        {
            _fade.DOFade(0, 0f).OnComplete(() => { _fade.gameObject.SetActive(false); });
            _mask.SetActive(false);
        }

        public void OpenDialog(string text , AnchorPresets preset, Action callback, Sprite icon = null)
        {
            transform.SetAsLastSibling();
            _callback = callback;

            if (preset == AnchorPresets.BottomLeft)
            {
                if (_currentDialog != null && _currentDialog != _dialogLeft)
                    _currentDialog.CloseVisual();
                
                _dialogLeft.Open(icon ? icon : _dialogIcon, text);
                _currentDialog = _dialogLeft;
            }
            else if (preset == AnchorPresets.BottomRight)
            {
                if (_currentDialog != null && _currentDialog != _dialogRight)
                    _currentDialog.CloseVisual();
                
                _dialogRight.Open(icon ? icon : _dialogIcon, text);
                _currentDialog = _dialogRight;
            }
            else
            {
                Debug.LogError($"Type of anchor preset ({preset}) is not supported for the dialog window." +
                               $"Choose one of these: AnchorPresets.BottomLeft or AnchorPresets.BottomRight");
            }
        }

        public void CloseDialog(bool callback)
        {
            if (callback)
                _currentDialog.Close();
            else if(!callback)
                _currentDialog.CloseVisual();
        }

        public void ShowButton(Button element, Action callback, float localY, bool freeze = false)
        {
            _lock.gameObject.SetActive(true);
            var obj = Instantiate(element, _lock);
            obj.gameObject.SetActive(true);
            if (freeze)
                obj.transform.position = element.transform.position;
            
            ShowArrow(obj.GetComponent<RectTransform>(), localY, Vector3.zero);
            obj.onClick.AddListener(() =>
            {
                Destroy(obj.gameObject);
                _lock.gameObject.SetActive(false);
                callback?.Invoke();
            });
        }

        public void ShowDefaultButton(AnchorPresets preset, Vector2 buttonSize, Vector2 anchoredPos, float arrowLocalY, Action callback)
        {
            _lock.gameObject.SetActive(true);
            var obj = Instantiate(_defaultButton, _lock);

            var rect = obj.GetComponent<RectTransform>();
            rect.SetAnchor(preset);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = buttonSize;
            
            obj.gameObject.SetActive(true);

            ShowArrow(obj.GetComponent<RectTransform>(), arrowLocalY, Vector3.zero);
            obj.onClick.AddListener(() =>
            {
                Destroy(obj.gameObject);
                _lock.gameObject.SetActive(false);
                callback?.Invoke();
            });
        }

        public void ShowButtonWithoutArrow(AnchorPresets preset, Vector2 buttonSize, Vector2 anchoredPos, bool locked, Action callback)
        {
            _lock.gameObject.SetActive(locked);
            var obj = Instantiate(_defaultButton, locked ? _lock : transform);

            var rect = obj.GetComponent<RectTransform>();
            rect.SetAnchor(preset);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = buttonSize;
            
            obj.gameObject.SetActive(true);
            
            obj.onClick.AddListener(() =>
            {
                Destroy(obj.gameObject);
                _lock.gameObject.SetActive(false);
                callback?.Invoke();
            });
        }
        
        public void ShowArrow(RectTransform target, float localY, Vector3 rotation)
        {
            transform.SetAsLastSibling();
            if (_currentArrow != null)
                Destroy(_currentArrow);
            _currentArrow = Instantiate(_arrow, target);
            _currentArrow.GetComponent<RectTransform>().SetTop(localY);
            _currentArrow.transform.rotation = Quaternion.Euler(rotation);
        }

        public void ShowArrow(AnchorPresets preset, Vector2 anchoredPos, Vector3 rotation)
        {
            transform.SetAsLastSibling();     
            if (_currentArrow != null)         
                Destroy(_currentArrow);          
            _currentArrow = Instantiate(_arrow, transform);          
            var rect = _currentArrow.GetComponent<RectTransform>();    
            rect.SetAnchor(preset);     
            rect.anchoredPosition = anchoredPos;          
            _currentArrow.transform.rotation = Quaternion.Euler(rotation);
        }

        public void HideArrow()
        {
            if (_currentArrow != null)
                Destroy(_currentArrow);
        }
        
        public void ShowHighlightMask(AnchorPresets preset, Vector2 size, int posX, int posY)
        {
            var rect = _highlightMask.GetComponent<RectTransform>();
            
            if (!_highlightMask.activeSelf)
            {
                rect.SetAnchor(preset, posX, posY);
                rect.sizeDelta = size;
                _highlightMask.SetActive(true);
            }
            else
            {
                rect.SetAnchor(preset);
                rect.DOAnchorPosX(posX, .5f);
                rect.DOAnchorPosY(posY, .5f);
                rect.DOSizeDelta(size, .5f);
            }
        }

        public void HideHighlightMask()
        {
            _highlightMask.SetActive(false);
        }

        private void DialogOnOnCloseEvent()
        {
            _callback?.Invoke();
        }

        public void EnableLock()
        {
            _lock.gameObject.SetActive(true);
        }

        public void DisableLock()
        {
            _lock.gameObject.SetActive(false);
        }
    }
}
