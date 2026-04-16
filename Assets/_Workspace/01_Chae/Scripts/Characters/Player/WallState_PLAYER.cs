using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WallState_PLAYER : IState
{   
    Player player;
    CountDownTimer attatchTimer;
    bool canSlide = false;
    float currentWallDir;

    public WallState_PLAYER(Player player){
        this.player = player;
        //Set Timer
        attatchTimer = new CountDownTimer(player.wallAttachTime);
        attatchTimer.OnTimerEnd += () => canSlide = true;
    }
    public void Enter(){
        Debug.Log("OnWall");

        //현재 벽 방향 저장
        currentWallDir = player.dirX;

        //Set Timer
        attatchTimer.Start(player.wallAttachTime);

        //Act
        player.AttatchWall();
    }
    public void Update(){
        attatchTimer.Tick(Time.deltaTime);
        
        // 1. 이미 벽점프를 실행했다면(딜레이 진행 중), 벽에서 빠져나갈 때까지 아래 슬라이드 로직을 무시
        if (player.isWallJumping) {
            return;
        }

        //Wall Jump
        // Mathf.Sign 버그 방지 및 반대 방향 입력 확인
        bool isPressingAway = (player.dirX > 0 && player.inputX < 0) || (player.dirX < 0 && player.inputX > 0);
        if(player.lastWallDir != currentWallDir){
            if(player.IsJumped() && isPressingAway){
                player.JumpWall();
                return;
            }else{
                if(canSlide){
                    player.SlideWall();
                }else{
                    player.AttatchWall();
                }
            }
        }else{
            player.SlideWall();
        }
        // if(player.IsJumped()){
        //     if(player.lastWallDir != currentWallDir && isPressingAway){
        //         player.JumpWall();
        //         // 2. 점프가 '성공'했을 때만 return 하여 바로 아래의 SlideWall/AttatchWall 실행을 막음
        //         return; 
        //     }else{
        //         canSlide = true;
        //     }
        // }

        // //Attatch-Slide
        // if(canSlide){
        //     player.SlideWall();
        // }else{
        //     player.AttatchWall();
        // }
    }
    public void Exit(){
        attatchTimer.Stop();
        player.SetGravity(1);
    }
}
