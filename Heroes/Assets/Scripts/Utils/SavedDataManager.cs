using System.Collections.Generic;
using System.Linq;
using Configs;
using CoreConfigs.Configs;
using SoundPool;

namespace Utils
{
    public static class SavedDataManager
    {
        public const string BotCollectionKey = "BotCollection";
        public const string BotFactionsKey = "BotFactions";
        public const string PlayerFactionsKey = "PlayerFactions";
        public const string PlayerCollectionKey = "PlayerCollection";
        
        public const string BotLevelKey = "BotLevel";
        public const string GameModeKey = "GameMode";
        private const string FightModeKey = "FightMode";
        
        public const string FactionProgressKey = "FactionProgress";
        
        public const string LastMercenaryUpdateKey = "LastMercenaryUpdate";
        public const string PurchasedMercenariesKey = "PurchasedMercenaries";
        public const string ShopRangeOfMercenariesKey = "ShopRangeOfMercenaries";
        public const string MercenariesCollectionKey = "MercenariesCollection";
        
        public const string AccountUnitLimitsInitializedKey = "AccountUnitsInitialized";
        public const string PlayerUnitsCountKey = "PlayerUnitsCount";
        public const string LastUnitLimitGenerationKey = "LastGeneration";
        
        public const string LastDailyQuestUpdateKey = "LastDailyQuestUpdate";
        public const string LastDailyQuestsKey = "LastDailyQUests";

        public const string PlayerNameKey = "PlayerName";
        public const string PlayerAvatarKey = "PlayerAvatar";

        public const string GameModeSingle = "single";
        public const string GameModePvp = "pvp";
        public const string FightModePvp = "PvpGame";
        public const string FightModeCampaign = "CampaignGame";
        public const string FightModeDailyQuest = "DailyQuest";
        
        public const string FinishedCampaignFightsKey = "FinishedCampaignFights";
        public const string LastFinishedCampaignFightKey = "LastFinishedCampaignFight";
        public const string FinishedDailyQuestsKey = "FinishedDailyQuests";
        
        public const string FirstBattleEndedKey = "FirstBattleEnded";
        public const string FirstBattleWinKey = "FirstBattleWin";

        private const string MusicSettingsKey = "MusicEnabled";
        private const string SoundSettingsKey = "SoundEnabled";

        public static bool IsSoundEnabled()
        {
            return SaveManager.SaveManager.GetValue(SoundSettingsKey, true);
        }

        public static void SetSoundEnabled(bool enabled)
        {
            SoundManager.Instance.SoundEnabled = enabled;
            SaveManager.SaveManager.Add(SoundSettingsKey, enabled);
        }

        public static bool IsMusicEnabled()
        {
            return SaveManager.SaveManager.GetValue(MusicSettingsKey, true);
        }

        public static void SetMusicEnabled(bool enabled)
        {
            SaveManager.SaveManager.Add(MusicSettingsKey, enabled);
        }

        public static void SetFightModePvp()
        {
            SaveFightMode(FightModePvp);
        }

        public static void SetFightModeCampaign()
        {
            SaveFightMode(FightModeCampaign);
        }

        public static void SetFightModeDailyQuest()
        {
            SaveFightMode(FightModeDailyQuest);
        }
        
        public static string GetFightMode()
        {
            return SaveManager.SaveManager.GetValue(FightModeKey, string.Empty);
        }

        private static void SaveFightMode(string fightMode)
        {
            SaveManager.SaveManager.Add(FightModeKey, fightMode);
        }

        public static void SaveBotDeck(DeckConfig deckConfig)
        {
            var factionString = StringUtils.DeckFactionsToString(deckConfig);
            var collectionString = StringUtils.DeckCollectionToString(deckConfig);

            SaveManager.SaveManager.Add(BotFactionsKey, factionString);
            SaveManager.SaveManager.Add(BotCollectionKey, collectionString);
        }

        public static void SavePlayerDeck(DeckConfig deckConfig)
        {
            var factionString = StringUtils.DeckFactionsToString(deckConfig);
            var collectionString = StringUtils.DeckCollectionToString(deckConfig);

            SaveManager.SaveManager.Add(PlayerFactionsKey, factionString);
            SaveManager.SaveManager.Add(PlayerCollectionKey, collectionString);
        }

        public static List<string> GetPlayerDeckUids()
        {
            return SaveManager.SaveManager.GetValue<string>(PlayerFactionsKey).Split(';').ToList();
        }

        public static string[] GetFinishedCampaignFights()
        {
            return SaveManager.SaveManager.GetValue<string>(FinishedCampaignFightsKey).Split(';');
        }
    }
}