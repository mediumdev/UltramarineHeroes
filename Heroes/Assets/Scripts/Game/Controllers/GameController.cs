using System;
using System.Collections;
using CoreUtils.Utils;
using Enums;
using Game.Battle;
using Photon.Pun;
using UnityEngine;

namespace Game.Controllers
{
    public class GameController : MonoSingleton<GameController>
    {
        private float _tick = 1f;

        public float Tick => _tick;
        public FlagsController FlagsController { get; set; }
        public PlayerController Player { get; set; }
        public PlayerController Enemy { get; set; }
        public LimitController LimitController { get; set; }
        public GameMachine GameMachine { get; set; }
        public int[] PlayerStartPlaceRandomizer { get; } = {1,1,1};
        public int[] EnemyStartPlaceRandomizer { get; } = {1,1,1};

        private bool _gameStarted;

        public event Action ControllerTickEvent;
        public event Action GameStartedEvent;
        public bool GameStarted => _gameStarted;

        public void GameStart()
        {
            Debug.LogWarning("Game started");
            _gameStarted = true;
            LimitController.OnStart();
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(GameTick());
            GameStartedEvent?.Invoke();
            GameMachine.IsBattleActive = true;
        }

        public void ContinueTick()
        {
            if (PhotonNetwork.IsMasterClient)
                StartCoroutine(GameTick());
            
            GameStartedEvent?.Invoke();
        }

        public void IncreasePlacePosition(PlayerType playerType, int line)
        {
            switch (playerType)
            {
                case PlayerType.Player:
                    if (PlayerStartPlaceRandomizer[line] < 2)
                        PlayerStartPlaceRandomizer[line]++;
                    else
                    {
                        PlayerStartPlaceRandomizer[line] = 0;
                    }
                    break;
                case PlayerType.Enemy:
                    if (EnemyStartPlaceRandomizer[line] < 2)
                        EnemyStartPlaceRandomizer[line]++;
                    else
                    {
                        EnemyStartPlaceRandomizer[line] = 0;
                    }
                    break;
            } 
        }

        private void Update()
        {
            if (!_gameStarted)
                return;
            
            GameMachine.OnUpdate();
        }

        private IEnumerator GameTick()
        {
            yield return new WaitForSeconds(_tick);
            ControllerTickEvent?.Invoke();
            if (GameMachine.IsBattleActive) 
                StartCoroutine(GameTick());
        }
    }
}