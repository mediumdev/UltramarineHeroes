using System;
using Configs;
using CoreConfigs.Configs;
using UnityEngine;

namespace UI.Campaign.CampaignStage
{
    [Serializable]
    public class Level
    {
        public Vector2 LevelPosition;
        public CampaignGameConfig Config;
    }
    
    public class StageConfig : ConfigBase
    {
        [SerializeField] private Vector2 _position;

        [SerializeField] private string _stagename;
        [SerializeField] private string _description;
        
        [SerializeField] private Level[] _levels;
        
        public Level[] Levels => _levels;
        public string Description => _description;
        public string StageName => _stagename;
        public Vector2 Possition => _position;
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Stage Config")]
        private static void Create()
        {
            CreateAsset<StageConfig>();
        }
#endif
    }
}

