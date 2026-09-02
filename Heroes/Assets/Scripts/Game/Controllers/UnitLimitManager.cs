using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using CoreConfigs.Configs;
using CoreUtils.Utils;
using Dynamic;
using Newtonsoft.Json;
using UnityEngine;
using Utils;
using Utils.SaveManager;

namespace Game.Controllers
{
    [Serializable]
    public struct UnitGeneration
    {
        public UnitConfig config;
        public int count;
        public DateTime dateTime;
    }
    
    public class UnitLimitManager : MonoSingleton<UnitLimitManager>
    {
        private static readonly Dictionary<UnitConfig, int> PlayerUnitsCount = new Dictionary<UnitConfig, int>();
        private static readonly Dictionary<UnitConfig, int> PlayerUnitsLimit = new Dictionary<UnitConfig, int>();
        private static readonly Dictionary<UnitConfig, DateTime> LastGeneration = new Dictionary<UnitConfig, DateTime>();

        public Dictionary<UnitConfig, int> PlayerUnitsCountDict => PlayerUnitsCount;

        protected override void Init()
        {
            base.Init();
            
            if (!SaveManagerSafe.GetValue(SavedDataManager.AccountUnitLimitsInitializedKey, false))
            {
                var initialStateConfig = ConfigBase.LoadFirstAvailableConfig<AccountInitialStateConfig>();
                var initialUnits = initialStateConfig.UnitLimits
                    .Select(x => new UnitGeneration
                    {
                        config = x.unit, count = x.count, dateTime = DateTime.UtcNow
                    }).ToList();
                SetUnitCount(initialUnits);
                SaveManagerSafe.Add(SavedDataManager.AccountUnitLimitsInitializedKey, true);
            }
            else
            {
                PlayerUnitsCount.Clear();
                LastGeneration.Clear();
                
                foreach (var factionConfig in FactionsController.Instance.FactionsList)
                foreach (var unitLimits in PlayerFactionsController.Instance.GetCurrentFactionUnitsData(factionConfig))
                {
                    var unitConfig = unitLimits.config;
                    var unitCount = DynamicVarLibrary.Instance.GetVar(VarCount(unitConfig));
                    PlayerUnitsCount[unitConfig] = unitCount == string.Empty 
                        ? 0 
                        : int.Parse(unitCount);
                    
                    var genDate = DynamicVarLibrary.Instance.GetVar(VarDate(unitConfig));
                    LastGeneration[unitConfig] = genDate == string.Empty
                        ? DateTime.UtcNow 
                        : DateTime.Parse(genDate);
                }
            }
            
            if (PlayerUnitsCount.Sum(x => x.Value) == 0) LoadDataFromFile();
            
            UpdateLimitsData();
            GenerateUnits();

            TickController.Instance.MinutesTickEvent += GenerateUnits;
        }

        private void OnDisable()
        {
            TickController.Instance.MinutesTickEvent -= GenerateUnits;
        }

        private static string VarCount(ConfigBase config)
        {
            return $"{config.Uid}_unit_count";
        }

        private static string VarDate(ConfigBase config)
        {
            return $"{config.Uid}_generation_datetime";
        }

        private void SetUnitCount(List<UnitGeneration> generations)
        {
            foreach (var data in generations)
            {
                var unitConfig = data.config;
                
                PlayerUnitsCount[unitConfig] = data.count;
                if (data.dateTime != DateTime.MinValue)
                    LastGeneration[unitConfig] = data.dateTime;
                DynamicVarLibrary.Instance.AddVar(VarCount(unitConfig), data.count);
                DynamicVarLibrary.Instance.AddVar(VarDate(unitConfig), LastGeneration[unitConfig]);
            }
            
            SaveDataToFile();
        }

        private void SetUnitCount(UnitConfig unitConfig, int count, DateTime dateTime)
        {
            SetUnitCount(new List<UnitGeneration>
            {
                new UnitGeneration { config = unitConfig, count = count, dateTime = dateTime }
            });
        }

        public void LoadDataFromFile()
        {
            var lastGenerationStr = SaveManagerSafe.GetValue(SavedDataManager.LastUnitLimitGenerationKey, "{}");
            var lastGenerationData = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(lastGenerationStr);
            
            var playerUnitsStr = SaveManagerSafe.GetValue(SavedDataManager.PlayerUnitsCountKey, "{}");
            var generation = JsonConvert.DeserializeObject<Dictionary<string, int>>(playerUnitsStr)
                .Select(x => new UnitGeneration
                {
                    config = ConfigBase.LoadConfig<UnitConfig>(x.Key),
                    count = x.Value,
                    dateTime = lastGenerationData.ContainsKey(x.Key) ? lastGenerationData[x.Key] : DateTime.UtcNow
                }).ToList();
            
            SetUnitCount(generation);
        }

