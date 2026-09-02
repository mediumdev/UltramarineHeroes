using System;
using System.Collections.Generic;
using CoreConfigs.Configs;
using UnityEditor;
using UnityEngine;

namespace Game.Shop
{
    public class ShopManagerConfig : ConfigBase
    {
        [SerializeField] private List<ShopSection> _shopSections;

        public List<ShopSection> ShopSections => _shopSections;

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Configs/ShopManagerConfig")]
        private static void Create()
        {
            CreateAsset<ShopManagerConfig>();
        }
#endif
    }

    [Serializable]
    public class ShopSection
    {
        [SerializeField] private string _sectionTitle;
        [SerializeField] private GameObject _sectionPrefab;
        [SerializeField] private string _sectionDescription;
        [SerializeField] private Sprite _sectionImage;
        [SerializeField] private List<ShopItemConfig> _shopProducts;

        public string SectionTitle => _sectionTitle;
        public GameObject SectionPrefab => _sectionPrefab;
        public string SectionDescription => _sectionDescription;
        public Sprite SectionImage => _sectionImage;
        public List<ShopItemConfig> ShopItemConfigs => _shopProducts;
    }
}