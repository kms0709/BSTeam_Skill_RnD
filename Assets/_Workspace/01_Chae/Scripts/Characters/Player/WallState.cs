using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor.Callbacks;
using UnityEngine;

public class WallState : IState
{   
    Player player;

    public WallState(Player player){
        this.player = player;
    }
    public void Enter(){
        Debug.Log("OnWall");
    }
    public void Update(){
        if(!player.IsGrounded()){
            player.SlideWall();
            if(player.IsJumped()){
                player.JumpWall();
            }
        }
    }
    public void Exit(){
        
    }
}
