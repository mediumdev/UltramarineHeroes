using Game.Controllers;
using Game.Units;
using UnityEngine;

namespace Configs.Abilities
{
    public class AreaDamageAbilityConfig : AbilityConfig
    {
        [SerializeField] private float _cleavePercent = 100f;
        [SerializeField] private int _cleaveRadius;

        public override void Cast(UnitController unit)
        {
            var damage = (int) (unit.Damage * (_cleavePercent / 100f));
            var targets = GameController.Instance.GameMachine.Search(unit.Target, _cleaveRadius, true);
            StartFx(unit.Target.transform);
            foreach (var target in targets)
            {
                if (target == unit.Target)
                    continue;
                
                target.TakeDamage(damage);
            }
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Abilities/Area Damage")]
        private static void Create()
        {
            CreateAsset<AreaDamageAbilityConfig>();
        }
#endif
    }
}