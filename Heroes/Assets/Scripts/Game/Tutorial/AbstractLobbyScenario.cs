using System;
using System.Collections.Generic;
using Utils.SaveManager;
using UnityEngine;

namespace Game.Tutorial.Lobby
{
    public abstract class AbstractLobbyScenario : MonoBehaviour
    {
        [SerializeField] private string[] _textArray;
        [SerializeField] private string _scene;
        [SerializeField] private string _name;

        protected List<Action> Steps = new List<Action>();
        public string Name => _name;

        protected virtual void Start()
        {
            CreateSteps();
            LobbyTutorialManager.Instance.AddScenario(this);
        }

        protected virtual void OnDisable()
        {
            LobbyTutorialManager.Instance.RemoveScenario(this);
        }

        protected string GetText(int i)
        {
            return _textArray != null && _textArray.Length > 0
                ? _textArray[Mathf.Min(i, _textArray.Length - 1)]
                : string.Empty;
        }

        protected void GetCurrentStep()
        {
            var step = SaveManagerSafe.GetValue("TutorialStep", 0);
            if (step >= Steps.Count)
            {
                OnComplete();
                return;
            }
            Debug.Log($"Step {step}");
            Steps[step]?.Invoke();
        }

        protected void CompleteStep()
        {
            var step = SaveManagerSafe.GetValue("TutorialStep", 0);
            step++;
            SaveManagerSafe.Add("TutorialStep", step);
        }
        
        // Назначает шаг, который будет первым при повторном запуске тутора, если закрыть его на текущем шаге
        protected void CompleteStep(int nextStep)
        {
            SaveManagerSafe.Add("TutorialStep", nextStep);
        }

        protected void OnComplete()
        {
            SaveManagerSafe.Add("TutorialStep", 0);
            OnCompleteEvent?.Invoke(_name); 
        }
        
        protected virtual void CreateSteps() {}
        public virtual void Play(bool onEnable) {}
        public virtual bool IsValid() { return true; }
        public string Scene => _scene;
        public event Action<string> OnCompleteEvent;
    }
}
