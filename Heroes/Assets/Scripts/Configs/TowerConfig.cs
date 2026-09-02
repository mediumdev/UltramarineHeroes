using CoreConfigs.Configs;
using Enums;
using UnityEngine;

namespace Configs
{
    public class TowerConfig : ConfigBase
    {
        [SerializeField] private LineType _lineType;
        [SerializeField] private int _resourcePerTick;

        public int ResourcePerTick => _resourcePerTick;
        public LineType LineType => _lineType;
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/TowerConfig")]
        private static void Create()
        {
            CreateAsset<TowerConfig>();
        }
#endif
    }
}