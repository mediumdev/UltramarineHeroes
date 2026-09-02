using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using CoreConfigs.Configs;
using Dynamic;
using SoundPool;
using UnityEngine;

namespace Utils
{
    public static class DynamicDataManager
    {
        private const string NextRewardKey = "NextReward";
        
        public const string CurrentCampaignFightKey = "CurrentCampaignFightUid";
        public const string CurrentDailyQuestKey = "CurrentDailyQuestUid";

        private const string MusicSettingsKey = "MusicEnabled";
        private const string SoundSettingsKey = "SoundEnabled";
        private const string PreviousSceneName = "PreviousSceneName";

        public static void SetPreviousScene(string scene)
        {
            DynamicVarLibrary.Instance.AddVar(PreviousSceneName, scene);
        }

        public static string GetPreviousScene()
        {
            return DynamicVarLibrary.Instance.GetVar(PreviousSceneName);
        }

        public static bool IsMusicEnabled()
        {
            var enabled = DynamicVarLibrary.Instance.GetVar(MusicSettingsKey);
            if (enabled == string.Empty)
            {
                enabled = SavedDataManager.IsMusicEnabled().ToString();
                DynamicVarLibrary.Instance.AddVar(MusicSettingsKey, enabled);
            }

            return enabled == true.ToString();
        }

        public static void SetMusicEnabled(bool enabled)
        {
            SavedDataManager.SetMusicEnabled(enabled);
            DynamicVarLibrary.Instance.AddVar(MusicSettingsKey, enabled.ToString());
        }

        public static bool IsSoundEnabled()
        {
            var enabled = DynamicVarLibrary.Instance.GetVar(SoundSettingsKey);
            if (enabled == string.Empty)
            {
                enabled = SavedDataManager.IsSoundEnabled().ToString();
                DynamicVarLibrary.Instance.AddVar(SoundSettingsKey, enabled);
            }

            return enabled == true.ToString();
        }

        public static void SetSoundEnabled(bool enabled)
        {
            SoundManager.Instance.SoundEnabled = enabled;
            SavedDataManager.SetSoundEnabled(enabled);
            DynamicVarLibrary.Instance.AddVar(SoundSettingsKey, enabled.ToString());
        }
        
        public static void SetNextRewards(List<RewardContainerConfig> rewardsList)
        {
            DynamicVarLibrary.Instance.AddVar(
                NextRewardKey, 
                string.Join(";", rewardsList.Select(x => x.Uid))
                );
        }

        public static List<RewardContainerConfig> GetNextRewards()
        {
            Debug.Log($"Get DynamicVar NextReward {DynamicVarLibrary.Instance.GetVar(NextRewardKey)}");

            var rewardUidList = DynamicVarLibrary.Instance.GetVar(NextRewardKey).Split(';');
            return rewardUidList.Select(ConfigBase.LoadConfig<RewardContainerConfig>).ToList();
        }
    }
}