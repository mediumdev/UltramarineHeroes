using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Enums;
using Game.Controllers;
using Game.Pool;
using Network;
using PhotonUtils;
using UnityEngine;
using Utils;
using Utils.SaveManager;
using Random = UnityEngine.Random;

namespace Game.Ai
{
    public enum BotLevel
    {
        Simple,
        Normal
    }
    
    public class SimpleBot : MonoBehaviour
    {
        [SerializeField] private NetworkGameController _networkGameController;
        [SerializeField] protected float _minSpawnTime;
        [SerializeField] protected float _maxSpawnTime;
        [SerializeField] private BotLevel _level;

        protected Coroutine _botRoutine;
        private bool _isEnabled;

        private void Start()
        {
            if (RoomServer.Instance.PvpGame || _networkGameController is null || !SaveManager.HasKey(SavedDataManager.BotLevelKey)) return;

            var botLevel = (BotLevel)SaveManager.GetValue(SavedDataManager.BotLevelKey, 0);
            if (botLevel != _level)
                return;

            _isEnabled = true;
            
            var collectionString = SaveManager.GetValue(SavedDataManager.BotCollectionKey, string.Empty);
            var factionString = SaveManager.GetValue(SavedDataManager.BotFactionsKey, string.Empty);
            
            object[] data = { false, collectionString.Split(';'), factionString.Split(';') };
            PhotonSingleton.Instance.RaiseEvent((byte) NetworkEvents.CollectionLoaded, data);
            PhotonSingleton.Instance.RaiseEvent((byte) NetworkEvents.BotReady, null);
        }

        private void OnEnable()
        {
            GameController.Instance.GameStartedEvent += StartBot;
        }
        
        private void OnDisable()
        {
            GameController.Instance.GameStartedEvent -= StartBot;
        }

        private void StartBot()
        {
            if (GameController.Instance.GameMachine.IsPaused)
                return;

            if (_isEnabled && _botRoutine == default)
                _botRoutine = StartCoroutine(BotRoutine());
        }

        protected virtual IEnumerator BotRoutine()
        {
            yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));

            if (!GameController.Instance.GameMachine.IsPaused)
            {
                var randomLine = (LineType) Random.Range(0, 3);
                var unitList = GetLineUnits(randomLine);
                SpawnRandom(unitList, randomLine);
                if (GameController.Instance.GameMachine.IsBattleActive)
                    _botRoutine = StartCoroutine(BotRoutine());
            }
            else
                _botRoutine = StartCoroutine(BotRoutine());
        }

        protected List<UnitConfig> GetLineUnits(LineType line)
        {
            var list = new List<UnitConfig>();
            
            foreach (var unit in GameController.Instance.Enemy.Collection)
            {
                if (GameController.Instance.LimitController.GetValue(PlayerType.Enemy, unit) > 0 || unit.IsInfinite)
                    if (unit.SetupProperties.Any(x => x.SetupType == line))
                    {
                        list.Add(unit);
                    }
            }

            return list;
        }

        protected void SpawnRandom(List<UnitConfig> list, LineType line)
        {
            if (list == null || list.Count < 1) return;

            var costList = list.Where(x => x.Cost <= GameController.Instance.Enemy.Mana).ToList();
            if (costList.Count < 1) return;
            
            var unitConfig = costList[Random.Range(0, costList.Count)];
            var gameMachine = GameController.Instance.GameMachine;
            var defaultCellX = 20;
            var gridCell = gameMachine.Cells[defaultCellX, (int) line];
            if (!gridCell.CanAddNewUnit(unitConfig))
                return;

            UnitPool.Instance.Spawn(unitConfig, PlayerType.Enemy, line);
        }
    }
}