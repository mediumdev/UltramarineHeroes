using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Dynamic;
using Enums;
using Game.Ai;
using Game.Environment;
using Game.Units;
using Photon.Pun;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Controllers
{
    public class PlayerController : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] private PlayerType _playerType;
        [SerializeField] private NormalBot _bot;
        [SerializeField] private List<Tower> _lines;
        [SerializeField] private Transform[] _towerPositions;
        [SerializeField] private FactionDeckConfig[] _availableDecks;
        [SerializeField] private int _maxMana = 10;
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private PlayerPanel _playerPanel;
        [SerializeField] private int _halfMana;
        [SerializeField] private int _lowMana;
        private int _maxHealth = 10000;
        private int _currentHealth;
        private int _currentMana;
        private int _currentGeneratedMana;
        private float _manaWaitTime = 0f;
        private float _manaTickTime = 1f;
        private int _halfHP;
        private int _lowHP;
        private bool _fiftyPercent = false;
        private bool _tenPercent = false;
        private Coroutine _generateManaCoroutine;
        private bool IsBot => PlayerType == PlayerType.Enemy;

        public PlayerType PlayerType => _playerType;
        public NormalBot Bot => _bot;
        public List<UnitConfig> Collection { get; private set; }
        public List<FactionConfig> Factions { get; set; }
        public bool TowerAbilityIsCasting { get; set; } = false;
        public int Mana => _currentMana;
        public int CurrentGeneratedMana => _currentGeneratedMana;
        public float ManaTickTime => _manaTickTime;
        public List<OutlineActivator> TowerVisual { get; private set; }

        public List<Tower> Towers => _lines;
        public event Action<int, int> HealthChangeEvent;
        public event Action<int> ManaChangeEvent;

        private void OnValidate()
        {
            _playerPanel = GetComponent<PlayerPanel>();
        }

        private void OnEnable()
        {
            if (_playerType == PlayerType.Player)
                GameController.Instance.Player = this;
            else if (_playerType == PlayerType.Enemy)
            {
                GameController.Instance.Enemy = this;
                if (_bot == null)
                    _bot = FindObjectOfType<NormalBot>();
            }

            if (PhotonNetwork.IsMasterClient)
                GameController.Instance.GameStartedEvent += StartManaGeneration;
        }

        private void OnDisable()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                GameController.Instance.GameStartedEvent -= StartManaGeneration;
                StopCoroutine(_generateManaCoroutine);;
            }
        }

        public Tower GetTower(LineType lineType)
        {
            return _lines.FirstOrDefault(x => x.LineType == lineType);
        }

        public void ChangeHitPoints(int value)
        {
            _currentHealth = Mathf.Max(0, _currentHealth + value);
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
            HealthChangeEvent?.Invoke(_currentHealth, value);
            ManaPlus();
            _healthBar.SetHealth(_currentHealth);
        }

        public void ChangeMana(int value)
        {
            _currentMana = Mathf.Max(0, _currentMana + value);
            ManaChangeEvent?.Invoke(_currentMana);
        }

        private void ManaPlus()
        {
            if (_halfHP >= _currentHealth && !_fiftyPercent)
            {
                _fiftyPercent = true;
                _currentMana = _currentMana + _halfMana;
                ManaChangeEvent?.Invoke(_currentMana);
            }
            
            if (_lowHP >= _currentHealth && !_tenPercent)
            {
                _tenPercent = true;
                _currentMana = _currentMana + _lowMana;
                ManaChangeEvent?.Invoke(_currentMana);
            }
        }

        private void StartManaGeneration()
        {
            _generateManaCoroutine = StartCoroutine(GenerateMana());
        }

        private IEnumerator GenerateMana()
        {
            var gemImage = _playerPanel.GemImage;
            var flagsCount = GameController.Instance.FlagsController.GetFlagsCount(_playerType);
            var manaTicks = GameController.Instance.GameMachine.ManaTicks;
            _currentGeneratedMana = flagsCount;
            _manaTickTime = manaTicks[Mathf.Min(flagsCount, manaTicks.Length - 1)];

            float boost = 1f;
            if (IsBot)
            {
                boost = _bot.ManaPercentageBoost;
                Debug.Log($"Bot Mana Percentage Boost = {boost}\n");
            }
            while (_manaWaitTime < _manaTickTime)
            {
                _manaWaitTime += Time.deltaTime * (1 * boost);
                gemImage.fillAmount = Math.Max(gemImage.fillAmount, _manaWaitTime / _manaTickTime);
                yield return null;
            }
            
            _manaWaitTime = 0;
            gemImage.fillAmount = 0;
            _currentMana = Mathf.Min(_maxMana,_currentMana + 1);
            ManaChangeEvent?.Invoke(_currentMana);
            
            if (GameController.Instance.GameMachine.IsBattleActive)
                _generateManaCoroutine = StartCoroutine(GenerateMana());
        }

        private void RandomizeDecks()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;
            
            Collection = new List<UnitConfig>();
            Factions = new List<FactionConfig>();
            
            var randomDeck = Random.Range(0, _availableDecks.Length);
            _availableDecks[randomDeck].RandomizeFactions();

            for (var i = 0; i < _availableDecks[randomDeck].FactionsList.Count; i++)
            {
                var faction = _availableDecks[randomDeck].FactionsList[i];
                Instantiate(faction.TowerObject, _towerPositions[(int)faction.FactionType]);
                Factions.Add(faction);
                _lines[i].Faction = faction;
                foreach (var unit in PlayerFactionsController.Instance.GetFactionUnits(faction))
                {
                    Collection.Add(unit);
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting && PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(_currentHealth);
                stream.SendNext(_currentMana);
            }
            else if (stream.IsReading)
            {
                _currentHealth = (int) stream.ReceiveNext();
                _currentMana = (int) stream.ReceiveNext();
                HealthChangeEvent?.Invoke(_currentHealth, 0);
                _healthBar.SetHealth(_currentHealth); 
                ManaChangeEvent?.Invoke(_currentMana);
            }
        }

        public void SetCollection(List<UnitConfig> collectionList)
        {
            Collection = collectionList;
        }

        public void SetFactions(List<FactionConfig> factionList)
        {
            Factions = factionList;
            TowerVisual = new List<OutlineActivator>();
            for (var i = 0; i < Factions.Count; i++)
            {
                var faction = Factions[i];
                var tower = Instantiate(faction.TowerObject, _towerPositions[(int)faction.FactionType]);
                TowerVisual.Add(tower.GetComponent<OutlineActivator>());
                if (_playerType == PlayerType.Enemy)
                {
                    tower.GetComponent<TowerVisualChanger>().Swap();
                }
                
                _lines[i].Faction = faction;
            }

            if (IsBot)
            {
                var storedBotHealth = DynamicVarLibrary.Instance.GetVar("BotHealthValue");
                _maxHealth = storedBotHealth == string.Empty ? 3000 : int.Parse(storedBotHealth);
            }
            else
            {
                _maxHealth = PlayerFactionsController.Instance.GetTowerListHealth(Towers);
            }
            
            _currentHealth = _maxHealth;
            HealthChangeEvent?.Invoke(_currentHealth, 0);
            _healthBar.SetMaxHealth(_currentHealth);
            _halfHP = _maxHealth / 2;
            _lowHP = 1000;
        }
    }
}