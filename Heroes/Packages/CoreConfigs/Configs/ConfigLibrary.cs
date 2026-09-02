using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CoreConfigs.Configs
{
    public class ConfigLibrary : ScriptableObject
    {

        [Serializable]
        public class LibraryItem
        {
            public string Uid;
            public ConfigBase Config;
        }

        [SerializeField] private List<LibraryItem> _library = new List<LibraryItem>();

        private static bool _init;
        private static ConfigLibrary _instance;

        public static bool Loaded => _init;

        public static ConfigLibrary Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("Load library at entry point of your game");
                }

                return _instance;
            }
        }

        public static void LoadLibrary(ConfigLibrary instance)
        {
            if (_init)
            {
                Debug.LogError("Trying set new Library, aborted");
                return;
            }

            _init = true;
            _instance = instance;
        }

        public bool Contains(string uid)
        {
            return _library.Any(x => x.Uid.Equals(uid));
        }

        public ConfigBase LoadConfig(string uid)
        {
            return _library.FirstOrDefault(x => x.Uid.Equals(uid))?.Config;
        }

        public ConfigBase LoadFirstAvailable<T>()
        {
            return _library.FirstOrDefault(x => x.Config is T).Config;
        }

        public T[] LoadAll<T>() where T : ConfigBase
        {
            return _library.FindAll(x => x.Config is T).Select(x => x.Config as T).ToArray();
        }

        public void AddConfig(string uid, ConfigBase configBase)
        {
            _library.Add(new LibraryItem {Uid = uid, Config = configBase});
        }

#if UNITY_EDITOR
    public static ConfigLibrary EditorInstance
    {
        get
        {
            var config = AssetDatabase.FindAssets("t:ConfigLibrary").FirstOrDefault();
            var configInstance = config != null
                ? AssetDatabase.LoadAssetAtPath<ConfigLibrary>(AssetDatabase.GUIDToAssetPath(config))
                : null;
            if (configInstance != null)
                EditorUtility.SetDirty(configInstance);
            return configInstance;
        }
    }

    public static void Clear()
    {
        if (EditorInstance != null)
            EditorInstance._library.Clear();
    }
    
    [MenuItem("Assets/Create/ConfigLibrary")]
    private static void CreateConfig()
    {
        ConfigUtils.CreateAsset<ConfigLibrary>();
    }
#endif
    }
}
