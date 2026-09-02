using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using DG.Tweening;
using Dynamic;
using Enums;
using Game.Controllers;
using Photon.Pun;
using RTLTMPro;
using TMPro;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.SaveManager;

namespace UI
{
    public class PlayerPanel : MonoBehaviour
    {
        [SerializeField] private RTLTextMeshPro _healthValue;
        [SerializeField] private RTLTextMeshPro _manaValue;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private RTLTextMeshPro _nickName;
        [SerializeField] private Image _avatarImage;
        [SerializeField] private Image[] _capturedTower;
        [SerializeField] private GameObject _manaParent;
        [SerializeField] private Image _gemImage;
        [SerializeField] private RTLTextMeshPro _takenDamage;
        [SerializeField] private bool _isMaster;
        [SerializeField] private Image[] _manaGems;
        [SerializeField] private Window _debriefingWindow;
        [SerializeField] private List<string> _botNames;
        [SerializeField] private List<int> _botHpValues;

        private float _accumulatedDamage;
        private Queue<float> _damageQueue;
        private float _time;
        private int _gemFade;
        private int _towerOwned;
        private DebriefingWindow _debriefing;
        
        private bool IsPlayer => _isMaster && PhotonNetwork.IsMasterClient || !_isMaster && !PhotonNetwork.IsMasterClient;

        public Image GemImage
        {
            get => _gemImage;
            set => _gemImage = value;
        }
        public float Time
        {
            get => _time;
            set => _time = value;
        }

        public event Action PlayerDeadEvent;

        private void Awake()
        {
            if (_botHpValues.Count == 0) return;
            
            var rnd = new System.Random();
            var hpChoiceIdx = rnd.Next(_botHpValues.Count);
            DynamicVarLibrary.Instance.AddVar("BotHealthValue", _botHpValues[hpChoiceIdx]);
        }

        private void OnEnable()
        {
            _nickName.gameObject.SetActive(true);
            
            if (_debriefingWindow.GetComponent<DebriefingWindow>().Player == PlayerType.Enemy)
            {
                _nickName.text = CustomizationManager.Instance.PlayerName;
                _avatarImage.sprite = CustomizationManager.Instance.CurrentAvatarImage;
            }
            else
            {
                var rnd = new System.Random();
                var units = CustomizationManager.Instance.AllUnitsIcons;
                var idx = rnd.Next(units.Count);
                _avatarImage.sprite = units[idx];
                if (_botNames.Count > 0)
                {
                    var nameIdx = rnd.Next(_botNames.Count);
                    _nickName.text = _botNames[nameIdx];
                }
                else
                {
                    _nickName.gameObject.SetActive(false);
                }
            }
            _manaValue.transform.parent.gameObject.SetActive(IsPlayer);
            _manaValue.text = $"{_playerController.Mana}";
            _manaParent.SetActive(true);
            var otherPlayer = PhotonNetwork.PlayerListOthers.FirstOrDefault();
            if (otherPlayer != null && !IsPlayer)
            {
                _nickName.text = otherPlayer.NickName;
            }

            GameController.Instance.ControllerTickEvent += ParseDamage;
            _playerController.ManaChangeEvent += OnManaChange;
            _playerController.HealthChangeEvent += HealthChange;
            _damageQueue = new Queue<float>();
        }

        private void Start()
        {
            GameController.Instance.GameMachine.OnArmyLost += DebriefingActivation;
        }

        private void OnDisable()
        {
            GameController.Instance.ControllerTickEvent -= ParseDamage;
            _playerController.ManaChangeEvent -= OnManaChange;
            _playerController.HealthChangeEvent -= HealthChange;
            
           if (GameController.Instance.GameMachine != null)
               GameController.Instance.GameMachine.OnArmyLost -= DebriefingActivation;
        }

        private void ParseDamage()
        {
            if (_takenDamage == null)
                return;

            _damageQueue.Enqueue(_accumulatedDamage);
            _accumulatedDamage = 0;

            if (_damageQueue.Count >= 3)
                _damageQueue.Dequeue();

            var value = _damageQueue.Sum();

            _takenDamage.gameObject.SetActive(value != 0);
            _takenDamage.text = value > 0 ? $"+{value}" : $"{value}";
        }

