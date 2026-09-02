using System.Collections;
using System.Collections.Generic;
using Configs;
using CoreConfigs.Configs;
using Dynamic;
using Game.Controllers;
using TMPro;
using UI.UIWindows.ChangeAvatar;
using UI.UIWindows.Lobby;
using UnityEngine;
using Utils;
using Utils.SaveManager;
using Random = System.Random;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private ConfigLibrary _configLibrary;
    [SerializeField] private LobbyWindow _lobby;
    [SerializeField] private LobbyWindowPlayerData _lobbyPlayerData;
    [SerializeField] private float _loadingTimeBase = 1f;
    [SerializeField] private TextMeshProUGUI _textField;
    [SerializeField] private List<string> _loadingPhrases;
    [SerializeField] private DailyQuestConfig _tutorialBattle;
    [SerializeField] private AudioSource _lobbySound;

    private Random _rnd = new Random();

    private void Awake()
    {
        if (!ConfigLibrary.Loaded)
            ConfigLibrary.LoadLibrary(_configLibrary);
        
        GameSettings.Instance.InitSettings();
    }

    private void OnEnable()
    {
        if (!DynamicDataManager.IsMusicEnabled()) _lobbySound.Pause();
        
        TickController.Instance.StartTicks();
        PlayerFactionsController.Instance.LoadPlayerFactions();
        CurrencyManager.Instance.LoadDataFromFile();
        UnitLimitManager.Instance.LoadDataFromFile();
        _lobbyPlayerData.Init();
        CustomizationManager.Instance.InitRepaints();

        StartCoroutine(GameLoadingProgress());
    }

    private IEnumerator GameLoadingProgress()
    {
        if (DynamicVarLibrary.Instance.GetVar("GameLoadingState") != "Started")
        {
            DynamicVarLibrary.Instance.AddVar("GameLoadingState", "Started");

            var baseTime = _loadingTimeBase + (float)_rnd.NextDouble() * 0.3f;
            if (_textField is null)
                yield return new WaitForSeconds(baseTime);
            else
            {
                var midTime = baseTime / _loadingPhrases.Count;
                var timeWasted = 0f;
                foreach (var phrase in _loadingPhrases)
                {
                    _textField.text = phrase;
                    var timeForStep = midTime + (float)_rnd.NextDouble() * 0.15f;
                    if (timeWasted + timeForStep > baseTime)
                        timeForStep = baseTime - timeWasted;
                    yield return new WaitForSeconds(timeForStep);
                    timeWasted += timeForStep;
                }
            }
        }
        
        StartGame();
    }

    private void StartGame()
    {
        if (!SaveManagerSafe.GetValue(SavedDataManager.FirstBattleEndedKey, false))
            if (_tutorialBattle != null)
                DailyQuestManager.StartDailyQuest(_tutorialBattle);
        
        if (_lobby != null)
            _lobby.LoadTutorial();
        
        gameObject.SetActive(false);
    }
}