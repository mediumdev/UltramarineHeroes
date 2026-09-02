using CoreUtils.Utils;
using UnityEngine;

namespace Packages.CoreUtils.Utils
{
    public class ScaleBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _scaleBar;
        [SerializeField] private bool _inverse;

        protected float _width;
        public float Percent { get; set; }

        private void Awake()
        {
            _width = GetComponent<RectTransform>().rect.width;
        }

        public virtual void Scale(float percent)
        {
            if (_width == 0)
                Awake();
            Percent = Mathf.Min(1f, percent);
            _scaleBar.SetRight(_inverse ? _width - Percent * _width : Percent * _width);
        }
    }
}