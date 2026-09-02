using System.Collections;
using UnityEngine;

namespace SoundPool
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField, HideInInspector] private AudioSource _source;

        private SoundConfig _soundConfig;
        private bool _paused;

        public string SoundId => _soundConfig.Uid;
        public bool Paused => _paused;

        public void Init(SoundConfig sound)
        {
            _soundConfig = sound;
            _source.clip = _soundConfig.GetClip();
            _source.volume = _soundConfig.Volume;
            _source.loop = _soundConfig.Loop;
        }

        public void Pause()
        {
            _source.Pause();
            _paused = true;
        }

        public void UnPause()
        {
            _source.UnPause();
            _paused = false;
        }
        
        private void Start()
        {
            _source.Play();
            StartCoroutine(SoundFinish());
        }

        private IEnumerator SoundFinish()
        {
            yield return new WaitUntil(() => !_source.isPlaying);
            SoundManager.Instance.Stop(this);
        }
        
        public void ChangeVolume(float maxValue)
        {
            _source.volume = maxValue;
        }

        private void OnValidate()
        {
            if (_source == null)
                _source = GetComponent<AudioSource>();
        }
    }
}
