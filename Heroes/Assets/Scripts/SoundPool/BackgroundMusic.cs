using System.Collections;
using UnityEngine;
using Utils;

namespace SoundPool
{
    public class BackgroundMusic : MonoBehaviour
    {
        [SerializeField] private SoundConfig _firstMusic;
        [SerializeField] private float _interval;
        [SerializeField] private SoundConfig _secondMusic;

        private SoundPlayer _player;

        private void Start()
        {
            StartCoroutine(Music());
        }

        private void Update()
        {
            CheckMusicSound();
        }

        private IEnumerator Music()
        {
            _player = SoundManager.Instance.Play(_firstMusic);
            yield return new WaitForSeconds(_interval);
            
            SoundManager.Instance.Stop(_player);
            _player = SoundManager.Instance.Play(_secondMusic);
        }

        private void CheckMusicSound()
        {
            if (_player == null) return;
            
            if (!DynamicDataManager.IsMusicEnabled() && !_player.Paused)
                _player.Pause();
            else if (DynamicDataManager.IsMusicEnabled() && _player.Paused)
                _player.UnPause();
        }
    }
}