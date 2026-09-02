using System.Collections.Generic;
using System.Linq;
using com.adjust.sdk;
using Configs;
using CoreConfigs.Configs;
using CoreUtils.Utils;
using Enums;
using Game;
using Game.Controllers;
using TMPro;
using UI.Windows;
using UnityEngine;
using Utils;
using Utils.SaveManager;

namespace UI.UIWindows.Faction
{
    public class FactionWindow : Window
    {
        [SerializeField] private FactionItem _item;
        [SerializeField] private Tooltip _unitItemInfo; 
        [SerializeField] private FactionTypeItem _typeItem;
        [SerializeField] private MercenarySetConfig _mercenarySet;
        [SerializeField] private FactionConfig[] _factions;
        [SerializeField] private Transform _typeContainer;
        [SerializeField] private Transform[] _containers;
        [SerializeField] private TextMeshProUGUI _changeText;
        [SerializeField] private ConfigLibrary _configLibrary;
        [SerializeField] private TextMeshProUGUI _playerHpText;
        [SerializeField] private GameObject _checkScreen;
        [SerializeField] private GameStartController _startController;
        
        private bool _bot;
        private List<string> _collectionUids = new List<string>();
        private readonly Dictionary<LineType, string> _factionUids = new Dictionary<LineType, string>();
        private readonly Dictionary<LineType, List<string>> _collectionUnitsByLine = new Dictionary<LineType, List<string>>();

        private static List<FactionConfig> AirFactions => FactionsController.Instance.AirFactions;
        private static List<FactionConfig> GroundFactions => FactionsController.Instance.GroundFactions;
        private static List<FactionConfig> UndergroundFactions => FactionsController.Instance.UndergroundFactions;
        
        public FactionConfig[] Factions => _factions;

        private void OnEnable()
        {
            if (!ConfigLibrary.Loaded)
                ConfigLibrary.LoadLibrary(_configLibrary);
            
            CreateLists();
            CreateSectionPanels();
            RepaintTowersHealth();
        }

        private void CreateLists()
        {
            _typeContainer.Clear();
            var factionUidList = FactionsController.Instance.FactionUidsByPlayer[_bot];
            
            _factionUids.Clear();
            for (var i = 0; i < factionUidList.Length; i++)
                _factionUids.Add((LineType)i, factionUidList[i]);
            
            var collectionString = SaveManager.GetValue(_bot 
                ? SavedDataManager.BotCollectionKey 
                : SavedDataManager.PlayerCollectionKey, 
                string.Empty);
            _collectionUids = collectionString.Split(';').ToList();

            var item = Instantiate(_typeItem, _typeContainer);
            item.Init(AirFactions, this, factionUidList, LineType.Air);
            item = Instantiate(_typeItem, _typeContainer);
            item.Init(GroundFactions, this, factionUidList, LineType.Ground);
            item = Instantiate(_typeItem, _typeContainer);
            item.Init(UndergroundFactions, this, factionUidList, LineType.Underground);
        }

        private void CreateSectionPanels()
        {
            ParseCollection();
            
            if (!_bot)
            {
                CreateSectionPanel(_containers[0], GetUnitsWithMercenaries(LineType.Air), LineType.Air);
                CreateSectionPanel(_containers[1], GetUnitsWithMercenaries(LineType.Ground), LineType.Ground);
                CreateSectionPanel(_containers[2], GetUnitsWithMercenaries(LineType.Underground), LineType.Underground);
            }
            else
            {
                CreateSectionPanel(_containers[0], ConvertFromType(LineType.Air).FactionUnits, LineType.Air);
                CreateSectionPanel(_containers[1], ConvertFromType(LineType.Ground).FactionUnits, LineType.Ground);
                CreateSectionPanel(_containers[2], ConvertFromType(LineType.Underground).FactionUnits, LineType.Underground);
            }
        }

        private void RepaintTowersHealth()
        {
            var factionsHealthSum = FactionsController.Instance.FactionUidsByPlayer[_bot]
                .Select(ConfigBase.LoadConfig<FactionConfig>)
                .Where(x => x != null)
                .Sum(x => PlayerFactionsController.Instance.GetFactionTowerHealth(x));
            _playerHpText.text = $"{factionsHealthSum} HP";
        }

        private void ParseCollection()
        {
            _collectionUnitsByLine.Clear();
            
            foreach (var collectionUid in _collectionUids)
            {
                foreach (var faction in _factions)
                {
                    var factionUnits = PlayerFactionsController.Instance.GetFactionUnits(faction, !_bot);
                    var target = factionUnits.FirstOrDefault(x => string.Equals(x.Uid, collectionUid));
                    
                    if (target == null) continue;

                    if (!_collectionUnitsByLine.ContainsKey(faction.FactionType))
                        _collectionUnitsByLine.Add(faction.FactionType, new List<string>());
                    _collectionUnitsByLine[faction.FactionType].Add(collectionUid);
                }
            }
        }
        
