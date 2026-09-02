using Configs.Effects;
using Game.Units;
using UnityEngine;

namespace Configs.Abilities
{
    public class ShieldAbilityConfig : AbilityConfig
    {
        [Header("Shield is `Spawn` ability")] 
        [SerializeField] private ShieldEffectConfig _shieldEffect;

        public override void Cast(UnitController source)
        {
            if (_shieldEffect is null) return;
            
            source.AddEffect(_shieldEffect);
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Abilities/Shield")]
        private static void Create()
        {
            CreateAsset<ShieldAbilityConfig>();
        }
#endif
    }
}