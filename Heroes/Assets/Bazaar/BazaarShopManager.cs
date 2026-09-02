using CoreConfigs.Configs;
using Game.Shop;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Linq;
using BazaarPlugin;
using TMPro;
using System.Reflection;

public class BazaarShopManager : MonoBehaviour
{
    private BazaarConfig _purchaseConfig;

    string[] skus;

    void Start()
    {
        _purchaseConfig = ConfigBase.LoadAll<BazaarConfig>().First();

        skus = new string[_purchaseConfig.Products.Count];

        for (int i = 0; i < _purchaseConfig.Products.Count; i++)
        {
            skus[i] = _purchaseConfig.Products[i].ProductId.ToString();
        }

        PurchaseManager.Initialize();

        querySkuDetails(skus);
    }

    void Update()
    {
        
    }

    public void purchaseProduct(string sku)
    {
        PurchaseManager.PurchaseProduct(sku);
    }

    public void querySkuDetails(string[] skus)
    {
        PurchaseManager.QuerySkuDetails(skus);
    }
}
