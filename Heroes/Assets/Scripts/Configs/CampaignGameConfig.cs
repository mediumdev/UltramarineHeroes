using System.Collections.Generic;
using CoreConfigs.Configs;
using Enums;
using UnityEngine;

namespace Configs
{
    public class CampaignGameConfig : ConfigBase
    {
        [SerializeField] private DeckConfig _botDeck;
        [SerializeField] private List<RewardContainerConfig> _rewards;
        [SerializeField] private BotDifficulty _botDifficulty;

        public DeckConfig BotDeck => _botDeck;
        public List<RewardContainerConfig> Rewards => _rewards;
        public BotDifficulty BotDifficulty => _botDifficulty;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Campaign Game")]
        private static void Create()
        {
            CreateAsset<CampaignGameConfig>();
        }
#endif
    }
}