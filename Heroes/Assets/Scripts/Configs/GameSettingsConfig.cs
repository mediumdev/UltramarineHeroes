using System.Collections.Generic;
using CoreConfigs.Configs;
using Game.Shop;
using UnityEngine;

namespace Configs
{
    public class GameSettingsConfig : ConfigBase
    {
        [SerializeField] private List<DailyQuestConfig> _dailyQuestBlacklist;
        [SerializeField] private ShopWindow _shopWindow;

        public List<DailyQuestConfig> DailyQuestBlacklist => _dailyQuestBlacklist;
        public ShopWindow ShopWindow => _shopWindow;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Game Settings Config")]
        private static void Create()
        {
            CreateAsset<GameSettingsConfig>();
        }
#endif
    }
}