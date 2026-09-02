using System.Collections;
using System.Linq;
using CoreUtils.Utils;
using DG.Tweening;
using RTLTMPro;
using TMPro;
using UI.Campaign;
using UI.Campaign.CampaignStage;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.SaveManager;

namespace UI.UIWindows.Campaign.CampaignStage
{
    public class StageSelector : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private float _animationTime;
        [SerializeField] private float _maxScale;
        [SerializeField] private float _minScale;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup _levelsCanvasGroup;
        [SerializeField] private RTLTextMeshPro _stagename;
        [SerializeField] private RTLTextMeshPro _description;
        [SerializeField] private string _previousScene;
        [SerializeField] private RectTransform _buttonContainer;
        [SerializeField] private CampaignButton _buttonPrefab;
        [SerializeField] private CampaignButtonLevelCheck[] _check;
        
        private Vector2 _position;
        private bool _animationComplete = true;
        private StageConfig _config;

        private void Awake()
        {
            //--начало TODO удалить после сборки с фейк-сейвом для Алексея
            /*const int levelFinishLimit = 40;
            var levelsFinished = SaveManager.GetValue<string>(SavedDataManager.FinishedCampaignFightsKey).Split(';');
            if (levelsFinished.Length < levelFinishLimit)
            {
                Debug.Log($"Add {levelFinishLimit} completed levels");
                var levelsCompletedCount = 0;
                var levelsCompleted = "";
                foreach (var chapter in _check)
                {
                    if (levelsCompletedCount >= levelFinishLimit) break;
                    
                    foreach (var level in chapter.StageConfig.Levels)
                    {
                        if (levelsCompletedCount >= levelFinishLimit) break;

                        levelsCompleted += levelsCompleted == "" ? level.Config.Uid : $";{level.Config.Uid}";
                        levelsCompletedCount += 1;
                    }
                }
                SaveManager.Add(SavedDataManager.FinishedCampaignFightsKey, levelsCompleted);
            }
            */
            //-конец TODO Удалить
            
            Repaint();
        }

        private void Repaint()
        {
            var levelsFinished = SavedDataManager.GetFinishedCampaignFights();
            var uncompletedLevelFound = false;
            
            foreach (var chapter in _check)
            {
                if (chapter.StageConfig.Levels.All(x => levelsFinished.Contains(x.Config.Uid)))
                {
                    chapter.SetCompleted();
                }
                else
                {
                    if (!uncompletedLevelFound)
                    {
                        uncompletedLevelFound = true;
                    }
                    else
                    {
                        chapter.SetLocked();
                    }
                }
            }
        }
        
        public void SetStage(StageConfig config)
        {
            if (!_animationComplete) return;

            _position = _rect.anchoredPosition;
            _config = config;
            _animationComplete = false;
            _scrollRect.StopMovement();
            _scrollRect.enabled = false;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _description.text = config.Description;
            _stagename.text = config.StageName;
            
            CreateButtons();

            _canvasGroup.DOFade(0f, _animationTime / 2f);
            _levelsCanvasGroup.DOFade(1f,_animationTime);
            _rect.DOAnchorPos(config.Possition, _animationTime);
            _rect.DOScale(_maxScale, _animationTime).OnComplete(OnAnimationComplete);
        }

        public void Back()
        {
            if (!_animationComplete) return;

            if (_config == null)
            {
                GoToScene.LoadScene(_previousScene);
            }

            _config = null;
            _animationComplete = false;
            _levelsCanvasGroup.interactable = false;
            _levelsCanvasGroup.blocksRaycasts = false;
            
            _canvasGroup.DOFade(1f, _animationTime);
            _levelsCanvasGroup.DOFade(0f,_animationTime / 2f);
            _rect.DOAnchorPos(_position, _animationTime);
            _rect.DOScale(_minScale, _animationTime).OnComplete(OnBackAnimationComplete);
        }

        private void CreateButtons()
        {
            _buttonContainer.Clear();
            
            var levelsFinished = SavedDataManager.GetFinishedCampaignFights();
            var uncompletedLevelFound = false;
            int count = 0; 
            
            foreach (var level in _config.Levels)
            {
                count += 1;
                var item = Instantiate(_buttonPrefab, _buttonContainer);
                item.Init(level, count);
                if (count % 5 == 0)
                {
                    item.SetHardLevel();
                }

                if (((IList) levelsFinished).Contains(level.Config.Uid))
                {
                    item.SetCompleted();
                }
                else
                {
                    if (!uncompletedLevelFound)
                    {
                        uncompletedLevelFound = true;
                    }
                    else
                    {
                        item.SetLocked();
                    }
                }
            }
        }

        private void OnBackAnimationComplete()
        {
            _animationComplete = true;
            _scrollRect.enabled = true;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        private void OnAnimationComplete()
        {
            _animationComplete = true;
            _levelsCanvasGroup.interactable = true;
            _levelsCanvasGroup.blocksRaycasts = true;
        }
    }
}
