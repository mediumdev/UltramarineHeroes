using CoreConfigs.Configs;
using UnityEngine;

namespace Configs
{

    public class UnitDeckConfig : ConfigBase
    {
        [SerializeField] private UnitConfig[] _collection;

        public UnitConfig[] Collection => _collection;
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/UnitDeckConfig")]
        private static void Create()
        {
            CreateAsset<UnitDeckConfig>();
        }
#endif
    }
}
