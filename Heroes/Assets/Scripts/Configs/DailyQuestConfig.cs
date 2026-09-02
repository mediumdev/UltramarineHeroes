using System.Collections.Generic;
using CoreConfigs.Configs;
using UnityEngine;

namespace Configs
{
    public class DailyQuestConfig : ConfigBase
    {
        [SerializeField] private DeckConfig _userDeck;
        [SerializeField] private DeckConfig _botDeck;
        [SerializeField] private List<RewardContainerConfig> _rewards;
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _description;
        [SerializeField] private string _name;

        public DeckConfig UserDeck => _userDeck;
        public DeckConfig BotDeck => _botDeck;
        public List<RewardContainerConfig> Rewards => _rewards;
        public Sprite Icon => _icon;
        public string Description => _description;
        public string Name => _name;
        

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Daily Quest")]
        private static void Create()
        {
            CreateAsset<DailyQuestConfig>();
        }
#endif
    }
}