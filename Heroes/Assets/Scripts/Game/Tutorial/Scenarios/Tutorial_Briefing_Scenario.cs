using System;
using System.Collections.Generic;
using com.adjust.sdk;
using DevToDev.Analytics;
using Game.Tutorial.Lobby; 
using UI;
using UI.UIWindows.Faction;
using UnityEngine;

public class Tutorial_Briefing_Scenario : AbstractLobbyScenario 
{
    private bool Faded =>  LobbyTutorialManager.Instance.Helper.Faded;

    [SerializeField] private Transform _towersContainer;
    [SerializeField] private Transform _midLineContainer;
    
#if UNITY_EDITOR
    [ContextMenu("Start Testing")]     
    private void StartTesting()   
    {
        //ADJUSTEVENT
        /*AdjustEvent tutorial_start = new AdjustEvent("ywgded");
        Adjust.trackEvent(tutorial_start);*/

        CreateSteps();     
        Step00();     
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
    }

    private void Step00()
    {
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(0), AnchorPresets.BottomRight, null);
        LobbyTutorialManager.Instance.Helper.ShowDefaultButton(AnchorPresets.MiddleCenter, 
            new Vector2(300f, 300f), new Vector2(-605f, 70f), -400f, Step01);
    }

    private void Step01()
    {
        // Смена фракции
        _towersContainer.GetChild(1).GetComponent<FactionTypeItem>().ChangeFaction(1);
        
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(1),AnchorPresets.BottomLeft, Step02);      
        LobbyTutorialManager.Instance.Helper.ShowDefaultButton(AnchorPresets.MiddleCenter, 
            new Vector2(265f, 300f), new Vector2(-103f, 78f), -400f, Step02);
        
        DTDAnalytics.Tutorial(8);
    }
    
    private void Step02()
    { 
        _midLineContainer.GetChild(1).GetComponent<FactionItem>().TryToSelect();
        DTDAnalytics.Tutorial(9);
        
        OnComplete();    
        LobbyTutorialManager.Instance.Helper.FadeOut();    
        LobbyTutorialManager.Instance.Helper.HideArrow();   
        LobbyTutorialManager.Instance.Helper.DisableLock();
        LobbyTutorialManager.Instance.Helper.HideHighlightMask();
        LobbyTutorialManager.Instance.Helper.CloseDialog(false);
    }
}
