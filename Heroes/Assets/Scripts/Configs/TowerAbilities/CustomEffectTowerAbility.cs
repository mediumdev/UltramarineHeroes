using System.Linq;
using Game.Units;
using UnityEngine;

namespace Configs.TowerAbilities
{
    public class CustomEffectTowerAbility : AbilityConfig
    {
        [SerializeField] private EffectConfig[] _abilityEffects;

        public override void Cast(UnitController source)
        {
            foreach (var effectConfig in _abilityEffects)
            {
                if (!source.GetEffects(effectConfig).Any())
                {
                    source.AddEffect(effectConfig);
                }
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/TowerAbilities/CustomEffectTowerAbility")]
        private static void Create()
        {
            CreateAsset<CustomEffectTowerAbility>();
        }
#endif
    }
}
