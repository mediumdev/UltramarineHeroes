using CoreConfigs.Configs;
using UnityEngine;

namespace Configs
{
    public class FlagConfig : ConfigBase
    {
        [SerializeField] private float _resourcePerTick;
        
        public float ResourcePerTick => _resourcePerTick;
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/FlagConfig")]
        private static void Create()
        {
            CreateAsset<FlagConfig>();
        }
#endif
    }
}