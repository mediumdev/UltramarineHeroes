using System;
using System.Collections.Generic;
using CoreConfigs.Configs;
using Game.Shop;
using UnityEditor;
using UnityEngine;

namespace Purchase
{
    public class PurchaseConfig : ConfigBase
    {
        [Serializable]
        public class NcProduct
        {
            public string ProductId;
            public ShopItemConfig ShopItemConfig;
        }

        [Serializable]
        public class CProduct
        {
            public string ProductId;
            public ShopItemConfig ShopItemConfig;
        }

        [Tooltip("Не многоразовые товары. Больше подходит для отключения рекламы и т.п.")]
        public List<NcProduct> ncProducts;

        [Tooltip("Многоразовые товары. Больше подходит для покупки игровой валюты и т.п.")]
        public List<CProduct> cProducts;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Configs/PurchaseConfig")]
        private static void Create()
        {
            CreateAsset<PurchaseConfig>();
        }
#endif
    }
}