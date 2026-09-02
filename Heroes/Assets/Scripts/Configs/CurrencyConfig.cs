using Enums;
using UnityEditor;
using UnityEngine;

namespace Configs
{
    public class CurrencyConfig : BaseItemConfig
    {
        [SerializeField] public long maxValue = long.MaxValue;
        [SerializeField] public CurrencyType currencyType;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Configs/Currency Config")]
        private static void Create()
        {
            CreateAsset<CurrencyConfig>();
        }
#endif
    }
}