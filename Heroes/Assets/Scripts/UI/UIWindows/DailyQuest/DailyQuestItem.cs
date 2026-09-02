using System.Linq;
using Configs;
using Game.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.SaveManager;

namespace UI.UIWindows.DailyQuest
{
    public class DailyQuestItem : MonoBehaviour
    {
        [Header("Active Quest")] 
        [SerializeField] private GameObject _activeObject;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _factionIcon;
        [SerializeField] private Image _rewardIcon;

        [Header("Completed Quest")] 
        [SerializeField] private GameObject _completedObject;
        [SerializeField] private TextMeshProUGUI _nameTextCompleted;
        [SerializeField] private Image _factionIconCompleted;
        [SerializeField] private Image _rewardIconCompleted;

        private DailyQuestConfig _config;
        private bool _active = true;
        private DailyQuestWindow _dailyQuestWindow;

        public void Init(DailyQuestConfig config, DailyQuestWindow dailyQuestWindow, bool active = true)
        {
            _config = config;
            _dailyQuestWindow = dailyQuestWindow;
            _active = active;
            
            _completedObject.SetActive(!active);
            _activeObject.SetActive(active);
            
            var rewardIcon = _config.Rewards[0].RewardLists[0].rewards[0].currencyRewardWithRange.currency.Icon;
            var questName = _config.Name;
            var factionIcon = _config.Icon;
            
            if (_active)
            {
                _nameText.text = questName;
                _factionIcon.sprite = factionIcon;
                _rewardIcon.sprite = rewardIcon;
                _descriptionText.text = _config.Description;
            }
            else
            {
                _nameTextCompleted.text = questName;
                _factionIconCompleted.sprite = factionIcon;
                _rewardIconCompleted.sprite = rewardIcon;
            }
        }

        public void StartBattle()
        {
            if (!_active) return;

            _dailyQuestWindow.Close();
            DailyQuestManager.StartDailyQuest(_config);
        }
    }
}
