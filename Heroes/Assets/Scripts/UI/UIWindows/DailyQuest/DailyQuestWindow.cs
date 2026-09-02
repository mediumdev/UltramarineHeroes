using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Game.Controllers;
using TMPro;
using UI.Windows;
using UnityEngine;
using Utils;
using Utils.SaveManager;

namespace UI.UIWindows.DailyQuest
{
    public class DailyQuestWindow : Window
    {
        [SerializeField] private DailyQuestItem _item;
        [SerializeField] private RectTransform _itemContainer;
        [SerializeField] private TextMeshProUGUI _timer;

        private List<DailyQuestConfig> _dailyQuestList;
        private TimeSpan _timerSpan;
        private Coroutine _timerCoroutine;

        private void OnEnable()
        {
            _dailyQuestList = DailyQuestManager.Instance.GetCurrentDailyQuests();
            var finishedDailyQuests = SaveManager.GetValue<string>(SavedDataManager.FinishedDailyQuestsKey)
                .Split(';').ToList();
        
            foreach (var questConfig in _dailyQuestList)
            {
                var item = Instantiate(_item, _itemContainer);
                item.Init(questConfig, this, !finishedDailyQuests.Contains(questConfig.Uid));
            }
        
            _timerCoroutine = StartCoroutine(TimerUpdate());
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
    }
}
