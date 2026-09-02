using System;
using System.Collections.Generic;
using System.Linq;
using Configs;
using CoreConfigs.Configs;
using CoreUtils.Utils;
using UnityEngine;
using Utils;
using Utils.SaveManager;

namespace Game.Controllers
{
    public class CustomizationManager : MonoSingleton<CustomizationManager>
    {
        public string PlayerName { get; private set; }
        public Sprite CurrentAvatarImage { get; private set; }
        public List<UnitConfig> AllUnits { get; private set; }
        public List<Sprite> AllUnitsIcons { get; private set; }
        public List<UnitConfig> UnlockedUnits { get; private set; }
        public List<Sprite> UnlockedUnitsIcons { get; private set; }

        public event Action AvatarChangedEvent;
        public event Action PlayerNameChangedEvent;

        protected override void Init()
        {
            base.Init();

            AllUnits = PlayerFactionsController.Instance.FactionsList
                .SelectMany(x => x.FactionProgress)
                .SelectMany(x => x.unitsData)
                .Select(x => x.config).Distinct().ToList();
            AllUnitsIcons = AllUnits.Select(x => x.Icon).ToList();
            
            UnlockedUnits = PlayerFactionsController.Instance.FactionsList
                .SelectMany(x => PlayerFactionsController.Instance.GetFactionUnitsUnlocked(x)).ToList();
            UnlockedUnitsIcons = UnlockedUnits.Select(x => x.Icon).ToList();

            var playerNameSaved = SaveManager.GetValue(SavedDataManager.PlayerNameKey, string.Empty);
            PlayerName = playerNameSaved == string.Empty
                ? "Player"
                : playerNameSaved;

            var avatarImageSaved = SaveManager.GetValue(SavedDataManager.PlayerAvatarKey, string.Empty);
            SetAvatarImage(avatarImageSaved == string.Empty
                ? UnlockedUnits[0]
                : ConfigBase.LoadConfig<UnitConfig>(avatarImageSaved)
                );
        }

        public void SetAvatarImage(UnitConfig unitConfig)
        {
            if (!UnlockedUnits.Contains(unitConfig))
                Debug.LogWarning($"Юнит {unitConfig.name} отсутствует в списке разблокированных");
            
            CurrentAvatarImage = unitConfig.Icon;
            SaveManager.Add(SavedDataManager.PlayerAvatarKey, unitConfig.Uid);
            AvatarChangedEvent?.Invoke();
        }

        public Sprite GetAvatarImage()
        {
            return CurrentAvatarImage;
        }

        public void SetPlayerName(string newName)
        {
            PlayerName = newName;
            SaveManager.Add(SavedDataManager.PlayerNameKey, PlayerName);
            PlayerNameChangedEvent?.Invoke();
        }

        public string GetPlayerName()
        {
            return PlayerName;
        }

        public void InitRepaints()
        {
            AvatarChangedEvent?.Invoke();
            PlayerNameChangedEvent?.Invoke();
        }
    }
}