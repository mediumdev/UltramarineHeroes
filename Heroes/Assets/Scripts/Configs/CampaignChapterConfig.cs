using CoreConfigs.Configs;
using UnityEngine;

namespace Configs
{
    public class CampaignChapterConfig : ConfigBase
    {
        [SerializeField] private DeckConfig[] _decks;

        public DeckConfig[] Decks => _decks;
        
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Campaign Chapter Config")]
        private static void Create()
        {
            CreateAsset<CampaignChapterConfig>();
        }
#endif
    }
}