using CoreConfigs.Configs;
using Configs;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Game.Shop
{
    public class ShopItemConfig : ConfigBase
    {
        [SerializeField] private BaseItemConfig _item;
        [SerializeField] private int _productCount;
        [SerializeField] private float _productPrice;
        [SerializeField] private bool _productPriceInCry;
        [SerializeField] private CurrencyConfig _currencyToPay;
        [SerializeField] private bool _isDiscount;
        [SerializeField] private float _discountPercent;
        [SerializeField] private string _storeKey;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Sprite _background;


        public BaseItemConfig itemConfig => _item;
        public float productPrice => _productPrice;
        public bool productPriceInCry => _productPriceInCry;
        public CurrencyConfig CurrencyToPay => _currencyToPay;
        public bool isDiscount => _isDiscount;
        public string StoreKey => _storeKey;
        public float discountPercent => _discountPercent;
        public int ProductCount => _productCount;
        [JsonIgnore] public Sprite Background => _background;
        [JsonIgnore] public Sprite Icon => _icon;


#if UNITY_EDITOR
        [MenuItem("Assets/Create/Configs/ShopItemConfig")]
        private static void Create()
        {
            CreateAsset<ShopItemConfig>();
        }
#endif
    }
}