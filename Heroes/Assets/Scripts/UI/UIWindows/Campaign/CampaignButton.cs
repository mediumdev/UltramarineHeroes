using Configs;
using Dynamic;
using TMPro;
using UI.Campaign.CampaignStage;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.UIWindows.Campaign
{
    public class CampaignButton : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private Image _completedImage;
        [SerializeField] private Image _lockedImage;
        [SerializeField] private Image _crownImage;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _levelNumber;
        
        private CampaignGameConfig _game;

        public void StartCampaignLevel()
        {
            SavedDataManager.SaveBotDeck(_game.BotDeck);
            DynamicDataManager.SetNextRewards(_game.Rewards);
            DynamicVarLibrary.Instance.AddVar(DynamicDataManager.CurrentCampaignFightKey, _game.Uid);

            //_networkLobbyController.LoadGameSceneBot(1, SavedDataManager.FightModeCampaign);
            DynamicDataManager.SetPreviousScene("Campaign");
            GoToScene.LoadScene("Briefing");
        }

        public void SetCompleted()
        {
            _completedImage.gameObject.SetActive(true);
            _levelNumber.gameObject.SetActive(false);
        }
        public void SetLocked()
        {
            _lockedImage.gameObject.SetActive(true);
            _button.interactable = false;
            _levelNumber.gameObject.SetActive(false);
        }

        public void SetHardLevel()
        {
            _crownImage.gameObject.SetActive(true);
        }
        
        public void Init(Level level, int number)
        {
            _rect.anchoredPosition = level.LevelPosition;
            _game = level.Config;
            _levelNumber.text = number.ToString();
        }
    }
}