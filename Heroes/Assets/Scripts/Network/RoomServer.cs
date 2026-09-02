using System;
using System.Collections.Generic;
using Configs;
using CoreConfigs.Configs;
using Enums;
using ExitGames.Client.Photon;
using Game.Controllers;
using Game.Pool;
using Game.Summons;
using Game.Units;
using Photon.Pun;
using Photon.Realtime;
using PhotonUtils;
using UI;
using UnityEngine;
using Utils;
using Utils.SaveManager;

namespace Network
{
    public class RoomServer : MonoPunSingleton<RoomServer>, IOnEventCallback
    {
        private const int MAX_PLAYERS = 2;
        private int _playersReady;
        private string _gameMode;

        public event Action SpawnEvent;
        public int PlayersReady
        {
            get => _playersReady;
            set => _playersReady = value;
        }
        public bool PvpGame
        {
            get
            {
                if (_gameMode == default)
                    _gameMode = SaveManager.GetValue(SavedDataManager.GameModeKey, SavedDataManager.GameModePvp);
                return _gameMode == SavedDataManager.GameModePvp;
            }
        }

        protected override void Init()
        {
            base.Init();
            DontDestroyOnLoad(this);
            PhotonNetwork.AddCallbackTarget(this);
        }
        
        public void OnEvent(EventData photonEvent)
        {
            var eventCode = (NetworkEvents) photonEvent.Code;
            var data = photonEvent.CustomData as object[];
            
            switch (eventCode)
            {
                case NetworkEvents.PlayerTwoJoined:
                    NetworkLobbyController.LoadGameScene(SavedDataManager.GameModePvp);
                    break;
                case NetworkEvents.PlayerReady:
                    Debug.LogWarningFormat("Player ready {0}", PhotonNetwork.IsMasterClient);
                    _playersReady++;
                    if (PlayersReady == MAX_PLAYERS)
                        GameController.Instance.GameStart();
                    break;
                case NetworkEvents.BotReady:
                    Debug.LogWarningFormat("Bot is ready");
                    _playersReady++;
                    break;
                case NetworkEvents.GameEnded:
                case NetworkEvents.PlayerDisconnected:
                    var firstBattleEnded = SaveManagerSafe.GetValue(SavedDataManager.FirstBattleEndedKey, false);
                    if (!firstBattleEnded)
                        SaveManagerSafe.Add(SavedDataManager.FirstBattleEndedKey, true);
                    
                    BackToLobby(!firstBattleEnded);
                    break;
                case NetworkEvents.InstantiateObject:
                    var uid = (string) data[3];
                    var config = (UnitConfig) ConfigLibrary.Instance.LoadConfig(uid);
                    var prefab = config.UnitPrefab;
                    var player = Instantiate(prefab, (Vector3) data[0], (Quaternion) data[1]);
                    player.Origin = prefab;
                    var vi = player.GetComponent<PhotonView>();
                    vi.ViewID = (int) data[2];
                    UnitPool.Instance.Push(player);
                    break;
                case NetworkEvents.Spawn:
                    var spawnUid = (string) data[0];
                    var playerType = (PlayerType) data[1];
                    var lineType = (LineType) data[2];
                    var unitConfig = (UnitConfig) ConfigLibrary.Instance.LoadConfig(spawnUid);
                    var health = (float) data[3];
                    var targetCellX = (int) data[4];
                    var isSummon = (bool) data[5];
                    
                    if (!unitConfig.IsInfinite)
                    {
                        if (!isSummon)
                        {
                            if (GameController.Instance.LimitController.GetValue(playerType, unitConfig) <= 0)
                                return;
                            GameController.Instance.LimitController.DecreaseValue(playerType, unitConfig);
                            SpawnEvent?.Invoke();
                        }
                    }

                    var unitPlayer = playerType == PlayerType.Player 
                        ? GameController.Instance.Player 
                        : GameController.Instance.Enemy;
                    var tower = unitPlayer.GetTower(lineType);
                    var newUnit = UnitPool.Instance.Pop(unitConfig.UnitPrefab, tower.transform, false);
                    if (targetCellX == -1)
                    {
                        newUnit.transform.localPosition = new Vector3(tower.Offset.x, tower.Offset.y, tower.Offset.z);
                    }
                    else
                    {
                        newUnit.transform.position = GameController.Instance.GameMachine
                            .Cells[targetCellX, (int)lineType].transform.position;
                    }

                    var newUnitTransform = newUnit.transform;
                    var newUnitLocalPosition = newUnitTransform.localPosition;
                    newUnitLocalPosition.y = tower.Offset.y;
                    newUnitTransform.localPosition = newUnitLocalPosition;
                    
                    newUnit.Init(unitConfig, playerType, lineType, targetCellX);
                    newUnit.Health = (int) (newUnit.Health * health);
                    newUnit.gameObject.SetActive(true);
                    newUnit.StartImmortalPhase();

                    if (playerType == PlayerType.Player || playerType == PlayerType.Enemy && !unitPlayer.Bot.FreeSummon)
                    {
                        var price = unitConfig.Cost - (tower.IsDiscountActive ? tower.ManaDiscount : 0);
                        if (tower.IsDiscountActive) tower.IsDiscountActive = false;
                        unitPlayer.ChangeMana(-(price > 0 ? price : 0));
                    }
                    
                    SpawnEvent?.Invoke();
                    break;
                case NetworkEvents.Despawn:
                    UnitPool.Instance.Push((int) data[0]);
                    break;
                case NetworkEvents.CollectionLoaded:
                    var playerCollection = (bool) data[0];
                    var collection = playerCollection ? GameController.Instance.Player : GameController.Instance.Enemy;
                    var collectionList = CreateCollection<UnitConfig>((string[])data[1]);
                    var factionList = CreateCollection<FactionConfig>((string[]) data[2]);
                    collection.SetCollection(collectionList);
                    collection.SetFactions(factionList);
                    break;
                case NetworkEvents.AbilityClick:
                    var line = (int) data[0];
                    var controller = GameController.Instance.Enemy;
                    controller.Towers[line].Cast(PlayerType.Enemy, controller);
                    SpawnEvent?.Invoke();
                    break;
                case NetworkEvents.Summon:
                    var obstacleSpawnUid = (string) data[0];
                    var obstacleConfig = (ObstacleConfig) ConfigLibrary.Instance.LoadConfig(obstacleSpawnUid);
                    var cellX = (int) data[1];
                    var obstaclePlayerType = (PlayerType) data[2];
                    var obstacleLine = (LineType) data[3];
                    var radius = (int) data[4];
                    var damage = (int) data[5];
                    var obstaclePlayer = obstaclePlayerType == PlayerType.Player 
                        ? GameController.Instance.Player 
                        : GameController.Instance.Enemy;
                    var obstacleTower = obstaclePlayer.GetTower(obstacleLine);
                    var newObstacle = SummonPool.Instance.Pop(obstacleConfig.ObstaclePrefab, obstacleTower.transform);
                    newObstacle.transform.parent = obstacleTower.transform;
                    var newObstacleLocalPosition = newObstacle.transform.localPosition;
                    newObstacleLocalPosition.y = obstacleTower.Offset.y;
                    newObstacle.transform.localPosition = newObstacleLocalPosition;
                    newObstacle.Create(obstaclePlayerType, cellX, (int) obstacleLine, radius, damage);                    
                    break;
                case NetworkEvents.DestroySummon:
                    SummonPool.Instance.Push((int) data[0]);
                    break;
                case NetworkEvents.InstantiateSummon:
                    var obstUid = (string) data[0];
                    var obstConfig = (ObstacleConfig) ConfigLibrary.Instance.LoadConfig(obstUid);
                    var obstPrefab = obstConfig.ObstaclePrefab;
                    var obstaclePrefab = Instantiate(obstPrefab, (Vector3) data[1], (Quaternion) data[2]);
                    obstaclePrefab.Origin = obstPrefab;
                    var obstacleVi = obstaclePrefab.GetComponent<PhotonView>();
                    obstacleVi.ViewID = (int) data[3];
                    SummonPool.Instance.Push(obstaclePrefab);
                    break;
            }
        }

        private List<T> CreateCollection<T>(string[] configs) where T : ConfigBase
        {
            var list = new List<T>();

            foreach (var configId in configs)
            {
                var config = (T) ConfigLibrary.Instance.LoadConfig(configId);
                if (config == null)
                    continue;
                
                list.Add(config);
            }

            return list;
        }

        private void BackToLobby(bool firstBattleEnded)
        {
            PhotonNetwork.Disconnect();
            _playersReady = 0;
            
            if (firstBattleEnded)
                DynamicDataManager.SetPreviousScene("Lobby");
            GoToScene.ToPreviousScene();
        }
    }
}
