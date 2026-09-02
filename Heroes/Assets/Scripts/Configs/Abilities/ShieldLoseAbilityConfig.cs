using System.Linq;
using Configs.Effects;
using Game.Units;
using UnityEngine;

namespace Configs.Abilities
{
    public class ShieldLoseAbilityConfig : AbilityConfig
    {
        [Header("Shield Lose is `Damage` ability")]
        [SerializeField] private ShieldEffectConfig _shieldEffect;
        [SerializeField] private EffectConfig[] _loseShieldEffects;

        public override void Cast(UnitController source)
        {
            if (source.GetEffects(_shieldEffect).Any()) return;
            
            foreach (var effect in _loseShieldEffects)
                effect.Cast(source);
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Abilities/Shield Lose")]
        private static void Create()
        {
            CreateAsset<ShieldLoseAbilityConfig>();
        }
#endif
    }
}