using System.Collections;
using System.Collections.Generic;
using Configs;
using Network;
using UnityEngine;
using Utils;
using Random = System.Random;

namespace Game
{
    public class GameStartController : MonoBehaviour
    {
        [SerializeField] private NetworkLobbyController _networkLobbyController;
        [SerializeField] private List<DeckConfig> _decks;
        [SerializeField] private GameObject _screen;
        [SerializeField] private Tutorial_Briefing_Scenario _tutorial;
        
        private readonly Random _rnd = new Random();
        public float _duration;

        public void Start()
        {
            if (_tutorial != null)
                _tutorial.enabled = true;
        }
        
        public void StartGame()
        {
            var fightMode = SavedDataManager.GetFightMode();
            
            switch (fightMode)
            {
                case SavedDataManager.FightModePvp:
                    var deckIdx = _rnd.Next(_decks.Count);
                    SavedDataManager.SaveBotDeck(_decks[deckIdx]);
                    
                    _screen.SetActive(true);
                    StartCoroutine(Coroutine());
                    
                    break;
                case SavedDataManager.FightModeCampaign:
                    break;
                case SavedDataManager.FightModeDailyQuest:
                    break;
                default:
                    Debug.LogError($"Unknown FightMode {fightMode}");
                    return;
            }
            
            if (fightMode != SavedDataManager.FightModePvp)
                NetworkLobbyController.LoadGameSceneBot(1);
        }

        private IEnumerator Coroutine()
        {
          yield return new WaitForSeconds(_duration);
          NetworkLobbyController.LoadGameSceneBot(1);
        }
        
    }
}