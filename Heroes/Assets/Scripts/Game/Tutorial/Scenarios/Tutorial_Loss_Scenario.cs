using System;
using System.Collections;
using System.Collections.Generic;
using Configs;
using Game.Controllers;
using UnityEngine;
using Game.Tutorial.Lobby;
using UI;
using UI.UIWindows.DailyQuest;

public class Tutorial_Loss_Scenario : AbstractLobbyScenario
{
    [SerializeField] private DailyQuestConfig _tutorialBattle;
    
    [ContextMenu("Start Testing")]
    private void StartTesting()
    {
        CreateSteps();
        Step00();
    }
    
    public override void Play(bool onEnable)
    {
        base.Play(onEnable);
        CreateSteps();
        //GetCurrentStep();
    }
    
    protected override void CreateSteps()
    {
        Steps = new List<Action>();
        Steps.Add(Step00);
        Steps.Add(Step01);
        
        Step00();
    }

    private void Step00()
    {
        LobbyTutorialManager.Instance.Helper.FadeIn();
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(0), AnchorPresets.BottomLeft, Step01);
        LobbyTutorialManager.Instance.Helper.ShowHighlightMask(AnchorPresets.MiddleCenter, 
            new Vector2(390f, 280f), 540, 29);
    }

    private void Step01()
    {
        OnComplete();
        LobbyTutorialManager.Instance.Helper.FadeOut();
        LobbyTutorialManager.Instance.Helper.HideArrow();
        LobbyTutorialManager.Instance.Helper.DisableLock();
        LobbyTutorialManager.Instance.Helper.CloseDialog(false);

        DailyQuestManager.StartDailyQuest(_tutorialBattle);
    }
}
