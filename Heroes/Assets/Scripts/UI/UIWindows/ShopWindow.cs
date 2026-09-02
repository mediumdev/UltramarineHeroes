using System.Collections.Generic;
using System.Linq;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
    public class ShopWindow : Window
    {
        [SerializeField] private ShopManagerConfig _shopManagerConfig;
        [SerializeField] private RectTransform _shopSectionContainer;
        [SerializeField] private ShopSectionButtonItem _shopSectionButtonItem;
        [SerializeField] private RectTransform _sectionButtonsContainer;

        private List<ShopSectionButtonItem> _shopSectionButtonItems = new List<ShopSectionButtonItem>();

        private ShopSectionItem _currentShopSection;
        private int _currentShopSectionIndex;

        private void OnEnable()
        {
            RepaintSections();
        }
        
        public void Init(int sectionNumber = 0)
        {
            if (_currentShopSection != null)
                Destroy(_currentShopSection.gameObject);

            RepaintSections(sectionNumber);
        }

        public void OpenSection(int sectionNumber = 0)
        {
            if (_currentShopSection != null)
                Destroy(_currentShopSection.gameObject);

            RepaintSections(sectionNumber);
        }

        public void RepaintSections(int sectionNumber = 0)
        {
            if (_shopManagerConfig.ShopSections.Count >= 0)
            {
                _currentShopSectionIndex = sectionNumber;
                var shopSection = _shopManagerConfig.ShopSections[sectionNumber];

                var shopSectionItem = Instantiate(shopSection.SectionPrefab, _shopSectionContainer);
                _currentShopSection = shopSectionItem.GetComponent<ShopSectionItem>();
                _currentShopSection.Initialize(shopSection);

                _shopSectionButtonItems =
                    _shopSectionButtonItems.Where(shopSectionButtonItem =>
                        shopSectionButtonItem != null).ToList();

                foreach (var shopSectionButtonItem in _shopSectionButtonItems)
                    Destroy(shopSectionButtonItem.gameObject);

                for (var i = 0; i < _shopManagerConfig.ShopSections.Count; i++)
                {
                    var shopSectionThis = _shopManagerConfig.ShopSections[i];

                    if (shopSectionThis.ShopItemConfigs.Count <= 0)
                    {
                        var shopSectionButtonItem = Instantiate(_shopSectionButtonItem, _sectionButtonsContainer);
                        shopSectionButtonItem.Init(i, shopSectionThis, sectionNumber == i, false, this);
                        _shopSectionButtonItems.Add(shopSectionButtonItem);
                        if (i == sectionNumber)
                        {
                            OpenSection(sectionNumber + 1);
                            return;
                        }
                    }
                    else
                    {
                        var shopSectionButtonItem = Instantiate(_shopSectionButtonItem, _sectionButtonsContainer);
                        shopSectionButtonItem.Init(i, shopSectionThis, sectionNumber == i, true, this);
                        _shopSectionButtonItems.Add(shopSectionButtonItem);
                    }
                }
            }
        }
        
        public void CloseWindow()
        {
            Close();
        }
    }
}