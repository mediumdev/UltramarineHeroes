using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using CoreConfigs.Configs;
using CoreUtils.Utils;
using DevToDev.Analytics;
using Enums;
using Game.Units;
using Newtonsoft.Json;
using Structs;
using UnityEngine;
using Utils;
using Utils.SaveManager;
using WebSocketSharp;

namespace Game.Controllers
{
    public class PlayerFactionsController : MonoSingleton<PlayerFactionsController>
    {
        private Dictionary<FactionConfig, int> _factionProgress = new Dictionary<FactionConfig, int>();
        private Dictionary<FactionConfig, UnitConfig[]> _factionUnitsUnlocked;
        private Dictionary<UnitConfig, int> _unitsUnlockLevels = new Dictionary<UnitConfig, int>();

        public FactionConfig[] FactionsList => FactionsController.Instance.FactionsList;
        public event Action FactionUpgradedEvent;

        protected override void Init()
        {
            base.Init();
            
            if (_factionProgress.Count == 0)
                LoadPlayerFactions();

            SaveUnitUnlockLevels();
        }

        private static Dictionary<FactionConfig, int> FactionProgressFromString(string factionsData)
        {
            var factionsList = JsonConvert.DeserializeObject<Dictionary<string, int>>(factionsData);
            var factionProgress = new Dictionary<FactionConfig, int>();
            foreach (var data in factionsList)
            {
                var config = ConfigBase.LoadConfig<FactionConfig>(data.Key);
                factionProgress[config] = data.Value;
            }

            return factionProgress;
        }

        private static string FactionProgressToString(Dictionary<FactionConfig, int> factionProgress)
        {
            var factionsList = new Dictionary<string, int>();
            foreach (var data in factionProgress)
                factionsList[data.Key.Uid] = data.Value;

            return JsonConvert.SerializeObject(factionsList);
        }

        public void LoadPlayerFactions()
        {
            var factionsData = SaveManager.GetValue(SavedDataManager.FactionProgressKey, string.Empty);
            var factionProgress = factionsData == string.Empty
                ? new Dictionary<FactionConfig, int>() 
                : FactionProgressFromString(factionsData);
            foreach (var factionConfig in FactionsList)
                if (!factionProgress.ContainsKey(factionConfig))
                    factionProgress[factionConfig] = 0;
            _factionProgress = factionProgress;
            SavePlayerFactions();
        }

        private void SavePlayerFactions()
        {
            SaveManager.Add(SavedDataManager.FactionProgressKey, FactionProgressToString(_factionProgress));
        }

        public List<UnitConfig> GetFactionUnitsUnlocked(FactionConfig faction)
        {
            if (faction is null)
            {
                Debug.LogWarning("Null faction in GetFactionUnitsUnlocked");
                return new List<UnitConfig>();
            }
            
            if (!_factionProgress.ContainsKey(faction))
            {
                _factionProgress[faction] = 0;
                SavePlayerFactions();
            }
            var progress = _factionProgress[faction];
            return faction.FactionProgress[progress].unitsData.Select(x => x.config).ToList();
        }

        public List<UnitConfig> GetFactionUnits(FactionConfig faction, bool isPlayer = true)
        {
            return isPlayer 
                ? GetFactionUnitsUnlocked(faction) 
                : faction.FactionProgress.Last().unitsData.Select(x => x.config).ToList();
        }
        
        public List<UnitConfig> GetAllFactionUnits(FactionConfig faction)
        {
            return faction.FactionProgress.Last().unitsData.Select(x => x.config).ToList();
        }

        public int GetFactionProgress(FactionConfig faction)
        {
            return _factionProgress[faction];
        }

        public int GetFactionProgressSum(List<string> factionUids = null)
        {
            var factions = factionUids?.Count > 0
                ? _factionProgress.Where(x => factionUids.Contains(x.Key.Uid))
                : _factionProgress;
            return factions.Sum(x => x.Value);
        }

        public bool IsFactionMaxUpgraded(FactionConfig faction)
        {
            var progress = _factionProgress[faction];
            return faction.FactionProgress.Length - 1 <= progress;
        }

        public CurrencyWithCount[] GetUpgradePrice(FactionConfig faction)
        {
            var progress = _factionProgress[faction];
            return progress < faction.FactionProgress.Length - 1
                ? faction.FactionProgress[progress + 1].cost
                : new CurrencyWithCount[0];
        }

        public List<UnitWithLimits> GetCurrentFactionUnitsData(FactionConfig faction)
        {
            var progress = _factionProgress[faction];
            return faction.FactionProgress[progress].unitsData;
        }

        public List<UnitWithLimits> GetNextFactionUnitsData(FactionConfig faction)
        {
            var progress = _factionProgress[faction];
            return progress < faction.FactionProgress.Length - 1
                ? faction.FactionProgress[progress + 1].unitsData
                : new List<UnitWithLimits>();
        }

        public FactionUpgradeResult UpgradeFaction(FactionConfig faction)
        {
            var progress = _factionProgress[faction];
            if (IsFactionMaxUpgraded(faction))
            {
                Debug.LogWarning($"Фракция {faction.name} уже прокачена до максимума");
                return FactionUpgradeResult.MaxProgress;
            }

            var upgradeCost = faction.FactionProgress[progress + 1].cost.ToList();
            if (!CurrencyManager.Instance.EnoughCurrency(upgradeCost))
            {
                Debug.LogWarning($"Недостаточно валюты для улучшения {faction.name}");
                return FactionUpgradeResult.NotEnoughCurrency;
            }
            
            CurrencyManager.Instance.SubtractCurrencyValue(upgradeCost);
            _factionProgress[faction] += 1;

            DTDAnalyticsEvents.FactionUpgrade(faction.name, _factionProgress[faction]);

            SavePlayerFactions();
            UnitLimitManager.Instance.UpdateLimitsData();
            FactionUpgradedEvent?.Invoke();
            return FactionUpgradeResult.Ok;
        }

        private void SaveUnitUnlockLevels()
        {
            _unitsUnlockLevels = new Dictionary<UnitConfig, int>();

            foreach (var factionConfig in FactionsList)
            {
                for (var i = 0; i < factionConfig.FactionProgress.Length; i++)
                {
                    var progress = factionConfig.FactionProgress[i];
                    foreach (var unitData in progress.unitsData)
                    {
                        if (_unitsUnlockLevels.ContainsKey(unitData.config)) continue;

                        _unitsUnlockLevels[unitData.config] = i;
                    }
                }   
            }
        }

        public int GetUnitUnlockLevel(UnitConfig unitConfig)
        {
            return _unitsUnlockLevels[unitConfig];
        }

        public int GetTowerListHealth(List<Tower> towers)
        {
            return towers.Sum(config => GetFactionTowerHealth(config.Faction));
        }

        public int GetFactionTowerHealth(FactionConfig config, bool next = false)
        {
            var progress = _factionProgress[config] + (next ? 1 : 0);
            return config.FactionProgress[progress].towerHealth;
        }
    }
}