using System.Collections.Generic;
using System.Linq;
using Configs;
using CoreUtils.Utils;
using Structs;
using UnityEngine;
using Random = System.Random;

namespace Game.Controllers
{
    public class LootManager : MonoSingleton<LootManager>
    {
        private readonly Random _random = new Random();

        public List<CurrencyWithCount> GetLootFromContainerList(List<RewardContainerConfig> containerList)
        {
            return containerList.SelectMany(GetLootFromContainer).ToList();
        }

        public List<CurrencyWithCount> GetLootFromContainer(RewardContainerConfig container)
        {
            var rewards = new List<CurrencyWithCount>();
            foreach (var rewardList in container.RewardLists)
            {
                var reward = GetRewardFromRewardList(rewardList);
                
                if (reward.currency is null) continue;
                
                rewards.Add(new CurrencyWithCount
                {
                    currency = reward.currency,
                    count = _random.Next(reward.rangeBottom, reward.rangeTop)
                });
            }

            return rewards;
        }

        private CurrencyRewardWithRange GetRewardFromRewardList(RewardWithWeightList rewardsList)
        {
            var totalWeight = rewardsList.rewards.Sum(y => y.weight);
            var randomWeightedIndex = _random.Next(totalWeight);
            var itemWeightedIndex = 0;
            foreach(var item in rewardsList.rewards)
            {
                itemWeightedIndex += item.weight;
                if (randomWeightedIndex < itemWeightedIndex)
                    return item.currencyRewardWithRange;
            }
            
            Debug.LogWarning($"GetRewardFromRewardList - unable to get reward: totalWeight={totalWeight}, " +
                             $"randomWeightedIndex={randomWeightedIndex}");
            return new CurrencyRewardWithRange { currency = null };
        }
    }
}