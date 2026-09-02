using System;
using System.Collections.Generic;
using CoreConfigs.Configs;
using UnityEngine;

namespace Dynamic
{

    [Serializable]
    public struct DynamicVar
    {
        public string Name;
        public object Value;
    }
    
    public class DynamicVarConfig : ConfigBase
    {
        [SerializeField] private List<DynamicVar> _dynamicVars;

        public List<DynamicVar> DynamicVars => _dynamicVars;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/DynamicVarLibrary")]
        private static void Create()
        {
            CreateAsset<DynamicVarConfig>();
        }
#endif
    }
}
