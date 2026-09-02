using System;
using System.Collections.Generic;
using CoreConfigs.Configs;
using UnityEditor;

namespace Game.Shop
{
    public class BazaarConfig : ConfigBase
    {
        [Serializable]
        public class Product
        {
            public string ProductId;
            public ShopItemConfig ShopItemConfig;
        }

        public string PublicKey;

        public List<Product> Products;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Configs/BazaarConfig")]
        private static void Create()
        {
            CreateAsset<BazaarConfig>();
        }
#endif
    }
}