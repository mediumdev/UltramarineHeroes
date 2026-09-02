using DG.Tweening;
using Enums;
using Game.Controllers;
using Game.Pool;
using Game.Summons;
using Game.Units;
using UnityEngine;

namespace Configs.Abilities
{
    public class SummonAbilityConfig : AbilityConfig
    {
        [SerializeField] private Obstacle _obstacle;
        [SerializeField] private bool _onlySourceLine = true;
        [SerializeField] private LineType[] _lines;
        [Range(1, 10)] [SerializeField] private int _radius = 1;
        [SerializeField] private int _damage;
        [SerializeField] private CastType _castType;
        
        public Obstacle Obstacle => _obstacle;

        public override void Cast(UnitController source)
        {
            if (_castType == CastType.OnStart) 
                Summon(source);
            
            if (_castType == CastType.Periodic)
            {
                var seq = DOTween.Sequence();
                seq.AppendCallback(() =>
                {
                    if (!source.IsMoving || (source.PlayerType == PlayerType.Player && source.CurrentCellX == 0) ||
                        (source.PlayerType == PlayerType.Enemy && source.CurrentCellX ==
                            GameController.Instance.GameMachine.HorizontalCellCount - 1))
                    {
                        seq.Restart();
                    }
                });
                seq.AppendInterval(_intervalDuration);
                seq.AppendCallback(() => Summon(source));
                seq.SetLoops(_intervalCount, LoopType.Restart);
            }
        }
        
        public override void CastDeath(UnitController source)
        {
            if (_castType == CastType.OnDeath) Summon(source);
        }
        
        public override void CastAfterDeath(UnitController source)
        {
            if (_castType == CastType.AfterDeath) Summon(source);
        }

        private void Summon(UnitController source)
        {
            if (_onlySourceLine)
            {
                SummonPool.Instance.Spawn(_obstacle.ObstacleConfig, source.CurrentCellX, source.PlayerType, source.CurrentLine, _radius, _damage);
            }
            else
            {
                for (var i = 0; i < 3; i++)
                {
                    foreach (var line in _lines)
                    {
                        if (i == (int) line)
                        {
                            SummonPool.Instance.Spawn(_obstacle.ObstacleConfig, source.CurrentCellX, source.PlayerType, line, _radius, _damage);
                        }
                    }
                }
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Abilities/SummonAbility")]
        private static void Create()
        {
            CreateAsset<SummonAbilityConfig>();
        }
#endif
    }
}