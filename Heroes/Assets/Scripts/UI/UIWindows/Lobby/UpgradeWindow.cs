using System.Collections.Generic;
using Configs;
using CoreUtils.Utils;
using Game.Controllers;
using TMPro;
using UI.Windows;
using UnityEngine;

namespace UI.UIWindows.Lobby
{
    public class UpgradeWindow : Window
    {
        [SerializeField] private UpgradeItem _item;
        [SerializeField] private RectTransform _container;
        [SerializeField] private RectTransform _lvlUpContainer;
        [SerializeField] private TextMeshProUGUI _currentLvlTitle;
        [SerializeField] private TextMeshProUGUI _nextLvlTitle;
        [SerializeField] private UpgradeCostItem[] _costItems;
        [SerializeField] private TextMeshProUGUI _towerHealthCurrent;
        [SerializeField] private TextMeshProUGUI _towerHealthNext;
        [SerializeField] private GameObject _consumablesObject;
        [SerializeField] private GameObject _consumablesBorders;
        [SerializeField] private TextMeshProUGUI _upgradeText;

        private FactionConfig _factionConfig;
    
        private List<UnitConfig> _units = new List<UnitConfig>();
        public void Init(FactionConfig factionConfig)
        {
            _factionConfig = factionConfig;
            Repaint();
        }

        private void Repaint()
        {
            CreateList();
            CreateSectionPanel();
        }
    
        private void CreateList()
        {
            _units = new List<UnitConfig>();
            foreach (var unit in PlayerFactionsController.Instance.GetFactionUnits(_factionConfig))
                _units.Add(unit);
        
            _units.Sort(Comparison);
        }

        private void CreateSectionPanel()
        {
            var oldUnitsLimits = new Dictionary<UnitConfig, UnitWithLimits>();
            
            _container.Clear();
            var currentUnits = PlayerFactionsController.Instance.GetCurrentFactionUnitsData(_factionConfig);
            currentUnits.Sort(Comparison);
            foreach (var unit in currentUnits)
            {
                var item = Instantiate(_item,_container);
                item.Init(unit);
                oldUnitsLimits[unit.config] = unit;
                // TODO использовать необходимые поля из limit, generationAmount, generationSeconds для отображения
                // Инфо о текущем количестве юнитов есть в UnitLimitManager.Instance:
                // GetUnitLimit - размер лимита юнита
                // GetUnitCount - количество юнита в "инвентаре"
            }
        
            _lvlUpContainer.Clear();

            var towerHealthCurrent = $"{PlayerFactionsController.Instance.GetFactionTowerHealth(_factionConfig)} HP";
            _towerHealthCurrent.text = towerHealthCurrent;
            
            var isMaxLvl = PlayerFactionsController.Instance.IsFactionMaxUpgraded(_factionConfig);
            _consumablesObject.SetActive(!isMaxLvl);
            _consumablesBorders.SetActive(!isMaxLvl);
            
            if (isMaxLvl)
            {
                _currentLvlTitle.text = "max level";
                _nextLvlTitle.text = null;
                _towerHealthNext.text = towerHealthCurrent;
                _towerHealthNext.color = Color.white;
                _upgradeText.text = "MAX LEVEL";
            }
            else
            {
                var towerHealthNext = $"{PlayerFactionsController.Instance.GetFactionTowerHealth(_factionConfig, true)} HP";
                _towerHealthNext.text = towerHealthNext;
                _towerHealthNext.color = Color.green;
                
                var factionProgress = PlayerFactionsController.Instance.GetFactionProgress(_factionConfig);
                _currentLvlTitle.text = $"level {factionProgress + 1}";
                _nextLvlTitle.text = $"level {factionProgress + 2}";
                
                _upgradeText.text = "UPGRADE";
                
                var nextUnits = PlayerFactionsController.Instance.GetNextFactionUnitsData(_factionConfig);
                nextUnits.Sort(Comparison);
                
                foreach (var unit in nextUnits)
                {
                    var oldData = oldUnitsLimits.ContainsKey(unit.config)
                        ? oldUnitsLimits[unit.config]
                        : new UnitWithLimits
                        {
                            config = unit.config,
                            generationAmount = 0,
                            generationSeconds = 100000,
                            limit = 0
                        };
                    
                    var item = Instantiate(_item,_lvlUpContainer);
                    item.Init(unit);
                    item.CompareWithOldUnit(oldData);
                }

                var upgradePrice = PlayerFactionsController.Instance.GetUpgradePrice(_factionConfig);
                for (var i = 0; i < upgradePrice.Length; i++)
                {
                    if (i >= _costItems.Length) break;
                
                    _costItems[i].Repaint(upgradePrice[i]);
                }
            }
        }
    
        private int Comparison(UnitConfig x, UnitConfig y)
        {
            return x.Cost >= y.Cost ? 1 : -1;
        }
        
        private int Comparison(UnitWithLimits x, UnitWithLimits y)
        {
            return x.config.Cost >= y.config.Cost ? 1 : -1;
        }

        public void DoUpgrade()
        {
            var upgradeResult = PlayerFactionsController.Instance.UpgradeFaction(_factionConfig);
            Debug.Log($"Улучшение фракции {_factionConfig.name}, результат - {upgradeResult}");
            // TODO метод апгрейда отдает FactionUpgradeResult, нужно добавить логику их отображения вместо дебаг-вывода
            // MaxProgress - фракция уже прокачена до максимума
            // NotEnoughCurrency - недостаточно валюты для прокачки
            // Ok - успешно улучшено
            
            Repaint();
        }
    }
}
