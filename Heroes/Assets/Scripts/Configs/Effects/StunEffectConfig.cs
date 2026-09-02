using Enums;
using Game.Units;
using UnityEngine;

namespace Configs.Effects
{
    public class StunEffectConfig : EffectConfig
    {
        public override void Enable(UnitController unit, PlayerType filter = PlayerType.None, int modifier = 1)
        {
            unit.IsStunned = modifier > 0;
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Effects/Stun")]
        private static void Create()
        {
            CreateAsset<StunEffectConfig>();
        }
#endif
    }
}
