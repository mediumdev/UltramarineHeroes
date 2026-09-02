using UnityEditor;

namespace Configs.Effects
{
    public class DotHotEffectConfig : EffectConfig
    {
#if UNITY_EDITOR
        [MenuItem("Assets/Create/Configs/Effects/DotHot")]
        private static void Create()
        {
            CreateAsset<DotHotEffectConfig>();
        }
#endif
    }
}