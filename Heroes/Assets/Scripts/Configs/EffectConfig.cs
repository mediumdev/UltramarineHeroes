using System.Collections.Generic;
using System.Linq;
using CoreConfigs.Configs;
using Enums;
using Game.Units;
using Structs;
using UnityEngine;

namespace Configs
{
    public class EffectConfig : ConfigBase
    {
        [SerializeField] protected float _duration;
        [SerializeField] protected float _tickDurationSeconds;
        [SerializeField] protected bool _endlessDuration;
        [SerializeField] protected float _value;
        
        [Header("Базовые настройки влияния на параметры юнита")]
        [SerializeField] protected List<ValueWithType> _maxHealth;
        [SerializeField] protected List<ValueWithType> _health;
        [SerializeField] protected List<ValueWithType> _speed;
        [SerializeField] protected List<ValueWithType> _damage;
        [SerializeField] protected List<ValueWithType> _attackSpeed;

        public float Duration => _duration;
        public float TickDurationSeconds => _tickDurationSeconds;
        public bool EndlessDuration => _endlessDuration;
        public float Value => _value;
        public List<ValueWithType> MaxHealth => _maxHealth;
        public List<ValueWithType> Health => _health;
        public List<ValueWithType> Speed => _speed;
        public List<ValueWithType> Damage => _damage;
        public List<ValueWithType> AttackSpeed => _attackSpeed;

        public virtual bool IsEnded(float duration, float value)
        {
            return !_endlessDuration && duration <= 0 || value <= 0;
        }

        public virtual void Cast(UnitController unit)
        {
            Enable(unit);
        }
        
        public virtual void CastAfterDeath(UnitController unit)
        {
            
        }
        
        // Действие эффекта включается и выключается
        // А не действует разово как у способности

        public virtual void Enable(UnitController unit, PlayerType typeFilter = PlayerType.None, int modifier = 1)
        {
            foreach (var data in _maxHealth.Where(x => x.type == typeFilter))
            {
                var delta = data.isPercentValue ? unit.MaxHealth * data.value / 100 : data.value;
                unit.MaxHealth += (int) delta * modifier;
            }
            
            foreach (var data in _health.Where(x => x.type == typeFilter))
            {
                if (modifier != -1)
                {
                    var delta = data.isPercentValue ? unit.Health * data.value / 100 : data.value;
                    unit.Health += (int)delta * modifier;
                }
            }

            foreach (var data in _speed.Where(x => x.type == typeFilter))
            {
                var delta = data.isPercentValue ? unit.MaxSpeed * data.value / 100 : data.value;
                unit.Speed += (int) delta * modifier;
            }
            
            foreach (var data in _damage.Where(x => x.type == typeFilter))
            {
                var delta = data.isPercentValue ? unit.MaxDamage * data.value / 100 : data.value;
                unit.Damage += (int) delta * modifier;
            }
            
            foreach (var data in _attackSpeed.Where(x => x.type == typeFilter))
            {
                // т.к. дефолтный множитель = 1
                var delta = data.isPercentValue ? /*unit.AttackSpeedMultiplier **/ data.value / 100 : data.value; 
                unit.AttackSpeedMultiplier += (int) delta * modifier;
            }
        }

        public virtual void Disable(UnitController unit, PlayerType filter)
        {
            Enable(unit, filter, -1);
        }
        
// #if UNITY_EDITOR
//         [UnityEditor.MenuItem("Assets/Create/Configs/Effect Config")]
//         private static void Create()
//         {
//             CreateAsset<EffectConfig>();
//         }
// #endif
    }
}