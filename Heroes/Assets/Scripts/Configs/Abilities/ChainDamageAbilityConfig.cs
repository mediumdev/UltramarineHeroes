using Game.Controllers;
using Game.Units;
using UnityEngine;

namespace Configs.Abilities
{
    public class ChainDamageAbilityConfig : AbilityConfig
    {
        [Header("Chain Damage")]
        [SerializeField] private float _changeDamagePercent = -30f;
        [SerializeField] private int _jumpRange = 2;
        [SerializeField] private int _maxTargets = 3;
        [SerializeField] private Vector3 _fxOffsetFromCaster;
        [SerializeField] private Vector3 _fxOffsetFromTarget;

        private UnitController SetDamage(float damage, UnitController unitFrom, UnitController unitTo, Vector3 offset)
        {
            var unitFromTransform = unitFrom.transform;
            var unitToTransform = unitTo.transform;
            var distance = (unitFromTransform.position + offset - unitToTransform.position).magnitude;
            var scale = new Vector3(1, 1, distance / _fx.Length);
            var fx = _fx.Create(unitFromTransform.parent, offset + unitFromTransform.localPosition, scale, Vector3.zero);
            fx.transform.LookAt(unitToTransform);
            unitTo.TakeDamage((int) damage);
            return unitTo;
        }

        public override void Cast(UnitController source)
        {
            var targets = GameController.Instance.GameMachine.Search(source.Target, _jumpRange, true);
            var damage = (float) source.Damage;
            var prevFrom = SetDamage(damage, source, source.Target, _fxOffsetFromCaster);
            var count = 1;
            foreach (var target in targets)
            {
                if (target == source.Target) continue;
                if (count >= _maxTargets) break;
                
                damage *= (100f + _changeDamagePercent) / 100f;
                prevFrom = SetDamage(damage, prevFrom, target, _fxOffsetFromTarget);
                count++;
            }
        }
                
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Abilities/Chain Damage")]
        private static void Create()
        {
            CreateAsset<ChainDamageAbilityConfig>();
        }
#endif
    }
}