        private void SaveDataToFile()
        {
            var unitCountStr = JsonConvert.SerializeObject(PlayerUnitsCount
                .ToDictionary(x => x.Key.Uid, x => x.Value));
            var genDatetimeStr = JsonConvert.SerializeObject(LastGeneration
                .ToDictionary(x => x.Key.Uid, x => x.Value));
            
            SaveManagerSafe.Add(SavedDataManager.PlayerUnitsCountKey, unitCountStr );
            SaveManagerSafe.Add(SavedDataManager.LastUnitLimitGenerationKey, genDatetimeStr );
        }

        public void UpdateLimitsData()
        {
            PlayerUnitsLimit.Clear();
            foreach (var factionConfig in FactionsController.Instance.FactionsList)
            foreach (var unitLimits in PlayerFactionsController.Instance.GetCurrentFactionUnitsData(factionConfig))
            {
                var unitConfig = unitLimits.config;
                
                PlayerUnitsLimit[unitConfig] = unitLimits.limit;
                if (!LastGeneration.ContainsKey(unitConfig))
                    SetUnitCount(unitConfig, unitLimits.limit, DateTime.UtcNow);
            }
        }

        private void GenerateUnits()
        {
            var unitGenerationData = new List<UnitGeneration>();
            
            foreach (var factionConfig in PlayerFactionsController.Instance.FactionsList)
            foreach (var unitData in PlayerFactionsController.Instance.GetCurrentFactionUnitsData(factionConfig))
            {
                var unitConfig = unitData.config;
                var genAmount = unitData.generationAmount;
                var genSeconds = unitData.generationSeconds;
                var limit = unitData.limit;
                    
                if (LastUnitGeneration(unitConfig) + TimeSpan.FromSeconds(genSeconds) > DateTime.UtcNow)
                    continue;

                if (PlayerUnitsCount[unitConfig] >= limit)
                {
                    LastGeneration[unitConfig] = DateTime.UtcNow;
                    continue;
                }

                var secondsAfterGen = (DateTime.UtcNow - LastUnitGeneration(unitConfig)).TotalSeconds;
                var genSteps = (int) Math.Truncate(secondsAfterGen / genSeconds);
                var targetUnitCount = PlayerUnitsCount[unitConfig] + genAmount * genSteps;
                if (targetUnitCount > limit) targetUnitCount = limit;
                var targetTime = targetUnitCount == limit
                    ? DateTime.UtcNow
                    : LastUnitGeneration(unitConfig) + TimeSpan.FromSeconds(genSteps * genSeconds);
                        
                unitGenerationData.Add(new UnitGeneration
                {
                    config = unitConfig, 
                    count = targetUnitCount, 
                    dateTime = targetTime
                });
            }
            
            SetUnitCount(unitGenerationData);
        }

        public bool SubtractUnit(UnitConfig unitConfig, int count = 1)
        {
            if (unitConfig.IsMercenary)
            {
                MercenariesController.Instance.SubtractUnit(unitConfig, count);
            }
            else
            {
                if (PlayerUnitsCount[unitConfig] < count)
                {
                    Debug.LogWarning($"Юнита {unitConfig.name} слишком мало ({PlayerUnitsCount[unitConfig]}шт) " +
                                     $"для вычитания {count} штук");
                    return false;
                }

                SetUnitCount(unitConfig, PlayerUnitsCount[unitConfig] - count, DateTime.MinValue);
            }
            
            return true;
        }

        public bool SubtractUnit(string uid, int count = 1)
        {
            return SubtractUnit(ConfigBase.LoadConfig<UnitConfig>(uid), count);
        }

        public bool UnitInInventory(UnitConfig unitConfig)
        {
            return PlayerUnitsCount.ContainsKey(unitConfig);
        }

        public int GetUnitCount(UnitConfig unitConfig)
        {
            return PlayerUnitsCount[unitConfig];
        }

        public int GetUnitLimit(UnitConfig unitConfig)
        {
            return PlayerUnitsLimit[unitConfig];
        }

        public DateTime LastUnitGeneration(UnitConfig unitConfig)
        {
            return LastGeneration.ContainsKey(unitConfig) ? LastGeneration[unitConfig] : DateTime.UtcNow;
        }
    }
}