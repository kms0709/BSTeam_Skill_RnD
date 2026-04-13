using System;
using UnityEngine;
public class CountDownTimer {
    public float timeLeft {get; private set;}
    public bool isRunning {get; private set;}
    public event Action OnTimerEnd;

    public CountDownTimer(float duration){
        timeLeft = duration;
    }

    public void Start(float duration){
        timeLeft = duration;
        isRunning = true;
    }
    public void Tick(float deltaTime){
        if(!isRunning) return;
        if(timeLeft > 0){
            timeLeft -= deltaTime;
        }else{
            timeLeft = 0;
            isRunning = false;
            OnTimerEnd?.Invoke();
        }
    }
    public void Stop() => isRunning = false;
}