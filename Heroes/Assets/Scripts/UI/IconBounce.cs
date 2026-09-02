using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class IconBounce : MonoBehaviour
    {
        [SerializeField] private GameObject _gameObject;

        public void Bounce()
        {
            var seq = DOTween.Sequence();
            seq.Append(_gameObject.transform.DOScale(1.05f,0.2f));
            seq.Append(_gameObject.transform.DOScale(1f,0.2f));
        }
    }
}
