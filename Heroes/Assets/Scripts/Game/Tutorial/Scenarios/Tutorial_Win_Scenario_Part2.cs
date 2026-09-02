using System;
using System.Collections.Generic;
using com.adjust.sdk;
using DevToDev.Analytics;
using Game.Tutorial.Lobby;
using UI;
using UI.UIWindows.Faction;
using UnityEngine;

public class Tutorial_Win_Scenario_Part2 : AbstractLobbyScenario
{
    [Header("Elements")]
    [SerializeField] private SceneController _controller;
    
    private FactionCastleWindow _factionCastleWindow;
    private bool Faded =>  LobbyTutorialManager.Instance.Helper.Faded;
    
#if UNITY_EDITOR
    [ContextMenu("Start Testing")]
    private void StartTesting()
    {
        CreateSteps();
        Step00();
    }
    
    [ContextMenu("Continue Testing")]
    private void ContinueTesting()
    {
        CreateSteps();
        GetCurrentStep();
    }
#endif

    public override void Play(bool onEnable)
    {
        base.Play(onEnable);
        CreateSteps();
        GetCurrentStep();
    }
    
    protected override void CreateSteps()
    {
        Steps = new List<Action>();
        Steps.Add(Step00);
        Steps.Add(Step01);
        Steps.Add(Step02);
        Steps.Add(Step03);
    }
    
    private void Step00() //На кампанию
    {
        if (!Faded)
            LobbyTutorialManager.Instance.Helper.FadeIn();
        
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(6),AnchorPresets.BottomRight, Step01);
        LobbyTutorialManager.Instance.Helper.ShowHighlightMask(AnchorPresets.MiddleCenter, 
            new Vector2(390f, 280f), -540, 29);
        LobbyTutorialManager.Instance.Helper.ShowDefaultButton(AnchorPresets.MiddleCenter, 
            new Vector2(450f, 340f), new Vector2(-540f, 29f), -300f, Step01);
    }
    
    private void Step01() 
    {
        CompleteStep(2);
        DTDAnalytics.Tutorial(10);
        _controller.PlayStory();
    }

    private void Step02() //На дейлики
    {
        if (!Faded)
            LobbyTutorialManager.Instance.Helper.FadeIn();
        
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(7),AnchorPresets.BottomRight, Step03);
        LobbyTutorialManager.Instance.Helper.ShowHighlightMask(AnchorPresets.MiddleCenter, 
            new Vector2(255f, 200f), -540, -349);
        
        LobbyTutorialManager.Instance.Helper.ShowDefaultButton(AnchorPresets.MiddleCenter, 
            new Vector2(450f, 340f), new Vector2(-540, -340), -300f, Step03);
    }

    private void Step03()
    {
        DTDAnalytics.Tutorial(-2);
        OnComplete();
        LobbyTutorialManager.Instance.Helper.FadeOut();
        LobbyTutorialManager.Instance.Helper.HideArrow();
        LobbyTutorialManager.Instance.Helper.DisableLock();
        LobbyTutorialManager.Instance.Helper.CloseDialog(false);

        //ADJUSTEVENT
        AdjustEvent tutorial_finish = new AdjustEvent("95w905");
        Adjust.trackEvent(tutorial_finish);

        _controller.OpenDailyQuestWindow();
    }
}
