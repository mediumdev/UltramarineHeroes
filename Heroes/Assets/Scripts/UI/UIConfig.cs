using CoreConfigs.Configs;
using UnityEngine;

namespace UI
{
    public class UIConfig : ConfigBase
    {
        private static UIConfig _instance;
        public static UIConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (UIConfig) ConfigLibrary.Instance.LoadFirstAvailable<UIConfig>();
                }

                return _instance;
            }
        }
        
        [SerializeField] private UnitInfo _unitInfo;
        [SerializeField] private UnitInfo _enemyUnitInfo;

        public UnitInfo EnemyUnitInfo => _enemyUnitInfo;

        public UnitInfo UnitInfo => _unitInfo;
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/UI/Main")]
        private static void Create()
        {
            CreateAsset<UIConfig>();
        }
#endif
    }
}
