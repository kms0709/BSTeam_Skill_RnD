using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor.Callbacks;
using UnityEngine;

public class WallState : IState
{   
    Player player;
    CountDownTimer timer;
    bool canSlide = false;

    public WallState(Player player){
        this.player = player;
        timer = new CountDownTimer(player.wallAttachTime);
        timer.OnTimerEnd += () => canSlide = true;
    }
    public void Enter(){
        Debug.Log("OnWall");
        timer.Start(player.wallAttachTime);
        player.AttatchWall();
    }
    public void Update(){
        timer.Tick(Time.deltaTime);
        if(!player.IsGrounded()){
            if(canSlide){
                player.SlideWall();
            }else{
                player.AttatchWall();
            }
            if(player.IsJumped()){
                player.JumpWall();
            }
        }
    }
    public void Exit(){
        timer.Stop();
        player.SetGravity(1);
    }
}
