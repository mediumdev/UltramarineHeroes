using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Configs;
using CoreConfigs.Configs;
using Game.Controllers;
using JetBrains.Annotations;
using Structs;
using TMPro;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
    public class ShopItem : MonoBehaviour
    {
        private event Action OnClick;
        
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Image _background;
        [SerializeField] private Image _itemImage;
        [SerializeField] private GameObject _numberItem;
        [SerializeField] private GameObject _numberItemText;
        [SerializeField] private GameObject _discount;
        [SerializeField] private TextMeshProUGUI _priceTxt;
        [SerializeField] private TextMeshProUGUI _discountPriceTxt;
        [SerializeField] private GameObject _currencyIcon;
        [SerializeField] private Button _buyButton;
        [SerializeField] private ShopWarningWindow _shopWarningWindow;
        private BazaarShopManager _bazaarShopManager;

        private ShopItemConfig _shopItemConfig;
        private CurrencyConfig _payCurrency;
        private bool _enoughCurrency;
        private const int CrystalPackSection = 0;

        public ShopItemConfig Config => _shopItemConfig;

        public void Init(ShopItemConfig shopItemConfig)
        {
            _bazaarShopManager = FindObjectOfType(typeof(BazaarShopManager)) as BazaarShopManager;

            _payCurrency = ConfigBase.LoadAll<CurrencyConfig>().FirstOrDefault(config => config == shopItemConfig.CurrencyToPay);
            _shopItemConfig = shopItemConfig;
            CurrencyManager.Instance.CurrencyChangedEvent += OnCurrencyChanged;
            Repaint();
        }

        private void OnDisable()
        {
            CurrencyManager.Instance.CurrencyChangedEvent -= OnCurrencyChanged;
        }

        private void OnCurrencyChanged(List<CurrencyWithCount> changedCurrencies)
        {
            Repaint();
        }

        private void Repaint()
        {
            if (_shopItemConfig == null)
                return;
            
            if (_title != null) 
                _title.text = _shopItemConfig.itemConfig.Title;

            if (_shopItemConfig.itemConfig != null)
            {
                if (_shopItemConfig.Background != null)
                {
                    _background.sprite = _shopItemConfig.Background;
                }
                _itemImage.sprite = _shopItemConfig.Icon;
            }

            if (_shopItemConfig.ProductCount > 1)
            {
                if (_numberItemText == null)
                    return;
                _numberItemText.SetActive(true);
                _numberItemText.GetComponentInChildren<TextMeshProUGUI>().text = _shopItemConfig.ProductCount.ToString();
            }
            else
            {
                _numberItem.SetActive(false);
            }
            
            if (_currencyIcon != null)
                _currencyIcon.SetActive(_shopItemConfig.productPriceInCry);

            if (_shopItemConfig.productPriceInCry)
            {
                if (!_shopItemConfig.isDiscount)
                {
                    _priceTxt.text = _shopItemConfig.productPrice.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    if (_shopItemConfig.isDiscount)
                    {
                        if (_discount != null)
                        {
                            _discount.SetActive(true);
                            _discountPriceTxt.gameObject.SetActive(true);
                            _discountPriceTxt.text =
                                _shopItemConfig.discountPercent + "%";
                            _priceTxt.text =
                                Math.Round((100 - _shopItemConfig.discountPercent) / 100 * _shopItemConfig.productPrice)
                                    .ToString(
                                        CultureInfo
                                            .InvariantCulture);
                        }
                    }
                }

                _currencyIcon.GetComponent<Image>().sprite = _shopItemConfig.CurrencyToPay.Icon;
                
                _enoughCurrency = CurrencyManager.Instance.EnoughCurrency(_payCurrency, Convert.ToInt32(_priceTxt.text));
                if (!_enoughCurrency && _buyButton != null)
                {
                    var btn = _buyButton.GetComponent<Button>();
                    var buyButtonColors = btn.colors;
                    buyButtonColors.normalColor = Color.white;
                    btn.colors = buyButtonColors;
                }
            }
            else
            {
                if (!_shopItemConfig.isDiscount)
                {
                    var priceText = ShopManager.Instance.LocalizedPriceString(_shopItemConfig.StoreKey);
                    var price = ShopManager.Instance.LocalizedPrice(_shopItemConfig.StoreKey);
                    var isoCurrencyCode = ShopManager.Instance.GetIsoCurrencyCode(_shopItemConfig.StoreKey);
                    
                    if (priceText != "Store not initialized")
                        _priceTxt.text = $"{price} {isoCurrencyCode}";
                    else
                         ShowFakePrice();
                }
                else
                {
                    _discount.SetActive(true);
                    _discountPriceTxt.gameObject.SetActive(true);
                    
                    var priceText = ShopManager.Instance.LocalizedPriceString(_shopItemConfig.StoreKey);
                    var price = ShopManager.Instance.LocalizedPrice(_shopItemConfig.StoreKey);
                    var isoCurrencyCode = ShopManager.Instance.GetIsoCurrencyCode(_shopItemConfig.StoreKey);
                    if (priceText != "Store not initialized")
                    {
                        _priceTxt.text = $"{price} {isoCurrencyCode}";
                        _discountPriceTxt.text = Math
                            .Round(((price * 100) / (100 - (decimal) _shopItemConfig.discountPercent)))
                            .ToString(CultureInfo.InvariantCulture);
                    }
                    else
                        ShowFakeDiscountPrice();
                }
            }
        }

        private void ShowFakePrice()
        {
            _priceTxt.text = _shopItemConfig.productPrice.ToString(CultureInfo.InvariantCulture) + " $";
        }

        private void ShowFakeDiscountPrice()
        {
            if (_discount == null)
                return;
            _discount.SetActive(true);
            _discountPriceTxt.gameObject.SetActive(true);
            _discountPriceTxt.text = _shopItemConfig.discountPercent + "%";
            _priceTxt.text =
                Math.Round(_shopItemConfig.productPrice / (100 - _shopItemConfig.discountPercent), 2)
                    .ToString(CultureInfo.InvariantCulture) + " $";
        }

        [UsedImplicitly]
        public void Click()
        {
            /*if (_shopItemConfig.productPriceInCry)
            {
                if (!_enoughCurrency)
                {
                    var shopWarningWindow = WindowManager.Instance.Open(_shopWarningWindow, true) as ShopWarningWindow;
                    if (shopWarningWindow != null)
                    {
                        shopWarningWindow.Init(CrystalPackSection);
                    }

                    OnClick?.Invoke();
                    return;
                }

                ShopManager.Instance.BuyShopItemByCry(_shopItemConfig);
            }
            else
            {
                    ShopManager.Instance.BuyShopItemByRealCry(_shopItemConfig);
            }*/

            _bazaarShopManager.purchaseProduct(_shopItemConfig.StoreKey);

            OnClick?.Invoke();
        }
    }
}