using System;
using System.Collections;
using CoreUtils.Utils;
using UnityEngine;

public class TickController : MonoSingleton<TickController>
{
    public event Action SecondsTickEvent;
    public event Action MinutesTickEvent;
    
    private Coroutine secondsTick;
    private Coroutine minutesTick;
    
    protected override void Init()
    {
        base.Init();
        
        DontDestroyOnLoad(gameObject);

        StartTicks();
    }

    public void StartTicks()
    {
        if (secondsTick == null)
            secondsTick = StartCoroutine(SecondsTick());
        
        if (minutesTick == null)
            minutesTick = StartCoroutine(MinutesTick());
    }

    private IEnumerator SecondsTick()
    {
        yield return new WaitForSeconds(1);
        
        SecondsTickEvent?.Invoke();
        secondsTick = StartCoroutine(SecondsTick());
    }

    private IEnumerator MinutesTick()
    {
        yield return new WaitForSeconds(60);
        
        MinutesTickEvent?.Invoke();
        minutesTick = StartCoroutine(MinutesTick());
    }
}