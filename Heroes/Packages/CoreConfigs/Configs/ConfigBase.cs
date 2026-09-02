using UnityEngine;

namespace CoreConfigs.Configs
{
    public class ConfigBase : ScriptableObject
    {
        [SerializeField] private string _uid;
        public string Uid
        {
            get { return _uid; }
        }

        public virtual void Init()
        {
        }

        public static T LoadConfig<T>(string uid) where T : ConfigBase
        {
            var result = ConfigLibrary.Instance.LoadConfig(uid) as T;
            if (result != null)
                result.Init(); 
            return result;
        }

        public static T LoadFirstAvailableConfig<T>() where T : ConfigBase
        {
            var result = ConfigLibrary.Instance.LoadFirstAvailable<T>() as T;
            if (result != null)
                result.Init();
            return result;
        }

        public static T[] LoadAll<T>() where T : ConfigBase
        {
            var result = ConfigLibrary.Instance.LoadAll<T>();
            foreach(var config in result)
                config.Init();
            return result;
        }

#if UNITY_EDITOR
        public void GenerateUid(string guid)
        {
            _uid = guid;
        }

        protected static void CreateAsset<T>() where T : ConfigBase
        {
            ConfigUtils.CreateAsset<T>();
        }
#endif
    }
}