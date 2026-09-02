using Enums;
using Game.Controllers;
using Game.Units;
using UnityEngine;

namespace Configs.Effects
{
    public class AfterDeathEffectConfig : EffectConfig
    {
        [SerializeField] private AbilityConfig _ability;
        [SerializeField] private bool _needTarget;
        public override void CastAfterDeath(UnitController source)
        {
            if (_needTarget)
            {
                _ability.Cast(source);
            }
            else
            {
                var unitPlayer = source.PlayerType == PlayerType.Player 
                    ? GameController.Instance.Player 
                    : GameController.Instance.Enemy;
                _ability.CastNoTarget(unitPlayer, source.CurrentLine);
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Effects/AfterDeathEffectConfig")]
        private static void Create()
        {
            CreateAsset<AfterDeathEffectConfig>();
        }
#endif
    }
}