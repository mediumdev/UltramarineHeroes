using System.Collections.Generic;
using Configs;
using CoreConfigs.Configs;
using CoreUtils.Utils;
using Newtonsoft.Json;
using UnityEngine;
using Utils;
using Utils.SaveManager;

public class MercenariesController : MonoSingleton<MercenariesController>
{
    private MercenarySetConfig MercenarySet;
    private Dictionary<UnitConfig, int> _mercenariesInStock;
    
    protected override void Init()
    {
        base.Init();
        
        MercenarySet = ConfigBase.LoadFirstAvailableConfig<MercenarySetConfig>();
        LoadCollection();
    }

    private void LoadCollection()
    {
        var mercenaryData = SaveManager.GetValue(SavedDataManager.MercenariesCollectionKey, string.Empty);
        var mercCollection = mercenaryData == string.Empty
            ? new Dictionary<UnitConfig, int>()
            : ConvertMercenaryCollectionFromString(mercenaryData);
        
        foreach (var config in MercenarySet.Mercenaries)
            if (!mercCollection.ContainsKey(config.Config))
                mercCollection[config.Config] = 0;
        
        _mercenariesInStock = mercCollection;
        SaveCollection();
    }

    private static Dictionary<UnitConfig, int> ConvertMercenaryCollectionFromString(string mercenaryData)
    {
        var mercCollection = new Dictionary<UnitConfig, int>();
        
        var dataList = JsonConvert.DeserializeObject<Dictionary<string, int>>(mercenaryData);
        foreach (var data in dataList)
        {
            var config = ConfigBase.LoadConfig<UnitConfig>(data.Key);
            mercCollection[config] = data.Value;
        }
        return mercCollection;
    }
    
    private static string ConvertMercenaryCollectionToString(Dictionary<UnitConfig, int> mercenaryData)
    {
        var collection = new Dictionary<string, int>();
        foreach (var data in mercenaryData)
            collection[data.Key.Uid] = data.Value;

        return JsonConvert.SerializeObject(collection);
    }
    
    public void ResetRange(List<int> newRange)
    {
        var str = string.Empty;
        for (var i = 0; i < newRange.Count; i++)
            str += i == newRange.Count - 1
                ? $"{newRange[i]}"
                : $"{newRange[i]};";
            
        SaveManager.Add(SavedDataManager.ShopRangeOfMercenariesKey, str);
    }
    
    private void SaveCollection()
    {
        SaveManager.Add(SavedDataManager.MercenariesCollectionKey, ConvertMercenaryCollectionToString(_mercenariesInStock));
    }
    
    public void AddUnitsToStock(Mercenary unit)
    {
        var value = _mercenariesInStock[unit.Config] + unit.SaleAmount;
        _mercenariesInStock[unit.Config] = value;
        SaveCollection();
    }
    
    public int UnitInStockCount(UnitConfig config)
    {
        return _mercenariesInStock[config];
    }
    
    public void SubtractUnit(UnitConfig config, int count = 1)
    {
        var value =  _mercenariesInStock[config] - count;
        if (value < 0)
            value = 0;
        
        _mercenariesInStock[config] = value;
        SaveCollection();
        Debug.LogWarning($"Mercenary {config.name} removed");
    }
}
