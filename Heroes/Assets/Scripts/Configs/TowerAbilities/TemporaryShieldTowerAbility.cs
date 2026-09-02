using System.Linq;
using Configs.Effects;
using Enums;
using Game.Controllers;
using Game.Units;
using UnityEngine;

namespace Configs.TowerAbilities
{
    public class TemporaryShieldTowerAbility : AbilityConfig
    {
        [SerializeField] private EffectConfig _shieldEffect;
        [SerializeField] private int _shieldOffDistance;

        public override void Cast(UnitController source)
        {
            if (source.PlayerType == PlayerType.Player)
            {
                if (source.CurrentCellX <= _shieldOffDistance)
                {
                    AttachShield(source);
                }
                else
                {
                    DetachShield(source);
                }
            }
            else if (source.PlayerType == PlayerType.Enemy)
            {
                if (source.CurrentCellX > GameController.Instance.GameMachine.HorizontalCellCount - _shieldOffDistance)
                {
                    AttachShield(source);
                }
                else
                {
                    DetachShield(source);
                }
            }
        }

        private void AttachShield(UnitController source)
        {
            if (source.HaveTemporaryShield) return;
            if (source.GetEffects(_shieldEffect).Any()) return;
            
            source.AddEffect(_shieldEffect);
            source.HaveTemporaryShield = true;
            source.SetShieldDamage((int) _shieldEffect.Value);
        }
        
        private void DetachShield(UnitController source)
        {
            foreach (var effect in source.GetEffects(typeof(ShieldEffectConfig)).ToArray())
            {
                if (effect.Value > 0)
                {
                    source.SetShieldDamage(0);
                    effect.Value = 0;
                    source.HaveTemporaryShield = false;
                }
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/TowerAbilities/TemporaryShield")]
        private static void Create()
        {
            CreateAsset<TemporaryShieldTowerAbility>();
        }
#endif
    }
}