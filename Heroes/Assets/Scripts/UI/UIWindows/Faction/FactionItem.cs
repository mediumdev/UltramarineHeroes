using System.Linq;
using Configs;
using CoreConfigs.Configs;
using Enums;
using Game.Controllers;
using JetBrains.Annotations;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.UIWindows.Faction
{
    public class FactionItem : MonoBehaviour, IPointerDownHandler, IPointerExitHandler
    {
        [FormerlySerializedAs("_selected")] [SerializeField] private GameObject[] _selectedGameObjects;
        [SerializeField] private RTLTextMeshPro _title;
        [SerializeField] private TextMeshProUGUI _gems;
        [SerializeField] private TextMeshProUGUI _count;
        [SerializeField] private Image _icon;
        [SerializeField] private Image[] _lines;
        //[SerializeField] private float _timeToShowTooltip = 1.5f;
        [SerializeField] public Tooltip _tooltip;

        [Header("Visual Part")]
        [SerializeField] private Image _border;
        [SerializeField] private Image _titleBack;
        [SerializeField] private Image _bottom;
        [SerializeField] private Image _crystallFrame;
        [SerializeField] private Image _limitFrame;
        [Space]
        [SerializeField] private Sprite _usualBorder;
        [SerializeField] private Sprite _usualTitleBack;
        [SerializeField] private Sprite _usualBottom;
        [SerializeField] private Sprite _usualCrystallFrame;
        [SerializeField] private Sprite _usualLimitFrame;
        [SerializeField] private Sprite _mercenaryBorder;
        [SerializeField] private Sprite _mercenaryTitleBack;
        [SerializeField] private Sprite _mercenaryBottom;
        [SerializeField] private Sprite _mercenaryCrystallFrame;
        [SerializeField] private Sprite _mercenaryLimitFrame;

        private string _configId;
        private UnitConfig _config;
        private FactionWindow _parent;
        private LineType _line;
        private float _timer;
        private bool _timerEnabled;
        
        public void Init(FactionWindow parent, UnitConfig config, LineType lineType)
        {
            _parent = parent;
            _config = config;
            _configId = config.Uid;
            _line = lineType;
            
            if (!string.IsNullOrEmpty(_config.Name))
            {
                _title.gameObject.SetActive(true);
                _title.text = _config.Name;
            }

            if (_config.Icon != null)
            {
                _icon.gameObject.SetActive(true);
                _icon.sprite = _config.Icon;
            }

            var haveUnitsCount = UnitLimitManager.Instance.UnitInInventory(_config)
                ? UnitLimitManager.Instance.GetUnitCount(_config)
                : MercenariesController.Instance.UnitInStockCount(
                    ConfigBase.LoadFirstAvailableConfig<MercenarySetConfig>().Mercenaries.First(m => m.Config == _config).Config);
            _count.text = $"{haveUnitsCount}/{_config.MaxCount}";
            _count.color = haveUnitsCount < _config.MaxCount ? Color.yellow : Color.green;
            _gems.text = _config.Cost.ToString();

            foreach (var setupProperty in _config.SetupProperties)
            {
                _lines[(int) setupProperty.SetupType].gameObject.SetActive(true);
            }

            RepaintBorders();
        }

        public void RepaintBorders()
        {
            _border.sprite = _config.IsMercenary ? _mercenaryBorder : _usualBorder;
            _bottom.sprite = _config.IsMercenary ? _mercenaryBottom : _usualBottom;
            _titleBack.sprite = _config.IsMercenary ? _mercenaryTitleBack : _usualTitleBack;
            _crystallFrame.sprite = _config.IsMercenary ? _mercenaryCrystallFrame : _usualCrystallFrame;
            _limitFrame.sprite = _config.IsMercenary ? _mercenaryLimitFrame : _usualLimitFrame;
        }

        private void Update()
        {
            if (!_timerEnabled) return;
            _timer += Time.deltaTime;
        }

        [UsedImplicitly]
        public void TryToSelect()
        {
           /* if (_timer >= _timeToShowTooltip)
            {
                ShowTooltip(_config);
                _timer = 0;
                _timerEnabled = false;
            }
            else*/
           
            _parent.TryToSelect(this, _configId, _line);
            _parent.ShowUnitInfo(_config);
        }

        public void Select(bool isActive)
        {
            foreach (var obj in _selectedGameObjects)
                obj.SetActive(isActive);
        }
        
        public void OnPointerDown (PointerEventData eventData)
        {
            _timerEnabled = true;
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            _timer = 0;
            _timerEnabled = false;
        }

        public void ShowTooltip(UnitConfig config)
        {
            _config = config;
            
            if (_tooltip == null) return;
            
            _tooltip.gameObject.SetActive(true);
            _tooltip.init(config);
        }
    }
}
