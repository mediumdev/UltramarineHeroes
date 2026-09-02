using Configs;
using JetBrains.Annotations;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class MercenaryItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _manaCost;
    [SerializeField] private TextMeshProUGUI _gems;
    [SerializeField] private TextMeshProUGUI _count;
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _purchasedMark;
    [SerializeField] private GameObject _button;
   
    private MercenaryWindow _parent;
    private Mercenary _mercenary;
    private int _slotId;

    public void Init(MercenaryWindow parent, Mercenary unit, int slotNumber, bool purchased = false)
    {
        _parent = parent;
        _mercenary = unit;
        
        if (!string.IsNullOrEmpty(unit.Config.Name))
        {
            _title.gameObject.SetActive(true);
            _title.text = unit.Config.Name;
        }

        if (unit.Config.Icon != null)
        {
            _icon.gameObject.SetActive(true);
            _icon.sprite = unit.Config.Icon;
        }

        _manaCost.text = unit.Config.Cost.ToString();
        _count.text = unit.SaleAmount.ToString();
        _gems.text = unit.CurrencyCost.count.ToString();

        _slotId = slotNumber;
        _button.SetActive(!purchased);
        _purchasedMark.SetActive(purchased);
    }

    public void PurchaseMercenary()
    {
        if (_parent.Purchase(_mercenary, _slotId))
        {
            _button.SetActive(false);
            _purchasedMark.SetActive(true);
        }
    }
    
    [UsedImplicitly]
    public void SelectMercenary()
    {
        _parent.ShowUnitInfo(_mercenary.Config);
    }
}

