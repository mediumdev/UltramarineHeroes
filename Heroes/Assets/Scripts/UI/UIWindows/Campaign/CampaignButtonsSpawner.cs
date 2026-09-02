using Configs;
using UI.Campaign;
using UI.UIWindows.Campaign;
using UnityEngine;

namespace UI
{
    public class CampaignButtonsSpawner : MonoBehaviour
    {
        [SerializeField] private CampaignChapterConfig _campaignChapterConfig;
        [SerializeField] private CampaignButton _campaignButtonPrefab;

        private void Start()
        {
            var levelCount = 0;
            foreach (var deck in _campaignChapterConfig.Decks)
            {
                if (deck == null) continue;
                
                levelCount += 1;
                var campaignButton = Instantiate(_campaignButtonPrefab, transform);
                campaignButton.gameObject.SetActive(true);
            }
        }
    }
}