using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BazaarPlugin;
using System;

#if UNITY_ANDROID

public class CafebazaarProvider : IPurchaseProvider
{
    public event Action ActionBillingSupported;
    public event Action<string> ActionBillingNotSupported;
    public event Action<List<string>, List<string>> ActionQueryInventorySucceeded;
    public event Action<string> ActionQueryInventoryFailed;
    public event Action<List<string>> ActionQuerySkuDetailsSucceeded;
    public event Action<string> ActionQuerySkuDetailsFailed;
    public event Action<List<string>> ActionQueryPurchasesSucceeded;
    public event Action<string> ActionQueryPurchasesFailed;
    public event Action<string> ActionPurchaseSucceeded;
    public event Action<string> ActionPurchaseFailed;
    public event Action<string> ActionConsumePurchaseSucceeded;
    public event Action<string> ActionConsumePurchaseFailed;

    public void Initialize(string publicKey)
    {
        BazaarIAB.init(publicKey);

        IABEventManager.billingSupportedEvent += BillingSupportedEvent;
        IABEventManager.billingNotSupportedEvent += BillingNotSupportedEvent;
        IABEventManager.queryInventorySucceededEvent += QueryInventorySucceededEvent;
        IABEventManager.queryInventoryFailedEvent += QueryInventoryFailedEvent;
        IABEventManager.querySkuDetailsSucceededEvent += QuerySkuDetailsSucceededEvent;
        IABEventManager.querySkuDetailsFailedEvent += QuerySkuDetailsFailedEvent;
        IABEventManager.queryPurchasesSucceededEvent += QueryPurchasesSucceededEvent;
        IABEventManager.queryPurchasesFailedEvent += QueryPurchasesFailedEvent;
        IABEventManager.purchaseSucceededEvent += PurchaseSucceededEvent;
        IABEventManager.purchaseFailedEvent += PurchaseFailedEvent;
        IABEventManager.consumePurchaseSucceededEvent += ConsumePurchaseSucceededEvent;
        IABEventManager.consumePurchaseFailedEvent += ConsumePurchaseFailedEvent;
    }

    private void OnDestroy()
    {
        IABEventManager.billingSupportedEvent -= BillingSupportedEvent;
        IABEventManager.billingNotSupportedEvent -= BillingNotSupportedEvent;
        IABEventManager.queryInventorySucceededEvent -= QueryInventorySucceededEvent;
        IABEventManager.queryInventoryFailedEvent -= QueryInventoryFailedEvent;
        IABEventManager.querySkuDetailsSucceededEvent -= QuerySkuDetailsSucceededEvent;
        IABEventManager.querySkuDetailsFailedEvent -= QuerySkuDetailsFailedEvent;
        IABEventManager.queryPurchasesSucceededEvent -= QueryPurchasesSucceededEvent;
        IABEventManager.queryPurchasesFailedEvent -= QueryPurchasesFailedEvent;
        IABEventManager.purchaseSucceededEvent -= PurchaseSucceededEvent;
        IABEventManager.purchaseFailedEvent -= PurchaseFailedEvent;
        IABEventManager.consumePurchaseSucceededEvent -= ConsumePurchaseSucceededEvent;
        IABEventManager.consumePurchaseFailedEvent -= ConsumePurchaseFailedEvent;
    }

    public void QueryInventory(string[] skus)
    {
        BazaarIAB.queryInventory(skus);
    }

    public void QuerySkuDetails(string[] skus)
    {
        BazaarIAB.querySkuDetails(skus);
    }

    public void QueryPurchases()
    {
        BazaarIAB.queryPurchases();
    }

    public bool SubscriptionsIsSupported()
    {
        return BazaarIAB.areSubscriptionsSupported();
    }

    public void PurchaseProduct(string sku)
    {
        BazaarIAB.purchaseProduct(sku);
    }

    public void ConsumeProduct(string sku)
    {
        BazaarIAB.consumeProduct(sku);
    }

