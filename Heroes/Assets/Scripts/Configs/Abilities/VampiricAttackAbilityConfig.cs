using Game.Units;
using UnityEngine;

namespace Configs.Abilities
{
    public class VampiricAttackAbilityConfig : AbilityConfig
    {
        [SerializeField] private float _healPercent;
        
        public override void Cast(UnitController unit)
        {
            StartFx(unit.transform);
            unit.TakeHeal((int) (unit.Damage * _healPercent / 100));
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Abilities/Vampiric Attack")]
        private static void Create()
        {
            CreateAsset<VampiricAttackAbilityConfig>();
        }
#endif
    }
}