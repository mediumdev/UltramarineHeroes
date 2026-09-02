using System;
using Configs;
using Enums;
using Game.Units;

namespace Game.Effects
{
    [Serializable]
    public class ActiveEffect
    {
        public EffectConfig Config { get; private set; }
        public float Duration { get; set; }
        public DateTime LastTick { get; set; }
        public float Value { get; set; }
        public float MaxValue { get; set; }

        public ActiveEffect(EffectConfig config)
        {
            Config = config;
            Duration = Config.Duration;
            Value = Config.Value;
            MaxValue = Value;
        }

        public bool IsEnded()
        {
            return Config.IsEnded(Duration, Value);
        }

        public void Cast(UnitController unit)
        {
            if (Config.TickDurationSeconds <= 0)
                return;

            if (LastTick == default)
            {
                LastTick = DateTime.Now;
                return;
            }
            
            if ((DateTime.Now - LastTick).TotalSeconds < Config.TickDurationSeconds)
                return;
            
            LastTick = DateTime.Now;
            Config.Cast(unit);
        }

        public void Enable(UnitController unit, PlayerType filter)
        {
            Config.Enable(unit, filter);
        }
        
        public void Disable(UnitController unit, PlayerType filter)
        {
            Config.Disable(unit, filter);
        }

        public void CastAfterDeath(UnitController unit)
        {
            Config.CastAfterDeath(unit);
        }
    }
}