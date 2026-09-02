using System;
using System.Linq;
using com.adjust.sdk;
using Configs;
using CoreConfigs.Configs;
using CoreUtils.Utils;
using DevToDev.Analytics;
using Game.Controllers;
using UnityEngine;
using UnityEngine.Purchasing;
using Utils;
using Utils.SaveManager;

namespace Game.Shop
{
    public class ShopManager : MonoSingleton<ShopManager>, IStoreListener
    {
        public event Action OnInitPurchaseManger;
        
        /// <summary>
        /// Событие, которое запускается при удачной покупке товара.
        /// </summary>
        public event OnSuccessPurchase OnPurchase;
        public delegate void OnSuccessPurchase(PurchaseEventArgs args);
        
        /// <summary>
        /// Событие, которое запускается при неудачной покупке товара.
        /// </summary>
        public event OnFailedPurchase PurchaseFailed;
        public delegate void OnFailedPurchase(Product product, PurchaseFailureReason failureReason);
        
        
        private CurrencyManager _currencyManager;
        
        private static IStoreController _mStoreController;
        private static IExtensionProvider _mStoreExtensionProvider;
        private string _currentProductId;
        private PurchaseConfig _purchaseConfig;
        
        protected override void Init()
        {
            base.Init();
            _currencyManager = CurrencyManager.Instance;
            
            InitializePurchasing();
#if UNITY_ANDROID
            var storeModule = StandardPurchasingModule.Instance(AppStore.GooglePlay);
#elif UNITY_IOS
            var storeModule = StandardPurchasingModule.Instance(AppStore.MacAppStore);
#endif
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(storeModule);
        }
        
        public void InitializePurchasing()
        {
            if (IsInitialized())
                return;
            
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            _purchaseConfig = ConfigBase.LoadAll<PurchaseConfig>().First();
            foreach (var product in _purchaseConfig.Products)
            {
                builder.AddProduct(product.ProductId, ProductType.Consumable);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public bool IsInitialized()
        {
            return _mStoreController != null && _mStoreExtensionProvider != null;
        }
        
        public string LocalizedPriceString(string productId)
        {
            if (!IsInitialized())
            {
                return "Store not initialized";
            }

            var product = _mStoreController.products.WithID(productId);
            return product.metadata.localizedPriceString;
        }

        public string GetIsoCurrencyCode(string productId)
        {
            if (!IsInitialized())
            {
                return "Store not initialized";
            }

            var product = _mStoreController.products.WithID(productId);
            return product.metadata.isoCurrencyCode;
        }

        public decimal LocalizedPrice(string productId)
        {
            if (!IsInitialized())
                return default;
            var product = _mStoreController.products.WithID(productId);
            return product.metadata.localizedPrice;
        }

        public void BuyShopItemByCry(ShopItemConfig shopItem)
        {
            GetItem(shopItem);

            var currencyToPay = _currencyManager.CurrencyConfigs().First(config => config == shopItem.CurrencyToPay);
            _currencyManager.SubtractCurrencyValue(currencyToPay, (int)shopItem.productPrice);
            
            DTDAnalyticsEvents.Purchase(shopItem.StoreKey, false);
        }
        
        internal void BuyShopItemByRealCry(ShopItemConfig shopItem)
        {
            var key = shopItem.StoreKey;
            if (_purchaseConfig.Products.Any(ncProduct => ncProduct.ProductId == key))
            {
                _currentProductId = key;
                BuyProductID(key);
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError($"ProductKey: {key} not found in PurchaseConfig.");
            }
#endif
        }

        private void GetItem(ShopItemConfig shopItem)
        {
            if (shopItem.itemConfig is CurrencyConfig itemConfig)
            {
                _currencyManager.AddCurrencyValue(itemConfig, shopItem.ProductCount);
            }
#if UNITY_EDITOR
            else
            {
                Debug.Log("Should add item from shop (case where item isn't a currency)");
                // add item
            }
#endif
        }

        private void BuyProductID(string productID)
        {
            if (IsInitialized())
            {
                Product product = _mStoreController.products.WithID(productID);

                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    LoadingProgress.Open();

                    _mStoreController.InitiatePurchase(product);
                }
                else
                {
                    Debug.Log(
                        "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    OnPurchaseFailed(product, PurchaseFailureReason.ProductUnavailable);
                }
            }
        }
       
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            Debug.Log("OnInitialized: PASS");

            _mStoreController = controller;
            _mStoreExtensionProvider = extensions;

            OnInitPurchaseManger?.Invoke();
        }
        
        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            if (_purchaseConfig.Products.Any(product => product.ShopItemConfig.StoreKey == purchaseEvent.purchasedProduct.definition.id))
            {
                if (OnPurchase != null)
                    OnPurchase(purchaseEvent);

                var product = _purchaseConfig.Products.First(p => p.ShopItemConfig.StoreKey == purchaseEvent.purchasedProduct.definition.id);
                GetItem(product.ShopItemConfig);
                
                //DTDAnalyticsEvents.Purchase(product.ShopItemConfig.StoreKey, true);

                //ADJUSTEVENT
                //AdjustEvent purchase_success = new AdjustEvent("n8oqt7");
                //Adjust.trackEvent(purchase_success);

                Debug.Log(_currentProductId + " Buyed!");
            }
            else
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'",
                    purchaseEvent.purchasedProduct.definition.id));
            
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            if (PurchaseFailed != null)
                PurchaseFailed(product, failureReason);

            LoadingProgress.Close();

            //ADJUSTEVENT
            //AdjustEvent purchase_fail = new AdjustEvent("vopsuc");
            //Adjust.trackEvent(purchase_fail);

            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",
                product.definition.storeSpecificId, failureReason));
        }
    }
}