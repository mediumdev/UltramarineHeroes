using System.Linq;
using Configs.Effects;
using Game.Units;
using UnityEngine;

namespace Configs.TowerAbilities
{
    public class RestoreShieldTowerAbility : AbilityConfig
    {
        [SerializeField] private int _value;
        [SerializeField] private bool _isPercent;
        [SerializeField] private bool _fullRecharge;

        public override void Cast(UnitController source)
        {
            foreach (var effect in source.GetEffects(typeof(ShieldEffectConfig)).ToArray())
            {
                if (effect.Value >= 0)
                {
                    if (_fullRecharge)
                    {
                        effect.Value = effect.MaxValue;
                    }
                    else
                    {
                        var value = _isPercent ? effect.MaxValue * _value / 100 : _value;
                        effect.Value += value;
                    }
                    source.SetShieldDamage((int) effect.Value);
                }
            }
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/TowerAbilities/RestoreShieldTowerAbility")]
        private static void Create()
        {
            CreateAsset<RestoreShieldTowerAbility>();
        }
#endif
    }
}
