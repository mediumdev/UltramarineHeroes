using System.Collections.Generic;
using System.Linq;
using CoreConfigs.Configs;
using CoreUtils.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace SoundPool
{
    public class SoundEntry
    {
        public string SoundId;
        public int ClipsInBackground;
    }
    
    public class SoundManager : MonoSingleton<SoundManager>
    {
        private SoundManagerConfig _config;
        private readonly List<SoundEntry> _entries = new List<SoundEntry>();
        private readonly List<SoundPlayer> _players = new List<SoundPlayer>();

        private bool _soundEnabled = true;

        public bool SoundEnabled
        {
            get => _soundEnabled && DynamicDataManager.IsSoundEnabled();
            set
            {
                _soundEnabled = value;
                if (SoundEnabled) return;
                
                foreach (var entry in _entries)
                    StopAll(entry.SoundId);
            }
        }

        protected override void Init()
        {
            base.Init();
            _config = ConfigLibrary.Instance.LoadFirstAvailable<SoundManagerConfig>() as SoundManagerConfig;
            SceneManager.sceneLoaded += SceneLoaded;
            DontDestroyOnLoad(gameObject);
        }

        private void SceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _entries.Clear();
            _players.Clear();
        }

        public SoundPlayer Play(SoundConfig sound)
        {
            if (sound == null)
            {
                // Debug.LogWarning("There is no sound config");
                return null;
            }
            
            if (!SoundEnabled)
                return null;
            
            var entry = _entries.FirstOrDefault(x => x.SoundId.Contains(sound.Uid));
            if (entry == null)
            {
                entry = CreateEntry(sound);
                _entries.Add(entry);
            }

            if (entry == null)
            {
                Debug.LogErrorFormat("something goes wrong in sound manager on playing {0}", sound);
                return null;
            }

            if (entry.ClipsInBackground >= sound.MaxClipsInBackground)
                return null;

            entry.ClipsInBackground++;
            var player = Instantiate(_config.CommonSoundPlayer);
            player.Init(sound);
            _players.Add(player);
            return player;
        }

        private SoundEntry CreateEntry(SoundConfig sound)
        {
            return new SoundEntry
            {
                SoundId = sound.Uid,
                ClipsInBackground = 0
            };
        }
        
        public void Stop(SoundPlayer soundPlayer)
        {
            RemoveSound(soundPlayer.SoundId);
            _players.Remove(soundPlayer);
            Destroy(soundPlayer.gameObject);
        }
        
        public void StopAll(SoundConfig config)
        {
            StopAll(config.Uid);
        }

        private void StopAll(string soundId)
        {
            var removedPlayers = new List<SoundPlayer>();
            foreach (var player in _players.Where(x => x.SoundId.Contains(soundId)))
            {
                removedPlayers.Add(player);
            }
            foreach (var player in removedPlayers)
            {
                Stop(player);
            }
        }

        private void RemoveSound(string soundId)
        {
            var entry = _entries.FirstOrDefault(x => x.SoundId.Contains(soundId));
            if (entry == null)
                return;

            entry.ClipsInBackground = Mathf.Max(0, entry.ClipsInBackground - 1);
        }
        
        public void ChangeVolume(SoundConfig config, float maxValue)
        {
            var sounds = _players.Where(x => x.SoundId.Contains(config.Uid));
            foreach (var sound in sounds)
            {
                sound.ChangeVolume(maxValue);
            }
        }
    }
}