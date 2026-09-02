using UI.Windows;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace UI
{
    public class GoToScene : MonoBehaviour
    {
        public void ToPreviousSceneUI()
        {
            ToPreviousScene();
        }
        
        public static void ToPreviousScene()
        {
            var previousScene = DynamicDataManager.GetPreviousScene();
            LoadScene(previousScene == string.Empty ? "Lobby" : previousScene);
        }

        public static void LoadScene(string sceneName)
        {
            WindowManager.Instance.CloseAll();
            SceneManager.LoadScene(sceneName);
        }
    }
}