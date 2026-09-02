using System;
using System.Collections.Generic;
using Configs;
using DevToDev.Analytics;
using DG.Tweening;
using Dynamic;
using Enums;
using Game.Controllers;
using Photon.Pun;
using Network;
using PhotonUtils;
using Structs;
using TMPro;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.SaveManager;

namespace UI
{
    public class DebriefingWindow : Window
    {
        [SerializeField] private GameObject _elo;
        [SerializeField] private Image _button;
        [SerializeField] private Image _back;
        [SerializeField] private Image _title;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private PlayerType _player;
        [SerializeField] private DebriefingItem _debriefingItem;
        [SerializeField] private RectTransform _container;

        private List<CurrencyWithCount> _rewardList = new List<CurrencyWithCount>();
        public PlayerType Player => _player;

        private void OnDisable()
        {
            PhotonSingleton.Instance.RaiseEvent((byte) NetworkEvents.PlayerDisconnected, null);
        }

        public void Init(List<RewardContainerConfig> rewardsList)
        {
            //PhotonNetwork.LeaveRoom();
            
            if (_player == PlayerType.Player)
            {
                var rewards = LootManager.Instance.GetLootFromContainerList(rewardsList);
                CurrencyManager.Instance.AddCurrencyValue(rewards);
                _rewardList = rewards;
                
                foreach (var reward in rewards)
                    Debug.Log($"Выдано за победу {reward.count}шт {reward.currency.name}");
            }

            SaveResults();
            RepaintInit();
        }

        private void RepaintInit()
        {
            _back.transform.DOScale(0f, 0f);
            _title.transform.DOScale(0f, 0f);
            _titleText.DOFade(0f, 0f);
            _button.DOFade(0f, 0f);
            _buttonText.DOFade(0f, 0f);
            
            _back.gameObject.SetActive(true);
            _title.gameObject.SetActive(true);

            var title = DOTween.Sequence();
            title.AppendInterval(0.3f);
            title.Append(_title.transform.DOScale(1.1f, 0.2f));
            title.Append(_title.transform.DOScale(1f, 0.1f));
            title.AppendInterval(0.3f).OnComplete(() =>
            {
                _titleText.DOFade(1f, 0.2f);
            });

            var back = DOTween.Sequence(); 
            back.AppendInterval(0.4f);
            back.Append(_back.transform.DOScale(1.1f, 0.2f));
            back.Append(_back.transform.DOScale(1f, 0.1f));
            /*back.AppendInterval(0.5f).OnComplete(() =>
            {
                _elo.gameObject.SetActive(true);
                _elo.transform.DOMoveY(_elo.transform.position.y - 80f, 0.4f);
            });*/

            var button = DOTween.Sequence();
            button.AppendInterval(2.5f).OnComplete(() =>
            {
                _button.DOFade(1f, 0.1f);
                _buttonText.DOFade(1f, 1f );
            });

            foreach (var reward in _rewardList)
            {
                var item = Instantiate(_debriefingItem, _container);
                item.Init(reward);
            }
        }

        private void SaveResults()
        {
            var win = _player == PlayerType.Player;
            var fightMode = SavedDataManager.GetFightMode();
            var fightDataKey = string.Empty;
            var finishedFightsDataKey = string.Empty;
            
            switch (fightMode)
            {
                case SavedDataManager.FightModeCampaign:
                    fightDataKey = DynamicDataManager.CurrentCampaignFightKey;
                    finishedFightsDataKey = SavedDataManager.FinishedCampaignFightsKey;
                    break;
                case SavedDataManager.FightModeDailyQuest:
                    fightDataKey = DynamicDataManager.CurrentDailyQuestKey;
                    finishedFightsDataKey = SavedDataManager.FinishedDailyQuestsKey;
                    break;
                case SavedDataManager.FightModePvp:
                    DTDAnalyticsEvents.FightPvp(win);
                    break;
            }
                
            if (fightDataKey != string.Empty && finishedFightsDataKey != string.Empty)
            {
                var fightUid = DynamicVarLibrary.Instance.GetVar(fightDataKey);
                if (fightUid == string.Empty)
                {
                    Debug.LogError($"Бой {fightMode} завершен, но нет информации о uid боя");
                }
                else
                {
                    var finishedFights = SaveManager.GetValue<string>(finishedFightsDataKey);
                    var finishedFightsUpdated = finishedFights == default
                        ? fightUid
                        : $"{finishedFights};{fightUid}";
                    SaveManager.Add(finishedFightsDataKey, finishedFightsUpdated);

                    if (fightMode == SavedDataManager.FightModeCampaign)
                    {
                        var key = SavedDataManager.LastFinishedCampaignFightKey;
                        SaveManager.Add(key, fightUid);
                        DTDAnalyticsEvents.FightCampaign(win, fightUid);
                    }
                }
            }
            
            var firstBattleWin = SaveManagerSafe.GetValue(SavedDataManager.FirstBattleWinKey, false);
            if (firstBattleWin) return;

            SaveManagerSafe.Add(SavedDataManager.FirstBattleWinKey, win);
        }
        
        public void ContinueButton()
        {
            Close();
        }
    }
}
