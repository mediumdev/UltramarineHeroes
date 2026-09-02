using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Configs.Abilities;
using Enums;
using Game.Pool;
using Photon.Pun;
using UnityEngine;
using Utils;

namespace Game.Controllers
{
    public class LimitController : MonoBehaviour
    {
        private readonly Dictionary<string, int> _playerLimits = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _enemyLimits = new Dictionary<string, int>();
        private Dictionary<string, int> _tempDictionary;

        private void Awake()
        {
            GameController.Instance.LimitController = this;
        }

        public void OnStart()
        {
            if (SavedDataManager.GetFightMode() == SavedDataManager.FightModeDailyQuest)
            {
                foreach (var unit in GameController.Instance.Player.Collection)
                {
                    _playerLimits.Add(unit.Uid, unit.MaxCount);
                    if (PhotonNetwork.IsMasterClient)
                        Preload(unit, 1);
                }
            }
            else
            {
                foreach (var data in UnitLimitManager.Instance.PlayerUnitsCountDict)
                {
                    var unit = data.Key;
                    _playerLimits.Add(unit.Uid, Math.Min(unit.MaxCount, data.Value));
                    if (PhotonNetwork.IsMasterClient)
                        Preload(unit, 1);
                }
            }
            
            foreach (var unit in GameController.Instance.Enemy.Collection)
            {
                _enemyLimits.Add(unit.Uid, unit.MaxCount);
                if (PhotonNetwork.IsMasterClient)
                    Preload(unit,
                        GameController.Instance.Player.Collection.Any(playerUnit => unit.Uid == playerUnit.Uid)
                            ? 2
                            : 1);
            }
        }
        
        private void Preload(UnitConfig unit, int multiplier)
        {
            UnitPool.Instance.Preload(unit.UnitPrefab, unit.PreloadCount * multiplier);
            if (unit.UnitSummon != null)
                UnitPool.Instance.Preload(unit.UnitSummon.UnitPrefab, unit.UnitSummon.PreloadCount * multiplier);
            if (unit.Projectile != null) 
                ProjectilePool.Instance.Preload(unit.Projectile, unit.Projectile.PreloadCount * multiplier);
            foreach (var ability in unit.Abilities)
            {
                var summonAbility = ability as SummonAbilityConfig;
                if (summonAbility != null)
                { 
                    if (summonAbility.Obstacle != null) 
                        SummonPool.Instance.Preload(summonAbility.Obstacle.ObstacleConfig.ObstaclePrefab, summonAbility.Obstacle.PreloadCount * multiplier);
                }
            }
        }

        public int GetValue(PlayerType playerType, UnitConfig unitConfig)
        {
            var uid = unitConfig.Uid;
            _tempDictionary = playerType == PlayerType.Player ? _playerLimits : _enemyLimits;
            
            if (unitConfig.IsMercenary)
                return MercenariesController.Instance.UnitInStockCount(unitConfig);

            if (!_tempDictionary.ContainsKey(uid)) return 0;
            if (_tempDictionary[uid] <= 0) return 0;

            return _tempDictionary[uid];
        }
        
        public void DecreaseValue(PlayerType playerType, UnitConfig unitConfig)
        {
            var uid = unitConfig.Uid;
            _tempDictionary = playerType == PlayerType.Player ? _playerLimits : _enemyLimits;

            if (playerType == PlayerType.Player && SavedDataManager.GetFightMode() != SavedDataManager.FightModeDailyQuest)
                UnitLimitManager.Instance.SubtractUnit(unitConfig);
            
            if (unitConfig.IsMercenary) return;
            
            _tempDictionary[uid]--;
            if (_tempDictionary[uid] == 0) 
                CheckLimits(playerType == PlayerType.Player);
        }

        private void CheckLimits(bool isPlayer) // Проверка остатков по всем юнитам в запасе
        {
            if (isPlayer && GameController.Instance.GameMachine.PossiblePlayerArmyLost
                || !isPlayer && GameController.Instance.GameMachine.PossibleEnemyArmyLost) return;
            
            var endOfLimits = true;
            foreach (var limit in isPlayer ? _playerLimits : _enemyLimits)
            {
                // если текущий юнит используется в бою
                bool needToCheck = GameController.Instance.Player.Collection.Any(u => u.Uid == limit.Key);
                if (isPlayer && !needToCheck)
                    continue;
                
                if (limit.Value > 0)
                {
                    endOfLimits = false;
                    break;
                }
            }
  
            if (endOfLimits)
            {
                if (isPlayer)
                    GameController.Instance.GameMachine.PossiblePlayerArmyLost = true;
                else
                    GameController.Instance.GameMachine.PossibleEnemyArmyLost = true;
            }
        }
    }
}