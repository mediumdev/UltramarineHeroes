using CoreConfigs.Configs;
using UnityEngine;

namespace SoundPool
{
    public class SoundConfig : ConfigBase
    {
        [SerializeField] private AudioClip[] _clips;
        [SerializeField] private int _maxClipsInBackground = 1;
        [SerializeField, Range(0, 1)] private float _volume = 1f;
        [SerializeField] private bool _loop = false;
        [SerializeField] private bool _static;

        public int MaxClipsInBackground => _maxClipsInBackground;
        public float Volume
        {
            get => _volume;
            set => _volume = value;
        }

        public bool Loop => _loop;

        private int _staticIndex;
        
        public AudioClip GetClip()
        {
            if (_static)
            {
                _staticIndex++;
                if (_staticIndex >= _clips.Length)
                    _staticIndex = 0;
            }
            return _clips != null && _clips.Length > 0 ? _static ? _clips[_staticIndex] :_clips[Random.Range(0, _clips.Length)] : null;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Sound/SoundConfig")]
        private static void Create()
        {
            CreateAsset<SoundConfig>();
        }
#endif
    }
}
