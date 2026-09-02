using System;
using CoreConfigs.Configs;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public struct FactionUnitsCollection
    {
        public FactionConfig faction;
        public UnitConfig[] units;
    }
    
    public class DeckConfig : ConfigBase
    {
        [SerializeField] private FactionUnitsCollection[] units;

        public FactionUnitsCollection[] Units => units;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Deck Config")]
        private static void Create()
        {
            CreateAsset<DeckConfig>();
        }
#endif
    }
}