using System;
using CoreUtils.Utils;
using UnityEngine;
using Utils;
using Utils.SaveManager;

public class DailyUpdateController : MonoSingleton<DailyUpdateController>
{
    private DateTime _lastMercenaryUpdateDate;

    protected override void Init()
    {
        base.Init();
        
        DontDestroyOnLoad(gameObject);
    }
    
    public bool MercenaryRangeNeedsUpdate()
    {
        var data = SaveManager.GetValue(SavedDataManager.LastMercenaryUpdateKey, string.Empty);
        if (data == string.Empty) return true;
        
        _lastMercenaryUpdateDate = DateTime.Parse(data);
        var nextUpdateDate = _lastMercenaryUpdateDate.Date.AddDays(1);
        Debug.Log($"Now {DateTime.UtcNow} >= AddDay {nextUpdateDate} : " + $"{DateTime.UtcNow >= nextUpdateDate}");
        return DateTime.UtcNow >= nextUpdateDate;
    }
}
