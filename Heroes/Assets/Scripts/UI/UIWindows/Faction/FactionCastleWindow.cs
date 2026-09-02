using System.Collections.Generic;
using System.Linq;
using Configs;
using CoreConfigs.Configs;
using CoreUtils.Utils;
using Enums;
using Game.Controllers;
using TMPro;
using UI.UIWindows.Lobby;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIWindows.Faction
{
    public class FactionCastleWindow : Window
    {
        [SerializeField] private CastleItem _item;
        [SerializeField] private Transform _container;
        [SerializeField] private TextMeshProUGUI _factionName;
        [SerializeField] private TextMeshProUGUI _level;
        [SerializeField] private UpgradeWindow _window;
        [SerializeField] private LobbyCurrency _sphereCounter;
        [SerializeField] private Tooltip _tooltip;
        [SerializeField] private Image BackgroundImage;

        private List<UnitConfig> _units = new List<UnitConfig>();
        private FactionConfig _config;
        private CurrencyConfig _spheres;
        private bool _repaintSubscribed;

        public void Init(FactionConfig config)
        {
            _config = config;
            var price = PlayerFactionsController.Instance.GetUpgradePrice(_config);
            _spheres = null;
            foreach (var item in price)
            {
                if (item.currency.currencyType != CurrencyType.Sphere) continue;
                _spheres = item.currency;
            }

            _sphereCounter.gameObject.SetActive(_spheres != null);
            if (_spheres != null)
                _sphereCounter.Init(_spheres);

            Repaint();

            if (_repaintSubscribed) return;
            
            PlayerFactionsController.Instance.FactionUpgradedEvent += Repaint;
            _repaintSubscribed = true;
        }

        private void OnDisable()
        {
            if (_repaintSubscribed)
                PlayerFactionsController.Instance.FactionUpgradedEvent -= Repaint;
        }

        public void ShowTooltip(UnitConfig config, float position)
        {
            _tooltip.init(config);
            var tooltipTransform = _tooltip.transform;
            var currentPosition = tooltipTransform.position;
            currentPosition.x = position;
            tooltipTransform.position = currentPosition;
            _tooltip.gameObject.SetActive(true);
        }

        public void HideTooltip()
        {
            _tooltip.gameObject.SetActive(false);
        }

        private void Repaint()
        {
            var factionProgress = PlayerFactionsController.Instance.GetFactionProgress(_config);
            
            _factionName.text = _config.FactionName;
            _level.text = $"Level {factionProgress + 1}";
            BackgroundImage.sprite = _config.FactionBackground;
            _units = PlayerFactionsController.Instance.GetAllFactionUnits(_config);
            _units.Sort(Comparison);
            _container.Clear();
            foreach (var unit in _units)
            {
                var item = Instantiate(_item, _container);
                item.Init(this, unit, factionProgress);
            }
        }
    
        private int Comparison(UnitConfig x, UnitConfig y)
        {
            return x.Cost >= y.Cost ? 1 : -1;
        }

        public void OpenUpgradeWindow()
        {
            var window = WindowManager.Instance.Open(_window, true) as UpgradeWindow;
            if (window != null) 
                window.Init(_config);
        }

        public void ChangeFaction(int direction)
        {
            var configs = ConfigBase.LoadAll<FactionConfig>()
                .Where(x => x.FactionType == _config.FactionType)
                .OrderBy(x => x.Uid).ToList();
            
            var idx = configs.IndexOf(_config);
            var idxNext = idx + direction;
            if (idxNext >= configs.Count)
                idxNext = 0;
            else if (idxNext < 0)
                idxNext = configs.Count - 1;
            
            Init(configs[idxNext]);
        }
    
        public void CloseWindow()
        {
            Close();
        }
    }
}
