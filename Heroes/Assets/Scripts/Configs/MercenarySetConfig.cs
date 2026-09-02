using System;
using System.Collections.Generic;
using CoreConfigs.Configs;
using Structs;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public class Mercenary
    {
        public UnitConfig Config;
        public int SaleAmount;
        public CurrencyWithCount CurrencyCost;
    }
    
    public class MercenarySetConfig : ConfigBase
    {
        [SerializeField] private List<Mercenary> _units;
        
        public List<Mercenary> Mercenaries => _units;
        public int MercenariesCount => _units.Count;
        
        public List<Mercenary> GetMercenaries(List<int> ids) 
        {
            var units = new List<Mercenary>();
            foreach (var id in ids)
                units.Add(_units[id]);
            
            return units;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/MercenarySetConfig")]
        private static void Create()
        {
            CreateAsset<MercenarySetConfig>();
        }
#endif
    }
}