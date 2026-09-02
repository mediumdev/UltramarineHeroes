using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Utils.SaveManager
{
    public static class SaveManager
    {
        private static readonly JObject jObject = LoadJson();
        private static string _jsonPath;
        private static JObject _defaultObject;


        private static JObject LoadJson()
        {
#if UNITY_ANDROID && !UNITY_EDITOR || UNITY_IOS && !UNITY_EDITOR || UNITY_STANDALONE_WIN && !UNITY_EDITOR
           if (File.Exists(Path.Combine(Application.persistentDataPath, "SettingsData.json")))
            {
            _jsonPath = Path.Combine(Application.persistentDataPath, "SettingsData.json");
            }
            else
            {
                File.WriteAllText(Path.Combine(Application.persistentDataPath, "SettingsData.json"),
                    "{}");
                _jsonPath = Path.Combine(Application.persistentDataPath, "SettingsData.json");
            }
#else
            var settingsDirectoryPath = Application.dataPath + "/Resources/Settings";
            if (Directory.Exists(settingsDirectoryPath))
            {
                var settingsDataPath = GetSettingsDataFilePath();
                if (File.Exists(GetSettingsDataFilePath()))
                {
                    _jsonPath = settingsDataPath;
                }
                else
                {
                    CreateAndFillSettingsFile();
                }
            }
            else
            {
                Directory.CreateDirectory(settingsDirectoryPath);
                CreateAndFillSettingsFile();
            }
#endif
            string jsonTxt = File.ReadAllText(_jsonPath);
            return JObject.Parse(jsonTxt);
        }

        private static void CreateAndFillSettingsFile()
        {
            File.WriteAllText(GetSettingsDataFilePath(),
                "{}");
            _jsonPath = GetSettingsDataFilePath();
        }

        private static string GetSettingsDataFilePath()
        {
            var settingsPath = Application.dataPath + "/Resources/Settings/";
            return Path.Combine(settingsPath, "SettingsData.json");
        }

        public static void SaveJson()
        {
            string jsonTxt = jObject.ToString(Formatting.Indented);
            File.WriteAllText(_jsonPath, jsonTxt);
        }

        public static void Add(string key, JToken value)
        {
            if (HasKey(key))
            {
                jObject.Properties().First(property => property.Name == key).Value = value;
            }
            else
            {
                jObject.Add(key, value);
            }

            SaveJson();
        }

        public static T GetValue<T>(string key)
        {
            if (typeof(T) == typeof(bool))
            {
                return (T) (object) GetBoolValue(key);
            }

            if (typeof(T) == typeof(string))
            {
                return (T) (object) GetStringValue(key);
            }

            T defaultValue = default;
            return GetValue(key, defaultValue);
        }

        public static T GetValue<T>(string key, T defaultValue)
        {
            if (HasKey(key))
            {
                return jObject.Properties().First(property => property.Name == key).Value.Value<T>();
            }

            Add(key, defaultValue.ToString());
            return defaultValue;
        }

        private static bool GetBoolValue(string key)
        {
            if (HasKey(key))
            {
                return jObject.Properties().First(property => property.Name == key).Value.Value<bool>();
            }

            Add(key, false);
            return false;
        }

        private static string GetStringValue(string key)
        {
            if (HasKey(key))
            {
                return jObject.Properties().First(property => property.Name == key).Value.Value<string>();
            }

            Add(key, "");
            return "";
        }


        public static void Remove(string key)
        {
            if (HasKey(key))
            {
                jObject.Properties().First(property => property.Name == key).Remove();
            }

            SaveJson();
        }

        public static bool HasKey(string key)
        {
            return jObject.Properties().Any(property => property.Name == key);
        }
    }
}