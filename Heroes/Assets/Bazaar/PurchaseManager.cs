using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreConfigs.Configs;
using Game.Shop;
using UnityEngine;
using UnityEngine.Purchasing;

#if UNITY_ANDROID

public static class PurchaseManager
{
    public static event Action ActionBillingSupported;
    public static event Action<string> ActionBillingNotSupported;
    public static event Action<List<string>, List<string>> ActionQueryInventorySucceeded;
    public static event Action<string> ActionQueryInventoryFailed;
    public static event Action<List<string>> ActionQuerySkuDetailsSucceeded;
    public static event Action<string> ActionQuerySkuDetailsFailed;
    public static event Action<List<string>> ActionQueryPurchasesSucceeded;
    public static event Action<string> ActionQueryPurchasesFailed;
    public static event Action<string> ActionPurchaseSucceeded;
    public static event Action<string> ActionPurchaseFailed;
    public static event Action<string> ActionConsumePurchaseSucceeded;
    public static event Action<string> ActionConsumePurchaseFailed;

    private static IPurchaseProvider _purchaseProvider = null;

    public static void Initialize()
    {
        BazaarConfig _bazaarConfig;
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        _bazaarConfig = ConfigBase.LoadAll<BazaarConfig>().First();

        _purchaseProvider = new CafebazaarProvider();
        _purchaseProvider.Initialize(_bazaarConfig.PublicKey);

        _purchaseProvider.ActionBillingSupported += BillingSupported;
        _purchaseProvider.ActionBillingNotSupported += BillingNotSupported;
        _purchaseProvider.ActionQueryInventorySucceeded += QueryInventorySucceeded;
        _purchaseProvider.ActionQueryInventoryFailed += QueryInventoryFailed;
        _purchaseProvider.ActionQuerySkuDetailsSucceeded += QuerySkuDetailsSucceeded;
        _purchaseProvider.ActionQuerySkuDetailsFailed += QuerySkuDetailsFailed;
        _purchaseProvider.ActionQueryPurchasesSucceeded += QueryPurchasesSucceeded;
        _purchaseProvider.ActionQueryPurchasesFailed += QueryPurchasesFailed;
        _purchaseProvider.ActionPurchaseSucceeded += PurchaseSucceeded;
        _purchaseProvider.ActionPurchaseFailed += PurchaseFailed;
        _purchaseProvider.ActionConsumePurchaseSucceeded += ConsumePurchaseSucceeded;
        _purchaseProvider.ActionConsumePurchaseFailed += ConsumePurchaseFailed;
    }

    private static void OnDisable()
    {
        _purchaseProvider.ActionBillingSupported -= BillingSupported;
        _purchaseProvider.ActionBillingNotSupported -= BillingNotSupported;
        _purchaseProvider.ActionQueryInventorySucceeded -= QueryInventorySucceeded;
        _purchaseProvider.ActionQueryInventoryFailed -= QueryInventoryFailed;
        _purchaseProvider.ActionQuerySkuDetailsSucceeded -= QuerySkuDetailsSucceeded;
        _purchaseProvider.ActionQuerySkuDetailsFailed -= QuerySkuDetailsFailed;
        _purchaseProvider.ActionQueryPurchasesSucceeded -= QueryPurchasesSucceeded;
        _purchaseProvider.ActionQueryPurchasesFailed -= QueryPurchasesFailed;
        _purchaseProvider.ActionPurchaseSucceeded -= PurchaseSucceeded;
        _purchaseProvider.ActionPurchaseFailed -= PurchaseFailed;
        _purchaseProvider.ActionConsumePurchaseSucceeded -= ConsumePurchaseSucceeded;
        _purchaseProvider.ActionConsumePurchaseFailed -= ConsumePurchaseFailed;
    }

    public static void QueryInventory(string[] skus)
    {
        _purchaseProvider.QueryInventory(skus);
    }

    public static void QuerySkuDetails(string[] skus)
    {
        _purchaseProvider.QuerySkuDetails(skus);
    }

    public static void QueryPurchases()
    {
        _purchaseProvider.QueryPurchases();
    }

    public static bool SubscriptionsIsSupported()
    {
        return _purchaseProvider.SubscriptionsIsSupported();
    }

    public static void PurchaseProduct(string sku)
    {
        _purchaseProvider.PurchaseProduct(sku);
    }

    public static void ConsumeProduct(string sku)
    {
        _purchaseProvider.ConsumeProduct(sku);
    }

    public static void ConsumeProducts(string[] skus)
    {
        _purchaseProvider.ConsumeProducts(skus);
    }

    private static void BillingSupported()
    {
        ActionBillingSupported?.Invoke();
    }

    private static void BillingNotSupported(string error)
    {
        ActionBillingNotSupported?.Invoke(error);
    }

    private static void QueryInventorySucceeded(List<string> purchases, List<string> skus)
    {
        ActionQueryInventorySucceeded?.Invoke(purchases, skus);
    }

    private static void QueryInventoryFailed(string error)
    {
        ActionQueryInventoryFailed?.Invoke(error);
    }

    private static void QuerySkuDetailsSucceeded(List<string> skus)
    {
        ActionQuerySkuDetailsSucceeded?.Invoke(skus);
    }

    private static void QuerySkuDetailsFailed(string error)
    {
        ActionQuerySkuDetailsFailed?.Invoke(error);
    }

    private static void QueryPurchasesSucceeded(List<string> purchases)
    {
        ActionQueryPurchasesSucceeded?.Invoke(purchases);
    }

    private static void QueryPurchasesFailed(string error)
    {
        ActionQueryPurchasesFailed?.Invoke(error);
    }

    private static void PurchaseSucceeded(string purchase)
    {
        ActionPurchaseSucceeded?.Invoke(purchase);
    }

    private static void PurchaseFailed(string error)
    {
        ActionPurchaseFailed?.Invoke(error);
    }

    private static void ConsumePurchaseSucceeded(string purchase)
    {
        ActionConsumePurchaseSucceeded?.Invoke(purchase);
    }

    private static void ConsumePurchaseFailed(string error)
    {
        ActionConsumePurchaseFailed?.Invoke(error);
    }
}

#endif