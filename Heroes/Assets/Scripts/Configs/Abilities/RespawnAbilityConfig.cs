using Enums;
using Game.Controllers;
using Game.Pool;
using Game.Units;
using UnityEngine;

namespace Configs.Abilities
{
    public class RespawnAbilityConfig : AbilityConfig
    {
        [SerializeField] private LineType _respawnFrom;
        [SerializeField] private LineType _respawnTo;
        [SerializeField] private float _respawnHealthPart = 0.5f;
        [SerializeField] private bool _respawnOnTower;
        [SerializeField] private CastType _castType;
        [SerializeField] private bool _summonSelf = true;
        
        public override void Cast(UnitController source)
        {
            if (_castType != CastType.OnStart) return;
            Spawn(source);
        }
        public override void CastAfterDeath(UnitController source)
        {
            if (_castType != CastType.AfterDeath) return;
            Spawn(source);
        }

        private void Spawn(UnitController source)
        {
            var target = _respawnOnTower ? -1 : source.DeathPositionX;
            if (source.CurrentLine == _respawnFrom)
            {
                var gameMachine = GameController.Instance.GameMachine;
                var defaultCellX = source.PlayerType == PlayerType.Player ? 0 : 20;
                var currentCellX =  _respawnOnTower ? defaultCellX : source.DeathPositionX;
                
                var gridCell = gameMachine.Cells[currentCellX, (int) _respawnTo];
                var summon = _summonSelf ? source.UnitConfig : source.UnitConfig.UnitSummon;
                
                if (!gridCell.CanAddNewUnit(summon))
                {
                    Debug.LogWarning("Can't spawn summon");
                    return;
                }
                
                UnitPool.Instance.Spawn(summon, source.PlayerType, _respawnTo, _respawnHealthPart, target, true);
            }
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Abilities/Respawn")]
        private static void Create()
        {
            CreateAsset<RespawnAbilityConfig>();
        }
#endif
    }
}