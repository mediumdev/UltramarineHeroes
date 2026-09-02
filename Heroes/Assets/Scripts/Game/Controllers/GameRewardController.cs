using System.Collections.Generic;
using Configs;
using UnityEngine;

namespace Game.Controllers
{
    public class GameRewardController : MonoBehaviour
    {
        [SerializeField] private List<RewardContainerConfig> _pvpRewards;

        public List<RewardContainerConfig> PvpRewards => _pvpRewards;
    }
}