using ArrowScroll;
using UnityEngine;

public class LineContainer : MonoBehaviour, IScrollableContainer
{
    [SerializeField] private int _visibleCount;

    public int VisibleCount => _visibleCount;
    public int GetCount { get; set; }
}
