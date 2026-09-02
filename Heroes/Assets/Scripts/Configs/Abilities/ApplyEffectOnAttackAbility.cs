using System;
using Game.Units;
using UnityEditor;
using UnityEngine;

namespace Configs.Abilities
{
    [Serializable]
    public struct EffectConfigsStruct
    {
        public EffectConfig[] EffectConfigs;
        public int AfterAttackNumber;
    }

    public class ApplyEffectOnAttackAbility : AbilityConfig
    {
        [SerializeField] private EffectConfigsStruct[] _dotHotEffectConfigsStructs;

        public override void Cast(UnitController source)
        {
            foreach (var dotHotEffectConfigsStruct in _dotHotEffectConfigsStructs)
            {
                if (dotHotEffectConfigsStruct.AfterAttackNumber == 0)
                {
                    AddEffect(source, dotHotEffectConfigsStruct);
                }
                else if (source.AttackNumber % dotHotEffectConfigsStruct.AfterAttackNumber == 0
                         && source.AttackNumber != 0)
                {
                    AddEffect(source, dotHotEffectConfigsStruct);
                }
            }
        }

        private static void AddEffect(UnitController source, EffectConfigsStruct dotHotEffectConfigsStruct)
        {
            foreach (var dotHotEffectConfig in dotHotEffectConfigsStruct.EffectConfigs)
            {
                source.Target.AddEffect(dotHotEffectConfig);
            }
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Configs/Abilities/DotHotAbility")]
        private static void Create()
        {
            CreateAsset<ApplyEffectOnAttackAbility>();
        }
#endif
    }
}