    public void ConsumeProducts(string[] skus)
    {
        BazaarIAB.consumeProducts(skus);
    }

    public void BillingSupportedEvent()
    {
        ActionBillingSupported?.Invoke();

        Debug.Log("Billing is supported");
    }

    public void BillingNotSupportedEvent(string error)
    {
        ActionBillingNotSupported?.Invoke(error);

        Debug.Log("Billing is not supported: " + error);
    }

    private void QueryInventorySucceededEvent(List<BazaarPurchase> purchases, List<BazaarSkuInfo> skus)
    {
        List<string> _purchases = new List<string>();
        List<string> _skus = new List<string>();

        foreach (BazaarPurchase purchase in purchases)
        {
            _purchases.Add(purchase.ToString());
        }

        foreach (BazaarSkuInfo sku in skus)
        {
            _skus.Add(sku.ToString());
        }

        ActionQueryInventorySucceeded?.Invoke(_purchases, _skus);

        Debug.Log(string.Format("SkuDetails: total purchases: {0}, total skus: {1}", purchases.Count, skus.Count));

        Debug.Log("Purchases:");

        for (int i = 0; i < purchases.Count; ++i)
        {
            Debug.Log(purchases[i].ToString());
        }

        Debug.Log("Skus:");

        for (int i = 0; i < skus.Count; ++i)
        {
            Debug.Log(skus[i].ToString());
        }
    }

    private void QueryInventoryFailedEvent(string error)
    {
        ActionQueryInventoryFailed?.Invoke(error);

        Debug.Log("SkuDetails fails: " + error);
    }

    private void QuerySkuDetailsSucceededEvent(List<BazaarSkuInfo> skus)
    {
        List<string> _skus = new List<string>();

        foreach (BazaarSkuInfo sku in skus)
        {
            _skus.Add(sku.ToString());
        }

        ActionQuerySkuDetailsSucceeded?.Invoke(_skus);

        Debug.Log(string.Format("Total skus: {0}", skus.Count));

        for (int i = 0; i < skus.Count; ++i)
        {
            Debug.Log(skus[i].ToString());
        }
    }

    private void QuerySkuDetailsFailedEvent(string error)
    {
        ActionQuerySkuDetailsFailed?.Invoke(error);

        Debug.Log("Skus fails: " + error);
    }

    private void QueryPurchasesSucceededEvent(List<BazaarPurchase> purchases)
    {
        List<string> _purchases = new List<string>();

        foreach (BazaarPurchase purchase in purchases)
        {
            _purchases.Add(purchase.ToString());
        }

        ActionQueryPurchasesSucceeded?.Invoke(_purchases);

        Debug.Log(string.Format("Total purchases: {0}", purchases.Count));

        for (int i = 0; i < purchases.Count; ++i)
        {
            Debug.Log(purchases[i].ToString());
        }
    }

    private void QueryPurchasesFailedEvent(string error)
    {
        ActionQueryPurchasesFailed?.Invoke(error);

        Debug.Log("Purchases fails: " + error);
    }

    private void PurchaseSucceededEvent(BazaarPurchase purchase)
    {
        ActionPurchaseSucceeded?.Invoke(purchase.ToString());

        Debug.Log("Purchase succeed: " + purchase);
    }

    private void PurchaseFailedEvent(string error)
    {
        ActionPurchaseFailed?.Invoke(error);

        Debug.Log("Purchase fail: " + error);
    }

    private void ConsumePurchaseSucceededEvent(BazaarPurchase purchase)
    {
        ActionConsumePurchaseSucceeded?.Invoke(purchase.ToString());

        Debug.Log("Consume product succeed: " + purchase);
    }

    private void ConsumePurchaseFailedEvent(string error)
    {
        ActionConsumePurchaseFailed?.Invoke(error);

        Debug.Log("Consume product fail: " + error);
    }
}

#endif