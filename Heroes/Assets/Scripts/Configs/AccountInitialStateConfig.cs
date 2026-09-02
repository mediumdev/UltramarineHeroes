using System.Collections.Generic;
using CoreConfigs.Configs;
using Structs;
using UnityEditor;
using UnityEngine;

namespace Configs
{
    public class AccountInitialStateConfig : ConfigBase
    {
        [SerializeField] private List<CurrencyWithCount> _currencies;
        [SerializeField] private List<UnitWithCount> _unitLimits;

        public List<CurrencyWithCount> Currencies => _currencies;
        public List<UnitWithCount> UnitLimits => _unitLimits;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Configs/Account Initial State")]
        private static void Create()
        {
            CreateAsset<AccountInitialStateConfig>();
        }
#endif
    }
}