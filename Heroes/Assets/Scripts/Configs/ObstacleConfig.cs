using System;
using CoreConfigs.Configs;
using Enums;
using Game.Summons;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public class ObstacleConfig : ConfigBase
    {
        [SerializeField] private Obstacle _obstaclePrefab;
        [SerializeField] private int _preloadCount;
        [SerializeField] private ObstacleType _obstacleType;
        [SerializeField] private int _counter;
        
        public Obstacle ObstaclePrefab => _obstaclePrefab;
        public int PreloadCount => _preloadCount;
        public ObstacleType ObstacleType => _obstacleType;
        public int Counter => _counter;
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/ObstacleConfig")]
        private static void Create()
        {
            CreateAsset<ObstacleConfig>();
        }
#endif
    }
}