        private void HealthChange(int health, int changeValue)
        {
            _accumulatedDamage += changeValue;
            
            if (_healthValue != null || _debriefing != null)
                _healthValue.text = health.ToString();

            if (health > 0) return;
                
            PlayerDeadEvent?.Invoke();
            GameController.Instance.GameMachine.GameStop();
            
            var rewardsList = new List<RewardContainerConfig>();
            var fightMode = SavedDataManager.GetFightMode();
            switch (fightMode)
            {
                case SavedDataManager.FightModePvp:
                    Debug.Log("Получаем награды для FightModePvp");
                    rewardsList = GameController.Instance.GameMachine.GameRewardController.PvpRewards;
                    break;
                case SavedDataManager.FightModeCampaign:
                    var finishedFights = SavedDataManager.GetFinishedCampaignFights();
                    var currentFight = DynamicVarLibrary.Instance.GetVar(DynamicDataManager.CurrentCampaignFightKey);
                    
                    if (finishedFights.Contains(currentFight))
                    {
                        Debug.Log("Уровень кампании уже пройден. Получаем награды для FightModePvp");
                        rewardsList = GameController.Instance.GameMachine.GameRewardController.PvpRewards;
                    }
                    else
                    {
                        Debug.Log("Получаем награды для FightModeCampaign");
                        rewardsList = DynamicDataManager.GetNextRewards();
                    }
                    
                    break;
                case SavedDataManager.FightModeDailyQuest:
                    Debug.Log("Получаем награды для FightModeDailyQuest");
                    rewardsList = DynamicDataManager.GetNextRewards();
                    break;
                default:
                    Debug.LogError($"Unknown fight mode {fightMode}");
                    break;
            }
            
            _debriefing = WindowManager.Instance.Open(_debriefingWindow) as DebriefingWindow;
            if (_debriefing != null)
                _debriefing.Init(rewardsList);
        }

        private void DebriefingActivation(bool isPlayer) // Активация дебрифинга победы, в случае истощения армии врага
        {
            var controlledByPlayer = _playerController.PlayerType == PlayerType.Player;
            
            if (isPlayer && controlledByPlayer) // Победа за врагом, армия игрока истощена
            {
                _debriefing = WindowManager.Instance.Open(_debriefingWindow) as DebriefingWindow;
                if (_debriefing != null)
                    _debriefing.Init(new List<RewardContainerConfig>());
            }
            else if (!isPlayer && !controlledByPlayer) // Победа за игроком
            {
                // Тут возможно ускорение сцены сражения, чтобы дать игроку добить соперника быстрее
            }
        }

        private void OnManaChange(int manaCount)
        {
            ManaGems();
            ManaView();
            CapturedTowers();
        }

        private void ManaGems()
        {
            foreach (var gem in _manaGems)
                gem.DOFade(0f, 0f);
            
            var value = Math.Min(_playerController.Mana, 10);

            if (_gemFade <= value)
            {
                _gemFade = value;
                
                for (var i = 0; i < value; i++)
                    _manaGems[i].DOFade(1f, 0f);
            }
            
            else
            {
                for (var i = 0; i < _gemFade; i++) _manaGems[i].DOFade(1f, 0f);
                
                for (int i = value; i < _gemFade; i++) _manaGems[i].DOFade(0f, 0.2f);

                _gemFade = value;
            }
        }

        private void ManaView()
        {
            if (_manaValue != null)
                _manaValue.text = $"{_playerController.Mana}";
        }

        private void CapturedTowers()
        {
            if (_capturedTower.Length <= 0) return;

            var towerCount = _playerController.CurrentGeneratedMana;

            if (_towerOwned < towerCount)
            {
                for (var i = _towerOwned; i < towerCount; i++)
                {
                    var seq = DOTween.Sequence();
                    seq.Append(_capturedTower[i].DOFade(1f,0f));
                    seq.Append(_capturedTower[i].gameObject.transform.DOScale(1.15f, 0.2f));
                    seq.Append(_capturedTower[i].gameObject.transform.DOScale(1f, 0.2f));
                }
                _towerOwned = towerCount;
            }
            else if (_towerOwned > towerCount)
            {
                for (int i = towerCount; i < _towerOwned; i++)  _capturedTower[i].DOFade(0f, 0.3f);
                _towerOwned = towerCount;
            }
        }
    }
}
