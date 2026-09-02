using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using DevToDev.Analytics;
using Game.Controllers;
using Packages.CoreUtils.Utils;
using Structs;
using TMPro;
using UI.Windows;
using UnityEngine;
using Utils;
using Utils.SaveManager;

namespace UI
{
    public class MercenaryWindow : Window
    {
        [SerializeField] private int _rangeSize = 3;
        [SerializeField] private MercenaryItem _item;
        [SerializeField] private Tooltip _unitItemInfo;
        [SerializeField] private Transform _container;
        [SerializeField] private MercenarySetConfig _mercConfig;
        [SerializeField] private TextMeshProUGUI _timer;

        private MercenariesController _mercController;
        
        private TimeSpan _timerSpan;
        private Coroutine _timerCoroutine;

        private List<Mercenary> _units;
        private List<int> _purchasedIDs;
    
        private void OnEnable()
        {
            _mercController = MercenariesController.Instance;
            
            _units = new List<Mercenary>();
            _purchasedIDs = new List<int>();
            
            var mercenariesData = SaveManager.GetValue(SavedDataManager.ShopRangeOfMercenariesKey, string.Empty);
            var shopRange = mercenariesData == string.Empty
                ? new List<int>() 
                : mercenariesData.Split(';').Select(m => int.Parse(m)).ToList();
            
            if (DailyUpdateController.Instance.MercenaryRangeNeedsUpdate())
                UpdateCurrentRange();
            else
            {
                if (shopRange.Count != _rangeSize || !IsCorrectIDList(shopRange))
                {
                    if (shopRange.Count != _rangeSize)
                        Debug.LogWarning($"Amount of saved range IDs ({shopRange.Count}) " +
                                         $"isn't equal to range size ({_rangeSize}). Creating new range.");
                    else
                        Debug.LogWarning($"There is incorrect IDs list: {mercenariesData}. Creating new range.");
                    
                    UpdateCurrentRange();
                }
                else
                {
                    _units = _mercConfig.GetMercenaries(shopRange);
                    Debug.Log($"Get Mercenaries: shop count {shopRange.Count} | units count {_units.Count}");
                }
            }
        
            _units.Sort(Comparison);

            // Get info about already purchased units for today
            var purchasedIDs = SaveManager.GetValue(SavedDataManager.PurchasedMercenariesKey, string.Empty);
        
            if (purchasedIDs != String.Empty)
            {
                var split = purchasedIDs.Split(';');
                foreach (var splitStr in split)
                    _purchasedIDs.Add(int.Parse(splitStr));
            }
        
            CreateSectionPanel();
            _timerCoroutine = StartCoroutine(TimerUpdate());
        }

        private bool IsCorrectIDList(List<int> ids)
        {
            foreach (var id in ids)
            {
                if (id >= _mercConfig.MercenariesCount || id < 0)
                    return false;
            }
            return true;
        }

        private void UpdateCurrentRange()
        {
            var totalAmount = _mercConfig.MercenariesCount;
        
            var IDs = new List<int>();
            for (int i = 0; i < totalAmount; i++)
                IDs.Add(i);
        
            IDs.Shuffle();
            var newRange = IDs.Take(_rangeSize).ToList();

            _mercController.ResetRange(newRange);
            _units = _mercConfig.GetMercenaries(newRange);
        
            SaveManager.Add(SavedDataManager.LastMercenaryUpdateKey, DateTime.UtcNow.ToString());
            SaveManager.Add(SavedDataManager.PurchasedMercenariesKey, string.Empty);
        }

        private void CreateSectionPanel()
        {
            int i = 0;
            foreach (var unit in _units)
            {
                var item = Instantiate(_item, _container);
                item.Init(this, unit, i, _purchasedIDs.Contains(i));
                i++;
            }
        }
    
        private int Comparison(Mercenary x, Mercenary y)
        {
            return x.Config.Cost > y.Config.Cost ? 1 : -1;
        }

        public void ShowUnitInfo(UnitConfig unitConfigs)
        {
            _unitItemInfo.gameObject.SetActive(true);
            _unitItemInfo.init(unitConfigs);
        }

        public bool Purchase(Mercenary unit, int slotId)
        {
            var money = new List<CurrencyWithCount> {unit.CurrencyCost};
            if (!CurrencyManager.Instance.EnoughCurrency(money)) return false;
        
            _mercController.AddUnitsToStock(unit);
            
            _purchasedIDs.Add(slotId);
            var purchasedString = string.Empty;
            for (var i = 0; i < _purchasedIDs.Count; i++)
                purchasedString += i == _purchasedIDs.Count - 1
                    ? $"{_purchasedIDs[i]}"
                    : $"{_purchasedIDs[i]};";
            
            CurrencyManager.Instance.SubtractCurrencyValue(money);

            DTDAnalyticsEvents.Purchase($"Mercenary: {unit.Config.Name}", false);

            SaveManager.Add(SavedDataManager.PurchasedMercenariesKey, purchasedString);
            return true;
        }
    
        private IEnumerator TimerUpdate()
        {
            var currentTime = DateTime.UtcNow;
            _timerSpan = currentTime.Date.AddDays(1) - currentTime;
        
            _timer.text = $"{_timerSpan.Hours}h {_timerSpan.Minutes}m";

            var timer = 30f;
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }

            _timerCoroutine = StartCoroutine(TimerUpdate());
            yield return null;
        }

#if UNITY_EDITOR

        [ContextMenu("Clear Purchased Line In Save File")]
        public void ClearPurchasedLineInSaveFiles()
        {
            SaveManager.Add("PurchasedMercenaries", string.Empty);
        }
#endif
    }
}
