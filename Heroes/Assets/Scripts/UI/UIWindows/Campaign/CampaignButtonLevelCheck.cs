using System.Linq;
using UI.Campaign.CampaignStage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Utils.SaveManager;

public class CampaignButtonLevelCheck : MonoBehaviour
{
    [SerializeField] private StageConfig _stageConfig;
    [SerializeField] private Image _lockedImage;
    [SerializeField] private Image _completedImage;
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _levelNumber;
    public StageConfig StageConfig => _stageConfig;
    
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
}
