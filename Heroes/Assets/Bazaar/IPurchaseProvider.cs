using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPurchaseProvider
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

    public void Initialize(string publicKey);
    public void QueryInventory(string[] skus);
    public void QuerySkuDetails(string[] skus);
    public void QueryPurchases();
    public bool SubscriptionsIsSupported();
    public void PurchaseProduct(string sku);
    public void ConsumeProduct(string sku);
    public void ConsumeProducts(string[] skus);
}
