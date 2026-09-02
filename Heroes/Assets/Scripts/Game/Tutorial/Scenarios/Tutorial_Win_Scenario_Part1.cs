using System;
using System.Collections.Generic;
using DevToDev.Analytics;
using Game.Tutorial.Lobby;
using UI;
using UI.UIWindows.Faction;
using UI.UIWindows.Lobby;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_Win_Scenario_Part1 : AbstractLobbyScenario
{
    [Header("Elements")] 
    [SerializeField] private Button _casernButton;
    [SerializeField] private OpenFactionWindow _window;
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
        Steps.Add(Step04);
        Steps.Add(Step05);
        Steps.Add(Step06);
        Steps.Add(Step07);
    }

    private void Step00() //На ресурсы
    {
        LobbyTutorialManager.Instance.Helper.FadeIn();
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(0), AnchorPresets.BottomLeft, Step01);
        LobbyTutorialManager.Instance.Helper.ShowHighlightMask(AnchorPresets.TopRight, new Vector2(282f, 230f), -141, -115);
        LobbyTutorialManager.Instance.Helper.ShowArrow(AnchorPresets.TopRight, new Vector2(-140, -330), new Vector3(0f,0f,180f));
    }

    private void Step01() //На казарму
    {
        CompleteStep(1);
        if (!Faded)
            LobbyTutorialManager.Instance.Helper.FadeIn();
        
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(1),AnchorPresets.BottomLeft, Step02);
        LobbyTutorialManager.Instance.Helper.ShowHighlightMask(AnchorPresets.MiddleCenter, 
            new Vector2(380f, 320f), 16, 24);
        LobbyTutorialManager.Instance.Helper.ShowButton(_casernButton, Step02, -285f);
        DTDAnalytics.Tutorial(3);
    }
    
    private void Step02() //На прокачку казармы 
    {
        CompleteStep(2);
        
        if (!Faded)
            LobbyTutorialManager.Instance.Helper.FadeIn();
        
        _window.Init();
        _factionCastleWindow = FindObjectOfType<FactionCastleWindow>();
        
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(2),AnchorPresets.BottomLeft, Step03);
        LobbyTutorialManager.Instance.Helper.ShowHighlightMask(AnchorPresets.BottomRight, 
            new Vector2(385f, 175f), -192, 87);
        LobbyTutorialManager.Instance.Helper.ShowDefaultButton(AnchorPresets.MiddleCenter, 
            new Vector2(350f, 140f), new Vector2(890f, -450f), -390f, Step03);
        DTDAnalytics.Tutorial(4);
    }

    private void Step03() //На прокачку казармы 
    {
        CompleteStep(3);
        if (!Faded)
            LobbyTutorialManager.Instance.Helper.FadeIn();
        if (_factionCastleWindow == null)
        {
            _window.Init();
            _factionCastleWindow = FindObjectOfType<FactionCastleWindow>();
        }
        _factionCastleWindow.OpenUpgradeWindow();
        
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(3), AnchorPresets.BottomLeft,null);
        LobbyTutorialManager.Instance.Helper.ShowHighlightMask(AnchorPresets.MiddleCenter, 
            new Vector2(303f, 102f), -2, -364);
        LobbyTutorialManager.Instance.Helper.ShowDefaultButton(AnchorPresets.MiddleCenter, 
            new Vector2(410f, 110f), new Vector2(0f, -355f), -400f, Step04);
        DTDAnalytics.Tutorial(5);
    }
    
    private void Step04() 
    {
        CompleteStep(5);
        
        if (!Faded)
            LobbyTutorialManager.Instance.Helper.FadeIn();
        
        var upgradeWindow = FindObjectOfType<UpgradeWindow>();
        upgradeWindow.DoUpgrade();

        LobbyTutorialManager.Instance.Helper.FadeOut();
        LobbyTutorialManager.Instance.Helper.ShowButtonWithoutArrow(AnchorPresets.MiddleCenter, 
            new Vector2(230f, 190f), new Vector2(-980f, -450f), false, Step05);
        LobbyTutorialManager.Instance.Helper.DisableLock();
        LobbyTutorialManager.Instance.Helper.CloseDialog(true);
        
        upgradeWindow.transform.SetAsLastSibling();
    }

    private void Step05() //На стрелочки для смены казармы
    {
        if (_factionCastleWindow != null)
            _factionCastleWindow.CloseWindow();
        
        if (!Faded)
            LobbyTutorialManager.Instance.Helper.FadeIn();
        
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(4),AnchorPresets.BottomLeft, Step06);
        LobbyTutorialManager.Instance.Helper.ShowHighlightMask(AnchorPresets.MiddleCenter, new Vector2(600f, 131f), 16, -11);
        DTDAnalytics.Tutorial(6);
    }
    
    private void Step06() //На арену
    {
        CompleteStep(6);
        if (!Faded)
            LobbyTutorialManager.Instance.Helper.FadeIn();
        
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(5),AnchorPresets.BottomLeft, Step07);
        LobbyTutorialManager.Instance.Helper.ShowHighlightMask(AnchorPresets.MiddleCenter, 
            new Vector2(390f, 280f), 540, 29);
        LobbyTutorialManager.Instance.Helper.ShowDefaultButton(AnchorPresets.MiddleCenter, 
            new Vector2(450f, 340f), new Vector2(540f, 29f), -300f, Step07);
    }
    
    private void Step07() 
    {
        CompleteStep(8);
        DTDAnalytics.Tutorial(7);
        _controller.PlayArena();
        
        OnComplete();
        LobbyTutorialManager.Instance.Helper.FadeOut();
        LobbyTutorialManager.Instance.Helper.HideArrow();
        LobbyTutorialManager.Instance.Helper.DisableLock();
        LobbyTutorialManager.Instance.Helper.CloseDialog(false);
    }
}
