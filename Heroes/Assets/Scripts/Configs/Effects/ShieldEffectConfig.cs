namespace Configs.Effects
{
    public class ShieldEffectConfig : EffectConfig
    {
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Effects/Shield")]
        private static void Create()
        {
            CreateAsset<ShieldEffectConfig>();
        }
#endif
    }
}