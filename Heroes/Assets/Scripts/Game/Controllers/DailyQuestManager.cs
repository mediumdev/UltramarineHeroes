using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Configs;
using CoreConfigs.Configs;
using CoreUtils.Utils;
using Dynamic;
using Network;
using Utils;
using Utils.SaveManager;
using Random = System.Random;

namespace Game.Controllers
{
    public class DailyQuestManager : MonoSingleton<DailyQuestManager>
    {
        private List<DailyQuestConfig> _questConfigs = new List<DailyQuestConfig>();
        private DateTime _lastGeneration;
        private Random _rnd;

        protected override void Init()
        {
            base.Init();

            _rnd = new Random();

            var savedLastGen = SaveManager.GetValue(SavedDataManager.LastDailyQuestUpdateKey, string.Empty);
            _lastGeneration = savedLastGen == string.Empty
                ? DateTime.UtcNow
                : DateTime.Parse(savedLastGen, CultureInfo.InvariantCulture);

            var savedDailyQuests = SaveManager.GetValue(SavedDataManager.LastDailyQuestsKey, string.Empty);
            if (savedDailyQuests == string.Empty || DateTime.UtcNow.Date > _lastGeneration.Date)
            {
                _questConfigs = GenerateQuestList();
                _lastGeneration = DateTime.UtcNow;
                SaveManager.Add(SavedDataManager.FinishedDailyQuestsKey, "");
            }
            else
            {
                _questConfigs = savedDailyQuests.Split(';').Select(ConfigBase.LoadConfig<DailyQuestConfig>).ToList();
            }
            
            SaveManager.Add(SavedDataManager.LastDailyQuestUpdateKey, _lastGeneration.ToString(CultureInfo.InvariantCulture));
            SaveManager.Add(SavedDataManager.LastDailyQuestsKey, string.Join(";", _questConfigs.Select(x => x.Uid)));
        }

        private List<DailyQuestConfig> GenerateQuestList()
        {
            var questConfigsAll = ConfigBase.LoadAll<DailyQuestConfig>()
                .Where(x => !GameSettings.Instance.Settings.DailyQuestBlacklist.Contains(x)).ToList();
            var questsGenerated = new List<DailyQuestConfig>();
            for (var i = 0; i < Math.Min(3, questConfigsAll.Count); i++)
            {
                var config = questConfigsAll[_rnd.Next(questConfigsAll.Count)];
                while (questsGenerated.Contains(config))
                    config = questConfigsAll[_rnd.Next(questConfigsAll.Count)];
                
                questsGenerated.Add(config);
            }
            
            return questsGenerated;
        }

        public List<DailyQuestConfig> GetCurrentDailyQuests()
        {
            return _questConfigs;
        }

        public static void StartDailyQuest(DailyQuestConfig config)
        {
            SavedDataManager.SaveBotDeck(config.BotDeck);
            SavedDataManager.SavePlayerDeck(config.UserDeck);
            
            DynamicVarLibrary.Instance.AddVar(DynamicDataManager.CurrentDailyQuestKey, config.Uid);
            SavedDataManager.SetFightModeDailyQuest();
            DynamicDataManager.SetNextRewards(config.Rewards);
            
            NetworkLobbyController.LoadGameSceneBot(1);
        }
    }
}