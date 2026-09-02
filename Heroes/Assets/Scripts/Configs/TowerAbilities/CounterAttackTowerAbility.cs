using Configs.Effects;
using Game.Units;
using UnityEngine;

namespace Configs.TowerAbilities
{
    public class CounterAttackTowerAbility : AbilityConfig
    {
        [SerializeField] private SpikeEffectConfig _spikeEffect;
        public override void Cast(UnitController source)
        {
            if (_spikeEffect is null) return;
            
            source.AddEffect(_spikeEffect);
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/TowerAbilities/CounterAttack")]
        private static void Create()
        {
            CreateAsset<CounterAttackTowerAbility>();
        }
#endif
    }
}
