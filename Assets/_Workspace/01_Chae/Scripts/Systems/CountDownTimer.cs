using System;
using UnityEngine;
public class CountDownTimer {
    public float timeLeft {get; private set;}

    public bool isRunning {get; private set;}
    public event Action OnTimerEnd;

    public CountDownTimer(float duration){
        timeLeft = duration;
    }
    
    /// <summary>
    /// 타이머 생성 함수
    /// <br/>
    /// 사용법 : 전역변수 CountDownTimer 생성 후 State클래스 Entet함수 및 Start 함수 에서 Create함수 사용
    /// <br/> 사용예시 : <br/> 
    ///
    /// CountDownTimer myTimer; (전역) 
    /// <br/>
    /// public void Enter(){
    /// <br/>
    /// myTimer = CountDownTimer.Create(duration,() => boolean = true); }
    /// 
    /// <br/>
    /// 
    /// </summary>
    /// <param name="duration">쿨다운 타이머 시간</param>
    /// <param name="callBack">호출할 callback (주로 boolean)</param>
    /// <returns></returns>
    public static CountDownTimer Create(float duration, Action callBack){
        var timer = new CountDownTimer(duration);
        timer.OnTimerEnd += callBack;
        return  timer;
    }
    /// <summary>
    /// 타이머 설정 함수 (사용법은 Create와 동일함) <br/>
    /// 언제 사용하는지 : 이미 만들어진 타이머의 설정(시간이나 할 일)을 바꾸고 싶을 때 사용.<br/>
    /// ex) 아이템을 먹어서 대쉬 쿨다운을 줄일 필요가있을때
    /// 
    /// </summary>
    /// <param name="duration">쿨다운 타이머 시간</param>
    /// <param name="callBack">호출할 callback (주로 boolean)</param>
    public void SetTimer(float duration, Action callBack){
        timeLeft = duration;
        OnTimerEnd = null;
        OnTimerEnd += callBack;
    }

    /// <summary>
    /// 타이머 시작 : 타이머 작동이 필요할시 초기에 호출해야됨
    /// </summary>
    /// <param name="duration"></param>
    public void StartTimer(float duration){
        timeLeft = duration;
        isRunning = true;
    }

    /// <summary>
    /// 타이머의 시간을 멈추거나 재개
    /// </summary>
    public void ToggleTimer(){
        if(timeLeft > 0) isRunning = !isRunning;
    }

    /// <summary>
    /// 타이머의 시간을 흐르게 해주는 함수 : Update에서 호출이 필요합니다<br/>
    /// 인자는 Time.deltaTime 을 넣어주세요
    /// </summary>
    /// <param name="deltaTime"></param>
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
    /// <summary>
    /// 타이머 정지 isRunning을 false로 설정.
    /// </summary>
    public void StopTimer() => isRunning = false;
}