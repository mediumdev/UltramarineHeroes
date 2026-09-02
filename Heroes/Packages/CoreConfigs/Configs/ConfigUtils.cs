#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CoreConfigs.Configs
{
    public static class ConfigUtils {
        
        [MenuItem("Assets/Update Configs")]
        private static void UpdateConfigs()
        {
            ConfigLibrary.Clear();
            var configs = AssetDatabase.FindAssets("t:ConfigBase");
            foreach (var guid in configs)
            {
                var current = AssetDatabase.LoadAssetAtPath<ConfigBase>(AssetDatabase.GUIDToAssetPath(guid));
                if (current != null)
                {
                    current.GenerateUid(guid);
                    if (ConfigLibrary.EditorInstance != null && !ConfigLibrary.EditorInstance.Contains(guid))
                        ConfigLibrary.EditorInstance.AddConfig(guid, current);
                }

                EditorUtility.SetDirty(current);
            }
        }

        public static T CreateAsset<T>() where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path?.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            var assetPathAndName =
                AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T) + ".asset");
            
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            UpdateConfigs();
            return asset;
        }
    }
}
#endif