        public UnitConfig[] GetUnitsWithMercenaries(LineType line)
        {
            var allUnits = PlayerFactionsController.Instance.GetFactionUnits(ConvertFromType(line));
            var allMercenaries = _mercenarySet.Mercenaries.Where(m => m.Config.SetupProperties[0].SetupType == line);

            var units = allUnits.ToList();

            foreach (var merc in allMercenaries)
                if (MercenariesController.Instance.UnitInStockCount(merc.Config) > 0) units.Add(merc.Config);

            return units.ToArray();
        }

        private FactionConfig ConvertFromType(LineType type)
        {
            return (FactionConfig) ConfigLibrary.Instance.LoadConfig(_factionUids[type]);
        }

        private void CreateSectionPanel(Transform container, UnitConfig[] unitConfigs, LineType lineType)
        {
            container.Clear();

            if (!_collectionUnitsByLine.ContainsKey(lineType))
                _collectionUnitsByLine.Add(lineType, new List<string>());
            var unitUids = _collectionUnitsByLine[lineType];

            var sorted = unitConfigs.ToList();
            sorted.Sort(Comparison);
            
            foreach (var unit in sorted)
            {
                var item = Instantiate(_item, container);
                item.Init(this, unit, lineType);
                //item.ShowTooltip(unit);
                //item._tooltip.gameObject.SetActive(false);
                
                if (unitUids.Contains(unit.Uid))
                    item.Select(true);
            }

            var scrollableContainer = container.GetComponent<LineContainer>();
            if (scrollableContainer != null)
            {
                scrollableContainer.GetCount = sorted.Count;
            }
        }

        private int Comparison(UnitConfig x, UnitConfig y)
        {
            return x.Cost > y.Cost ? 1 : -1;
        }

        public void TryToSelect(FactionItem factionItem, string configId, LineType type)
        {
            TryToSelectUnit(factionItem, configId, type);
        }

        private void TryToSelectUnit(FactionItem factionItem, string configId, LineType type)
        {
            var list = _collectionUnitsByLine[type];

            if (list.Contains(configId))
            {
                factionItem.Select(false);
                list.Remove(configId);
            }
            else if (list.Count < 3)
            {
                factionItem.Select(true);
                list.Add(configId);
            }

            _collectionUnitsByLine[type] = list;
            FactionsController.Instance.SaveFactionCollectionData(_bot, _factionUids, _collectionUnitsByLine);
        }
        
        public void ChangeFaction(LineType factionLine, FactionConfig faction)
        {
            if (!_factionUids.ContainsKey(factionLine)) 
                _factionUids[factionLine] = string.Empty;
            _factionUids[factionLine] = faction.Uid;
            
            if (!_collectionUnitsByLine.ContainsKey(factionLine))
                _collectionUnitsByLine.Add(factionLine, new List<string>());
            _collectionUnitsByLine[factionLine].Clear();
            foreach (var unitConfig in PlayerFactionsController.Instance.GetFactionUnits(faction, !_bot))
                _collectionUnitsByLine[factionLine].Add(unitConfig.Uid);
            _collectionUnitsByLine[factionLine] = _collectionUnitsByLine[factionLine].Take(3).ToList();

            CreateSectionPanel(
                _containers[(int)factionLine], 
                _bot ? ConvertFromType(factionLine).FactionUnits : GetUnitsWithMercenaries(factionLine),
                factionLine
                );
            FactionsController.Instance.SaveFactionCollectionData(_bot, _factionUids, _collectionUnitsByLine);

            RepaintTowersHealth();
        }

        private bool CheckUnitsInLine(UnitConfig[] unitConfigs, LineType lineType)
        {
            var unitUids = _collectionUnitsByLine[lineType];

            foreach (var unit in unitConfigs)
            {
                if (unitUids.Contains(unit.Uid))
                {
                    var haveUnitsCount = UnitLimitManager.Instance.UnitInInventory(unit)
                        ? UnitLimitManager.Instance.GetUnitCount(unit)
                        : MercenariesController.Instance.UnitInStockCount(
                            ConfigBase.LoadFirstAvailableConfig<MercenarySetConfig>().Mercenaries.First(m => m.Config == unit).Config);

                    if (haveUnitsCount < unit.MaxCount) return false;
                }
            }

            return true;
        }

        public void CheckUnitsStrengthIsFull()
        {
            //ADJUSTEVENT
            /*AdjustEvent app_open = new AdjustEvent("f56e9t");
            Adjust.trackEvent(app_open);*/

            var isFullStrength = CheckUnitsInLine(GetUnitsWithMercenaries(LineType.Air), LineType.Air) && 
                                 CheckUnitsInLine(GetUnitsWithMercenaries(LineType.Ground), LineType.Ground) && 
                                 CheckUnitsInLine(GetUnitsWithMercenaries(LineType.Underground), LineType.Underground);
            if (isFullStrength) 
                _startController.StartGame();
            else
                _checkScreen.SetActive(true);
        }

        public void ChangeSettings()
        {
            _bot = !_bot;
            _changeText.text = _bot ? "Bot" : "Player";
            
            OnEnable();
        }

        public void ShowUnitInfo(UnitConfig unitConfigs)
        {
            _unitItemInfo.gameObject.SetActive(true);
            _unitItemInfo.init(unitConfigs);
        }
    }
}
