using Enums;
using UnityEngine;

namespace Configs
{
    public class BaseItemConfig : UiConfig
    {
        [SerializeField] private Rarity _rarity;

        public Rarity rarity
        {
            get => _rarity;
            set => _rarity = value;
        }
    }
}