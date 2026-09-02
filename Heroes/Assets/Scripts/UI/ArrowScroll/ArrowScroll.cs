using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace ArrowScroll {
    public class ArrowScroll : MonoBehaviour {
        [SerializeField] private GameObject _scrollView;
        [SerializeField] private float _step;
        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;

        private int _currentIndex;
        private int _maxIndex;
        private IScrollableContainer _container;
        private float _defaultX;

        private void Start()
        {
            _container = _scrollView.GetComponent<IScrollableContainer>();

            if (_container == null) {
                Destroy(this);
                return;
            }

            _maxIndex = _container.GetCount - _container.VisibleCount;
            _defaultX = _scrollView.transform.localPosition.x;
            _leftButton.interactable = false;
            
            Scroll(0);
        }

        [UsedImplicitly]
        public void Scroll(int index) {
            _currentIndex = Mathf.Max(0, Mathf.Min(_maxIndex, _currentIndex + index));
            _scrollView.transform.localPosition = new Vector3(_defaultX - _currentIndex * _step, _scrollView.transform.localPosition.y);

            _leftButton.interactable = _currentIndex > 0;
            _rightButton.interactable = _currentIndex < _maxIndex;
        }
    }
}
