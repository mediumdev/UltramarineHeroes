using System;
using System.Collections.Generic;
using Configs;
using CoreConfigs.Configs;
using CoreUtils.Utils;
using Enums;
using Utils;
using Utils.SaveManager;

namespace Game.Controllers
{
    public class FactionsController: MonoSingleton<FactionsController>
    {
        public List<FactionConfig> AirFactions { get; private set; }
        public List<FactionConfig> GroundFactions { get; private set; }
        public List<FactionConfig> UndergroundFactions { get; private set; }
        public FactionConfig[] FactionsList { get; private set; }
        public Dictionary<bool, string[]> FactionUidsByPlayer { get; private set; }

        private static string GetFactionKey(bool bot)
        {
            return bot ? SavedDataManager.BotFactionsKey : SavedDataManager.PlayerFactionsKey;
        }
        
        private static string GetCollectionKey(bool bot)
        {
            return bot ? SavedDataManager.BotCollectionKey : SavedDataManager.PlayerCollectionKey;
        }

        protected override void Init()
        {
            base.Init();

            SetFactionsList();
            ParseCollection();
        }

        private void SetFactionsList()
        {
            FactionsList = new FactionConfig[]{};
            FactionsList = ConfigBase.LoadAll<FactionConfig>();
            SetFactionsByLineType();
        }

        private void ParseCollection()
        {
            FactionUidsByPlayer = new Dictionary<bool, string[]>
            {
                [true] = SaveManager.GetValue(GetFactionKey(true), string.Empty).Split(';'),
                [false] = SaveManager.GetValue(GetFactionKey(false), string.Empty).Split(';')
            };
        }

        public void SaveFactionCollectionData(bool bot, Dictionary<LineType, string> factionUids,
            Dictionary<LineType, List<string>> unitUidsByLine)
        {
            var factionString = string.Empty;
            foreach (var factionData in factionUids)
                factionString += factionData.Value + ";";
            SaveManager.Add(GetFactionKey(bot), factionString);
            
            var collectionString = string.Empty;
            foreach (var collectionData in unitUidsByLine)
                foreach (var collectionUid in collectionData.Value)
                    collectionString += collectionUid + ";";
            SaveManager.Add(GetCollectionKey(bot), collectionString);

            ParseCollection();
        }

        private void SetFactionsByLineType()
        {
            AirFactions = new List<FactionConfig>();
            GroundFactions = new List<FactionConfig>();
            UndergroundFactions = new List<FactionConfig>();

            foreach (var faction in FactionsList)
            {
                switch (faction.FactionType)
                {
                    case LineType.Air:
                        AirFactions.Add(faction);
                        break;
                    case LineType.Ground:
                        GroundFactions.Add(faction);
                        break;
                    case LineType.Underground:
                        UndergroundFactions.Add(faction);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}