using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor.Callbacks;
using UnityEngine;

public class WallState : IState
{   
    Player player;
    CountDownTimer attatchTimer;
    bool canSlide = false;
    GameObject currentWall;
    float currentWallDir;

    public WallState(Player player){
        this.player = player;
        //Set Timer
        attatchTimer = new CountDownTimer(player.wallAttachTime);
        attatchTimer.OnTimerEnd += () => canSlide = true;
    }
    public void Enter(){
        Debug.Log("OnWall");

        //벽 감지;
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position,Vector2.right * player.dir.x,player.rayDistanceWall,player.wallLayer);
        if(hit.collider != null){
            currentWall = hit.collider.gameObject;
            currentWallDir = player.dir.x;
        }
        if(currentWall != player.lastWall)
            player.canWallJump = true;

        //Set Timer
        attatchTimer.Start(player.wallAttachTime);

        //Act
        player.AttatchWall();
    }
    public void Update(){
        attatchTimer.Tick(Time.deltaTime);
        
        //Wall Jump
        if(player.IsJumped()){
            // if(player.inputX == 0){
            //     player.EscapeWall();
            if(Mathf.Sign(player.inputX) != player.dir.x){
                player.JumpWall();
            }
            return;
        }

        //Attatch-Slide
        if(canSlide){
            player.SlideWall();
        }else{
            player.AttatchWall();
        }

    }
    public void Exit(){
        attatchTimer.Stop();
        player.SetGravity(1);
    }
}
