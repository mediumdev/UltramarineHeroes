using Structs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebriefingItem : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _rewardCount;

    public void Init(CurrencyWithCount item)
    {
        if (item.currency == null || item.count == 0) return;
        
        _icon.sprite = item.currency.Icon;
        _rewardCount.text = item.count.ToString();

    }
}
