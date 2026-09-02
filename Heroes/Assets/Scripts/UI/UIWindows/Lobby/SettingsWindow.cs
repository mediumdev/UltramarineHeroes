using UI.Windows;
using UnityEngine;
using Utils;

namespace UI.UIWindows.Lobby
{
    public class SettingsWindow : Window
    {
        [SerializeField] private AboutWindow _about;
        [SerializeField] private GameObject _musicCheck;
        [SerializeField] private GameObject _soundCheck;
        [SerializeField] private AudioSource _lobbySound;
    
        private void OnEnable()
        {
            _lobbySound = GameObject.Find("AudioSource").GetComponent<AudioSource>();
            
            _musicCheck.SetActive(DynamicDataManager.IsMusicEnabled());
            _soundCheck.SetActive(DynamicDataManager.IsSoundEnabled());
        }

        public void ToggleMusic()
        {
            var musicEnabled = !DynamicDataManager.IsMusicEnabled();
            _musicCheck.SetActive(musicEnabled);
            DynamicDataManager.SetMusicEnabled(musicEnabled);
            if (musicEnabled)
                _lobbySound.UnPause();
            else
                _lobbySound.Pause();
        }

        public void ToggleSound()
        {
            var musicEnabled = !DynamicDataManager.IsSoundEnabled();
            _soundCheck.SetActive(musicEnabled);
            DynamicDataManager.SetSoundEnabled(musicEnabled);
        }
    
        public void OpenAboutWindow()
        {
            var window = WindowManager.Instance.Open(_about, true) as AboutWindow;
        }

        public void OpenPrivacyPolicyUrl()
        {
            Application.OpenURL("http://ultramarinegames.com/index.php/privacy-policy");
        }

        public void OpenContactUrl()
        {
            Application.OpenURL("http://ultramarinegames.com/");
        }
    }
}
