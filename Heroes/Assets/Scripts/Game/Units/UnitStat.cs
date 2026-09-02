using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Enums;
using Game.Effects;

namespace Game.Units
{
    public class UnitStat
    {
        private float _health = float.MinValue;
        private float _speed = float.MinValue;
        private float _damage = float.MinValue;
        private List<ActiveEffect> _effects = new List<ActiveEffect>();
        
        private UnitConfig Config { get; }
        public PlayerType PlayerType { get; }

        public float MaxSpeed => Config.MoveSpeed;
        public float MaxDamage => Config.Damage;
        public float MaxHealth => Config.HitPoints;
        public List<ActiveEffect> Effects => _effects;

        public float Speed
        {
            get => Math.Abs(_speed - float.MinValue) < 0.1 ? _speed = MaxSpeed : _speed;
            set => _speed = value;
        }
        public float Damage
        {
            get => Math.Abs(_damage - float.MinValue) < 0.1 ? _damage = MaxDamage : _damage;
            set => _damage = value;
        }
        public float Health
        {
            get => Math.Abs(_health - float.MinValue) < 0.1 ? _health = MaxHealth : _health;
            set => _health = value;
        }

        public UnitStat(UnitConfig config, PlayerType playerType)
        {
            Config = config;
            PlayerType = playerType;
        }

        public void AddEffect(EffectConfig effectConfig)
        {
            RemoveEffect(effectConfig);
            _effects.Add(new ActiveEffect(effectConfig));
        }

        public void UpdateEffect(Type type, float value)
        {
            var effects = GetEffects(type).ToList();
            if (effects.Count == 0) return;
            
            var effect = effects[0];
            effect.Value += value;
            if (effect.IsEnded() && effect.Value <= 0)
                RemoveEffect(effect.Config);
        }
        
        public void RemoveEffect(EffectConfig effectConfig)
        {
            _effects.RemoveAll(x => x.Config == effectConfig);
        }
        
        public IEnumerable<ActiveEffect> GetEffects(Type type)
        {
            return _effects.Where(x => x.Config.GetType() == type);
        }
    }
}