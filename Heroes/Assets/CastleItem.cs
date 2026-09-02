using Configs;
using Game.Controllers;
using TMPro;
using UI.UIWindows.Faction;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CastleItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _cost;
    [SerializeField] private GameObject _limitBackground;
    [SerializeField] private TextMeshProUGUI _limit;
    [SerializeField] private GameObject _progressBar;
    [SerializeField] private Image _progressFill;
    [SerializeField] private GameObject _unavailable;
    [SerializeField] private TextMeshProUGUI _unlockLevel;
    [SerializeField] private float _offset;

    private FactionCastleWindow _parent;
    private UnitConfig _config;
    private bool _unlocked;
    
    public void Init(FactionCastleWindow parent, UnitConfig config, int factionProgress)
    {
        _parent = parent;
        _config = config;
        
        var unlockLevel = PlayerFactionsController.Instance.GetUnitUnlockLevel(_config);

        _icon.sprite = config.Icon;
        _name.text = config.Name;
        _cost.text = config.Cost.ToString();
        _unlocked = unlockLevel <= factionProgress;

        if (_unlocked)
        {
            _limit.text = $"{UnitLimitManager.Instance.GetUnitCount(config)}/{UnitLimitManager.Instance.GetUnitLimit(config)}";
        }
        else
        {
            _limitBackground.SetActive(false);
            _unavailable.SetActive(true);
            _unlockLevel.text = $"level {unlockLevel + 1}";
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(!_unlocked) return;

        var isPointerLeft = eventData.position.x < Screen.width / 2f ? -1 : 1;
        var position = transform.position.x + _offset * isPointerLeft * Screen.width/10f;
        _parent.ShowTooltip(_config, position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(!_unlocked) return;
        
        _parent.HideTooltip();
    }
}