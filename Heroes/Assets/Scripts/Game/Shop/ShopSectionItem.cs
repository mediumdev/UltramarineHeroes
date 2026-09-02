using UnityEngine;

namespace Game.Shop
{
    public class ShopSectionItem : MonoBehaviour
    {
        [SerializeField] private RectTransform _itemContainer;
        [SerializeField] private RectTransform _extraItemContainer;
        [SerializeField] private ShopItem _shopItemSmall;
        [SerializeField] private ShopItem _shopItemBig;

        private ShopSection _shopSection;

        public void Initialize(ShopSection shopSection)
        {
            _shopSection = shopSection;
            Repaint();
        }

        private void Repaint()
        {
            foreach (var shopItemConfig in _shopSection.ShopItemConfigs)
            {
                var shopItem = shopItemConfig.isDiscount 
                    ? Instantiate(_shopItemBig, _extraItemContainer)
                    : Instantiate(_shopItemSmall, _itemContainer);
                
                shopItem.Init(shopItemConfig);
            }
        }
    }
}