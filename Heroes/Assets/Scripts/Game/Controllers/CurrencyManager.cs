using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using CoreConfigs.Configs;
using CoreUtils.Utils;
using Dynamic;
using Enums;
using Newtonsoft.Json;
using Structs;
using UnityEngine;
using Utils.SaveManager;

namespace Game.Controllers
{
    public class CurrencyManager : MonoSingleton<CurrencyManager>
    {
        private static readonly Dictionary<string, int> CurrencyDictionary = new Dictionary<string, int>();
        private const string AccountInitializedKey = "AccountCurrenciesInitialized";
        private const string CurrencyDictionaryKey = "CurrencyDictionary";
    
        public event Action<List<CurrencyWithCount>> CurrencyChangedEvent;

        protected override void Init()
        {
            base.Init();

            if (!SaveManagerSafe.GetValue(AccountInitializedKey, false))
            {
                var initialStateConfig = ConfigBase.LoadFirstAvailableConfig<AccountInitialStateConfig>();
                SetCurrencyValue(initialStateConfig.Currencies);
                SaveManagerSafe.Add(AccountInitializedKey, true);
            }
            else
            {
                foreach (var currencyConfig in ConfigBase.LoadAll<CurrencyConfig>())
                {
                    var currencyCount = DynamicVarLibrary.Instance.GetVar(currencyConfig.Uid);
                    CurrencyDictionary[currencyConfig.Uid] = currencyCount == string.Empty 
                        ? 0 
                        : int.Parse(currencyCount);
                }
            }
            
            DontDestroyOnLoad(gameObject);
        }

        public void LoadDataFromFile()
        {
            var currencies = JsonConvert
                .DeserializeObject<Dictionary<string, int>>(
                    SaveManagerSafe.GetValue(CurrencyDictionaryKey, "{}")
                    )
                .Select(x => new CurrencyWithCount
            {
                currency = ConfigBase.LoadConfig<CurrencyConfig>(x.Key),
                count = x.Value
            }).ToList();
            SetCurrencyValue(currencies);
        }

        public void SaveDataToFile()
        {
            SaveManagerSafe.Add(CurrencyDictionaryKey, JsonConvert.SerializeObject(CurrencyDictionary));
        }

        private void SaveCurrenciesToDynamicVars(List<CurrencyConfig> configList = null)
        {
            if (configList is null)
                configList = CurrencyConfigs();

            foreach (var currencyConfig in configList)
                DynamicVarLibrary.Instance.AddVar(currencyConfig.Uid, CurrencyValue(currencyConfig));
        }

        private void SetCurrencyValueBase(List<CurrencyWithCount> currencyItems, bool addValue, bool useOldValue)
        {
            if (!addValue)
            {
                var currencyValuesValid = true;
                foreach (var currencyItem in currencyItems)
                {
                    if (currencyItem.currency == null) continue;
                    
                    var currencyValue = useOldValue && CurrencyDictionary.ContainsKey(currencyItem.currency.Uid) 
                        ? CurrencyDictionary[currencyItem.currency.Uid] 
                        : 0;
                    var sign = addValue ? 1 : -1;
                    currencyValue += Math.Abs(currencyItem.count) * sign;

                    if (currencyValue >= 0) continue;

                    currencyValuesValid = false;
                    Debug.LogError($"Количество валюты не может быть меньше 0: " +
                                   $"валюта '{currencyItem.currency.name}', количество = '{currencyValue}'");
                }
                if (!currencyValuesValid) return;
            }
        
            var currenciesUpdated = new List<CurrencyConfig>();
            foreach (var currencyItem in currencyItems)
            {
                if (currencyItem.currency == null) continue;
                
                var currencyValue = useOldValue && CurrencyDictionary.ContainsKey(currencyItem.currency.Uid) 
                    ? CurrencyDictionary[currencyItem.currency.Uid] 
                    : 0;
                var sign = addValue ? 1 : -1;
                currencyValue += Math.Abs(currencyItem.count) * sign;
            
                CurrencyDictionary[currencyItem.currency.Uid] = currencyValue;
                currenciesUpdated.Add(currencyItem.currency);
            }
            SaveCurrenciesToDynamicVars(currenciesUpdated);
            SaveDataToFile();

            CurrencyChangedEvent?.Invoke(currenciesUpdated.Select(x => new CurrencyWithCount
            {
                currency = x, 
                count = CurrencyValue(x)
            }).ToList());
        }

        public void SetCurrencyValue(List<CurrencyWithCount> currencyItems)
        {
            SetCurrencyValueBase(currencyItems, true, false);
        }

        public void SetCurrencyValue(CurrencyConfig currencyConfig, int cost)
        {
            SetCurrencyValue(new List<CurrencyWithCount>
            {
                new CurrencyWithCount { currency = currencyConfig, count = cost }
            });
        }
        
        public void SubtractCurrencyValue(List<CurrencyWithCount> currencyItems)
        {
            SetCurrencyValueBase(currencyItems, false, true);
        }

        public void SubtractCurrencyValue(CurrencyConfig currencyConfig, int cost)
        {
            SubtractCurrencyValue(new List<CurrencyWithCount>
            {
                new CurrencyWithCount { currency = currencyConfig, count = cost }
            });
        }

        public void AddCurrencyValue(List<CurrencyWithCount> currencyItems)
        {
            SetCurrencyValueBase(currencyItems, true, true);
        }

        public void AddCurrencyValue(CurrencyConfig currencyConfig, int cost)
        {
            AddCurrencyValue(new List<CurrencyWithCount>
            {
                new CurrencyWithCount { currency = currencyConfig, count = cost }
            });
        }

        public bool EnoughCurrency(List<CurrencyWithCount> currencyWithValueList)
        {
            return currencyWithValueList.TrueForAll(x => CurrencyValue(x.currency) >= x.count);
        }

        public bool EnoughCurrency(CurrencyConfig currencyConfig, int neededValue)
        {
            return EnoughCurrency(new List<CurrencyWithCount>
            {
                new CurrencyWithCount { currency = currencyConfig, count = neededValue }
            });
        }

        public int CurrencyValue(string currencyUid)
        {
            return CurrencyDictionary.ContainsKey(currencyUid) ? CurrencyDictionary[currencyUid] : 0;
        }

        public int CurrencyValue(CurrencyConfig currencyConfig)
        {
            return CurrencyValue(currencyConfig.Uid);
        }

        public List<CurrencyConfig> CurrencyConfigs()
        {
            return ConfigBase.LoadAll<CurrencyConfig>().ToList();
        }

        public List<CurrencyConfig> CurrencyConfigsWithType(CurrencyType currencyType)
        {
            return CurrencyConfigs().Where(x => x.currencyType == currencyType).ToList();
        }

        public List<BaseItemConfig> BaseItemConfigs()
        {
            return ConfigBase.LoadAll<BaseItemConfig>().ToList();
        }
    }
}