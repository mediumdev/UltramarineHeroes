using CoreConfigs.Configs;
using UnityEngine;

namespace SoundPool
{
    public class SoundManagerConfig : ConfigBase
    {
        [SerializeField] private SoundPlayer _commonSoundPlayer;

        public SoundPlayer CommonSoundPlayer => _commonSoundPlayer;
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Sound/SoundManagerConfig")]
        private static void Create()
        {
            CreateAsset<SoundManagerConfig>();
        }
#endif
    }
}
