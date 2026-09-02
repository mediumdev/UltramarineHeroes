namespace Configs.Effects
{
    public class SpikeEffectConfig : EffectConfig
    {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Effects/Spike")]
        private static void Create()
        {
            CreateAsset<SpikeEffectConfig>();
        }
#endif
    }
}
