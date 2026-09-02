using System;
using System.Collections.Generic;
using CoreConfigs.Configs;
using UnityEditor;

namespace Game.Shop
{
    public class PurchaseConfig : ConfigBase
    {
        [Serializable]
        public class Product
        {
            public string ProductId;
            public ShopItemConfig ShopItemConfig;
        }

        public List<Product> Products;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Configs/PurchaseConfig")]
        private static void Create()
        {
            CreateAsset<PurchaseConfig>();
        }
#endif
    }
}