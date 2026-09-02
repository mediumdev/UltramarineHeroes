using Structs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCostItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _cost;
    [SerializeField] private Image _icon;

    public void Repaint(CurrencyWithCount currency)
    {
        _cost.text = currency.count.ToString();
        _icon.sprite = currency.currency.Icon;
    }
}
