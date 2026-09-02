using System;
using System.Collections.Generic;
using CoreConfigs.Configs;
using UnityEngine;

namespace Configs
{
    [Serializable]
    public struct CurrencyRewardWithRange
    {
        public CurrencyConfig currency;
        public int rangeBottom;
        public int rangeTop;
    }

    [Serializable]
    public struct RewardWithWeight
    {
        public CurrencyRewardWithRange currencyRewardWithRange;
        public int weight;
    }

    [Serializable]
    public struct RewardWithWeightList
    {
        public List<RewardWithWeight> rewards;
    }

    public class RewardContainerConfig : ConfigBase
    {
        [SerializeField] private List<RewardWithWeightList> _rewardLists;

        public List<RewardWithWeightList> RewardLists => _rewardLists;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Reward Container Config")]
        private static void Create()
        {
            CreateAsset<RewardContainerConfig>();
        }
#endif
    }
}