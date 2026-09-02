using System;
using System.Collections.Generic;
using System.Linq;
using CoreUtils.Utils;
using Utils.SaveManager;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Tutorial.Lobby
{
    [Serializable]
    public class Log
    {
        public List<string> List = new List<string>();
    }
    
    public class LobbyTutorialManager : MonoSingleton<LobbyTutorialManager>
    {
        private AbstractLobbyScenario _currentScenario;
        private readonly List<AbstractLobbyScenario> _scenarios = new List<AbstractLobbyScenario>();

        private LobbyTutorialHelper _helper;
        private Log _tutorialLog;

        public LobbyTutorialHelper Helper { get; set; }

        protected override void Init()
        {
            base.Init();

            DontDestroyOnLoad(gameObject);
            _tutorialLog = JsonUtility.FromJson<Log>(SaveManagerSafe.GetValue("TutorialLog", ""));
            if (_tutorialLog == null)
                _tutorialLog = new Log();
        }

        public void AddScenario(AbstractLobbyScenario scenario)
        {
            _scenarios.Add(scenario);
            PlayScenario(true);
        }

        public void RemoveScenario(AbstractLobbyScenario scenario)
        {
            if (_currentScenario == scenario)
                _currentScenario = null;
            
            _scenarios.Remove(scenario);
        }

        private void PlayScenario(bool onEnable)
        {
            if (_currentScenario != null)
                return;
            
            var newScenario =
                _scenarios.FirstOrDefault(x => !IsCompleted(x.Name) && x.IsValid() && string.Equals(x.Scene, SceneManager.GetActiveScene().name));
            if (newScenario == null)
                return;
            _currentScenario = newScenario;
            _currentScenario.Play(onEnable);
            _currentScenario.OnCompleteEvent += CompleteScenario;
        }

        public void CompleteScenario(string scenarioName)
        {
            _tutorialLog.List.Add(scenarioName);
            var log = JsonUtility.ToJson(_tutorialLog);
            SaveManagerSafe.Add("TutorialLog", log);
            _currentScenario = null;
            PlayScenario(false);
        }

        public bool IsCompleted(string scenarioName)
        {
             return _tutorialLog.List.Contains(scenarioName);
        }
    }
}
