using System;
using Game.Units;
using UnityEngine;

namespace Configs.Abilities
{
    public class CavalryAttackAbilityConfig : AbilityConfig
    {
        [SerializeField] private float _attackBonusPercent = 50f;
        [SerializeField] private int _maxSpeed;
        [SerializeField] private CleaveAttackAbilityConfig _cleaveAbility;

        public override void Cast(UnitController source)
        {
            var damageBonus = _attackBonusPercent * source.Speed / _maxSpeed / 100;
            source.Damage = (int) (source.MaxDamage * Math.Max(1, damageBonus));
            
            if (_cleaveAbility is null) return;
            
            _cleaveAbility.Cast(source);
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Abilities/Cavalry Attack")]
        private static void Create()
        {
            CreateAsset<CavalryAttackAbilityConfig>();
        }
#endif
    }
}