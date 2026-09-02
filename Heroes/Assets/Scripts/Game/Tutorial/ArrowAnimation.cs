using System;
using DG.Tweening;
using UnityEngine;

namespace Game.Tutorial.Lobby
{
    public class ArrowAnimation : MonoBehaviour
    {
        [SerializeField] private Transform _arrow;
        
        private void Awake()
        {
            var seq = DOTween.Sequence();
            seq.Append(_arrow.DOLocalMoveY(50f, 0.5f));
            seq.AppendInterval(0.2f);
            seq.Append(_arrow.DOLocalMoveY(0, 0.5f));
            seq.AppendInterval(0.2f);
            seq.SetLoops(-1);
        }
    }
}
