using System.Collections.Generic;
using CoreConfigs.Configs;
using CoreUtils.Utils;

namespace Dynamic
{
    public class DynamicVarLibrary : MonoSingleton<DynamicVarLibrary>
    {
        private readonly Dictionary<string, DynamicVar> _values = new Dictionary<string, DynamicVar>();

        protected override void Init()
        {
            base.Init();
            ParseDefault();

            DontDestroyOnLoad(gameObject);
        }

        private void ParseDefault()
        {
            var config = ConfigBase.LoadFirstAvailableConfig<DynamicVarConfig>();
            foreach (var dynamicVar in config.DynamicVars)
            {
                _values.Add(dynamicVar.Name, dynamicVar);
            }
        }

        public void AddVar(string varName, DynamicVar value)
        {
            _values[varName] = value;
        }

        public void AddVar(string varName, object value)
        {
            AddVar(varName, new DynamicVar { Name = varName, Value = value });
        }

        public string GetVar(string varName)
        {
            return _values.ContainsKey(varName) ? _values[varName].Value.ToString() : string.Empty;
        }
    }
}