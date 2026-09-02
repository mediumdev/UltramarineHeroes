using System;
using System.Linq;
using Enums;
using Game.Controllers;
using Game.Units;
using UnityEngine;

namespace Configs.Abilities
{
    public class InspiringAbilityConfig : AbilityConfig
    {
        [SerializeField] private AttackType _attackType;
        [SerializeField] private bool _buffAllFactions;
        [SerializeField] private EffectConfig[] _abilityEffects;

        public AttackType AttackType => _attackType;
        public bool BuffAllFactions => _buffAllFactions;

        public override void Cast(UnitController source)
        {
            foreach (var unit in UnitCollection.Instance.GlobalUnitCollection)
            {
                if (unit == source) continue;
                
                if (unit.CurrentLine != source.CurrentLine /*|| _attackType != AttackType.All && unit.UnitConfig.AttackType != _attackType*/)
                    continue;
                
                foreach (var effectConfig in _abilityEffects)
                {
                    if (unit.PlayerType == source.PlayerType && IsAnyAvailableEffect(effectConfig, PlayerType.Player))
                    {
                        unit.AddEffect(effectConfig, PlayerType.Player);
                    }
                    else if (unit.PlayerType != source.PlayerType && IsAnyAvailableEffect(effectConfig, PlayerType.Enemy))
                    {
                        unit.AddEffect(effectConfig, PlayerType.Enemy);
                    }
                }
            }
        }

        public override void CastDeath(UnitController source)
        {
            foreach (var unit in UnitCollection.Instance.GlobalUnitCollection)
            {
                if (unit == source) continue;
                
                if (unit.CurrentLine != source.CurrentLine /*|| _attackType != AttackType.All && unit.UnitConfig.AttackType != _attackType*/)
                    continue;
                
                foreach (var effectConfig in _abilityEffects)
                {
                    if (unit.PlayerType == source.PlayerType && IsAnyAvailableEffect(effectConfig, PlayerType.Player))
                    {
                        unit.RemoveEffect(effectConfig, PlayerType.Player);
                    }
                    
                    if (unit.PlayerType != source.PlayerType && IsAnyAvailableEffect(effectConfig, PlayerType.Enemy))
                    {
                        unit.RemoveEffect(effectConfig, PlayerType.Enemy);
                    }
                }
            }
        }

        private bool IsAnyAvailableEffect(EffectConfig effect, PlayerType playerType)
        {
            var available = effect.MaxHealth.Any(x => x.type == playerType)
                            || effect.Health.Any(x => x.type == playerType)
                            || effect.Speed.Any(x => x.type == playerType)
                            || effect.Damage.Any(x => x.type == playerType)
                            || effect.AttackSpeed.Any(x => x.type == playerType);
            
            return available;
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Abilities/Inspiring")]
        private static void Create()
        {
            CreateAsset<InspiringAbilityConfig>();
        }
#endif
    }
}