using System.Collections.Generic;
using System.Linq;
using Configs;
using CoreConfigs.Configs;
using CoreUtils.Utils;
using Enums;
using Game.Controllers;
using Game.Tutorial.Lobby;
using UI.Windows;
using UnityEngine;
using Utils;
using Utils.SaveManager;

namespace UI.UIWindows.Lobby
{
    public class LobbyWindow : Window
    {
        [SerializeField] private LobbyTower _typeItem;
        [SerializeField] private FactionConfig[] _factions;
        [SerializeField] private Transform _typeContainer;
        [SerializeField] private Tutorial_Win_Scenario_Part1 _afterBattleSuccessTutorial_1;
        [SerializeField] private Tutorial_Win_Scenario_Part2 _afterBattleSuccessTutorial_2;
        [SerializeField] private Tutorial_Loss_Scenario _afterBattleFailTutorial;
        [SerializeField] private ConfigLibrary _configLibrary;

        private bool _bot;
        private readonly Dictionary<LineType, string> _factionUids = new Dictionary<LineType, string>();
        private readonly Dictionary<LineType, List<string>> _unitUidsByLine = new Dictionary<LineType, List<string>>();

        private static List<FactionConfig> AirFactions => FactionsController.Instance.AirFactions;
        private static List<FactionConfig> GroundFactions => FactionsController.Instance.GroundFactions;
        private static List<FactionConfig> UndergroundFactions => FactionsController.Instance.UndergroundFactions;

        public FactionConfig[] Factions => _factions;

        private void OnEnable()
        {
            if (!ConfigLibrary.Loaded)
                ConfigLibrary.LoadLibrary(_configLibrary);

            CreateLists();
        }

        public void LoadTutorial()
        {
            var firstBattleEnded = SaveManagerSafe.GetValue(SavedDataManager.FirstBattleEndedKey, false);
            var firstBattleWin = SaveManagerSafe.GetValue(SavedDataManager.FirstBattleWinKey, false);

            if (!firstBattleEnded) return;
            if (_afterBattleSuccessTutorial_1 == null || _afterBattleSuccessTutorial_2 == null 
            || _afterBattleFailTutorial == null) 
                return;

            if (firstBattleWin)
            {
                if (!LobbyTutorialManager.Instance.IsCompleted(_afterBattleSuccessTutorial_1.Name))
                    _afterBattleSuccessTutorial_1.enabled = true;
                else if (!LobbyTutorialManager.Instance.IsCompleted(_afterBattleSuccessTutorial_2.Name))
                    _afterBattleSuccessTutorial_2.enabled = true;
            }
            else
                _afterBattleFailTutorial.enabled = true;
        }

        private void CreateLists()
        {
            _typeContainer.Clear();
            var factionUidList = FactionsController.Instance.FactionUidsByPlayer[_bot];

            var item = Instantiate(_typeItem, _typeContainer);
            item.Init(AirFactions, this, factionUidList, LineType.Air);
            item = Instantiate(_typeItem, _typeContainer);
            item.Init(GroundFactions, this, factionUidList, LineType.Ground);
            item = Instantiate(_typeItem, _typeContainer);
            item.Init(UndergroundFactions, this, factionUidList, LineType.Underground);
        }
        
        public void ChangeFaction(LineType factionLine, FactionConfig faction)
        {
            if (!_factionUids.ContainsKey(factionLine)) 
                _factionUids[factionLine] = string.Empty;
            _factionUids[factionLine] = faction.Uid;
            
            if (!_unitUidsByLine.ContainsKey(factionLine))
                _unitUidsByLine.Add(factionLine, new List<string>());
            _unitUidsByLine[factionLine].Clear();
            foreach (var unitConfig in PlayerFactionsController.Instance.GetFactionUnits(faction, !_bot))
                _unitUidsByLine[factionLine].Add(unitConfig.Uid);
            _unitUidsByLine[factionLine] = _unitUidsByLine[factionLine].Take(3).ToList();

            FactionsController.Instance.SaveFactionCollectionData(_bot, _factionUids, _unitUidsByLine);
        }
    }
}