using Enums;
using UnityEngine;

namespace Configs.Abilities
{
    public class DamageToTagAbilityConfig : AbilityConfig
    {
        [SerializeField] private UnitTag[] _targets;
        [SerializeField] private float _damageModifier = 0.5f;

        public UnitTag[] Targets => _targets;
        public float DamageModifier => _damageModifier;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Abilities/Damage To Tag")]
        private static void Create()
        {
            CreateAsset<DamageToTagAbilityConfig>();
        }
#endif
    }
}