using System;
using System.Collections.Generic; 
using DevToDev.Analytics;
using Game.Battle;
using Game.Tutorial.Lobby; 
using UI; 
using UnityEngine;  

public class Tutorial_Battle_Scenario : AbstractLobbyScenario 
{
    [SerializeField] private GameMachine _gameMachine;
    [SerializeField] private UnitCardMenu _cardMenu;
    
#if UNITY_EDITOR
    [ContextMenu("Start Testing")]     
    private void StartTesting()   
    { 
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
        //Step00();
    }

    private void Step00()
    {
        _gameMachine.GamePause();
        
        LobbyTutorialManager.Instance.Helper.FadeIn(); 
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(0),AnchorPresets.BottomRight, Step01);       
        LobbyTutorialManager.Instance.Helper.ShowHighlightMask(AnchorPresets.MiddleCenter,             
            new Vector2(240f, 235f), -684, 105);        
        LobbyTutorialManager.Instance.Helper.ShowDefaultButton(AnchorPresets.MiddleCenter,              
            new Vector2(240f, 235f), new Vector2(-684f, 105f), -400f, Step01);
        DTDAnalytics.Tutorial(-1);
    }

    private void Step01()
    {
        // Открывание башни 
        if (_cardMenu != null)
            _cardMenu.CreateMenu(1);
        
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(1),AnchorPresets.BottomRight, Step02);      
        LobbyTutorialManager.Instance.Helper.ShowHighlightMask(AnchorPresets.BottonCenter,             
            new Vector2(186f, 199f), -106, 128);        
        LobbyTutorialManager.Instance.Helper.ShowDefaultButton(AnchorPresets.BottonCenter, 
            new Vector2(186f, 199f), new Vector2(-106f, 128f), -400f, Step02);
        DTDAnalytics.Tutorial(1);
    }

    private void Step02()
    {
        _gameMachine.GameContinue();
        
        // Спавн юнита
        _cardMenu.transform.GetComponentsInChildren<UnitIconItem>()[0].SpawnClick();
        
        // Указатель на флаг
        LobbyTutorialManager.Instance.Helper.OpenDialog(GetText(2),AnchorPresets.BottomRight, Step03);
        LobbyTutorialManager.Instance.Helper.ShowHighlightMask(AnchorPresets.MiddleCenter,             
            new Vector2(190f, 190f), -10, 110); 
        LobbyTutorialManager.Instance.Helper.ShowArrow(AnchorPresets.MiddleCenter, 
            new Vector2(-14.8f, 265), Vector3.zero);
        DTDAnalytics.Tutorial(2);
    }

    private void Step03()
    {
        OnComplete();    
        LobbyTutorialManager.Instance.Helper.FadeOut();    
        LobbyTutorialManager.Instance.Helper.HideArrow();   
        LobbyTutorialManager.Instance.Helper.DisableLock();
        LobbyTutorialManager.Instance.Helper.HideHighlightMask();
        LobbyTutorialManager.Instance.Helper.CloseDialog(false);
    }
}
