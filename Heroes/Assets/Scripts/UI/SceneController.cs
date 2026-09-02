using UI.Windows;
using UnityEngine;
using Utils;

namespace UI
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField] private Window _shop;
        [SerializeField] private Window _mercenary;
        [SerializeField] private Window _avatar;
        [SerializeField] private Window _settings;
        [SerializeField] private Window _dailyQuest;
        [SerializeField] private Window _exitWindow;
        [SerializeField] private Window _spheresWindow;
        [SerializeField] private GameObject _spheres;

        public void OpenShopWindow()
        {
            Debug.Log("click");
            WindowManager.Instance.Open(_shop);
        }
    
        public void OpenAvatarWindow()
        {
            WindowManager.Instance.Open(_avatar);
        }

        public void OpenSpheresScreen()
        {
            _spheres.gameObject.SetActive(true);
        }
    
        public void PlayArena()
        {
            Debug.Log("click PlayArena");
            SavedDataManager.SetFightModePvp();
            DynamicDataManager.SetPreviousScene("Lobby");
            GoToScene.LoadScene("Briefing");
        }
    
        public void PlayStory()
        {
            Debug.Log("click PlayStory");
            SavedDataManager.SetFightModeCampaign();
            GoToScene.LoadScene("Campaign");
        }
     
        public void OpenMercenaryWindow()
        {
            WindowManager.Instance.Open(_mercenary);
        }
    
        public void OpenDailyQuestWindow()
        {
            WindowManager.Instance.Open(_dailyQuest);
        }
    
        public void OpenSettingsWindow()
        {
            WindowManager.Instance.Open(_settings);
        }
        
        public void OpenSpheresWindow()
        {
            WindowManager.Instance.Open(_spheresWindow);
        }
    
        public void OpenExitWindow()
        {
            WindowManager.Instance.Open(_exitWindow);
        }
    }
}
