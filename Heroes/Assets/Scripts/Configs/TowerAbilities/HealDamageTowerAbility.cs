using System.Collections.Generic;
using Enums;
using Game.Units;
using UnityEditor;
using UnityEngine;

namespace Configs.TowerAbilities
{
    public class HealDamageTowerAbility : AbilityConfig
    {
        [SerializeField] private int _value;
        [SerializeField] private bool _isPercent;
        [SerializeField] private FxPlayer _fxOnTarget;
        [SerializeField] private Vector3 _fxOnTargetPosition = Vector3.zero;

        public override void Cast(UnitController source)
        {
            StartFxOnUnit(new List<Transform> {source.transform}, source.PlayerType == PlayerType.Player);

            if (_isPercent)
            {
                if (_value < 0)
                {
                    source.TakeDamage(source.MaxHealth * _value / 100);
                }
                else
                {
                    source.TakeHeal(source.MaxHealth * _value / 100);
                }
            }
            else
            {
                if (_value < 0)
                {
                    source.TakeDamage(_value);
                }
                else
                {
                    source.TakeHeal(_value);
                }
            }
        }

        private void StartFxOnUnit(IEnumerable<Transform> targets, bool rotateFx = false)
        {
            if (_fxOnTarget is null) return;

            foreach (var target in targets)
            {
                var position = _fxOnTargetPosition;
                var rotation = _fxOnTarget.transform.rotation.eulerAngles;
                if (rotateFx)
                {
                    rotation.y += 180;
                    position.x *= -1;
                }

                var fx = _fxOnTarget.Create(target, position, _fxScale, rotation);
                Debug.Log($"Cast FX {_fxOnTarget.gameObject.name} to transform {target.gameObject.name}. " +
                          $"rotateFx = {rotateFx}, position = {position.ToString("F3")}, rotation = {rotation.ToString("F3")}");
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Configs/TowerAbilities/HealDamageTowerAbility")]
        private static void Create()
        {
            CreateAsset<HealDamageTowerAbility>();
        }
#endif
    }
}