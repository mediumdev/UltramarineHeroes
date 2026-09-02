using DevToDev.Analytics;
using Utils;
using Utils.SaveManager;
using WebSocketSharp;

public static class DTDAnalyticsEvents
{
    private static string GetLastCampaign()
    {
        string lastCampaign = SaveManager.GetValue<string>(SavedDataManager.LastFinishedCampaignFightKey);
        if (lastCampaign.IsNullOrEmpty())
            lastCampaign = "0";

        return lastCampaign;
    }
    
    public static void FactionUpgrade(string factionName, int progress)
    {
        var parameters = new DTDCustomEventParameters();
        
        parameters.Add(key: "CurrentCampaignState", value: GetLastCampaign());
        parameters.Add(key: "Faction", value: factionName);
        parameters.Add(key: "ObtainedLevel", value: progress.ToString());
        DTDAnalytics.CustomEvent(eventName: "FactionUpgrade", parameters: parameters);
    }

    public static void FightCampaign(bool win, string fightUid)
    {
        var parameters = new DTDCustomEventParameters();
        
        parameters.Add(key: "Win", value: win);
        parameters.Add(key: "FightUid", value: fightUid);
        DTDAnalytics.CustomEvent(eventName: "FightCampaign", parameters: parameters);
    }
    
    public static void FightPvp(bool win)
    {
        var parameters = new DTDCustomEventParameters();
        
        parameters.Add(key: "Win", value: win);
        DTDAnalytics.CustomEvent(eventName: "FightPVP", parameters: parameters);
    }

    public static void Purchase(string storeKey, bool realCurrency)
    {
        var parameters = new DTDCustomEventParameters();

        parameters.Add(key: "CurrentCampaignState", value: GetLastCampaign());
        parameters.Add(key: "Product", value: $"{storeKey}");
        parameters.Add(key: "RealCurrency", value: realCurrency); 
        DTDAnalytics.CustomEvent(eventName: "Purchase", parameters: parameters);
    }
}
