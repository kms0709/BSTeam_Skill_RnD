using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class GroundState : IState
{
    Player player;
    public GroundState(Player player){
        this.player = player;
    }
    public void Enter(){
        Debug.Log("OnGround");
        player.SetGravity(0);

        //벽점프 Var 조정
        player.isWallJumping = false;
        player.canWallJump = true;

        //대쉬 boolean 조정
        player.canDash = true;
        player.isDashing = false;
    }
    public void Update(){
        player.Move();
        if(player.IsJumped()){
            player.Jump();
        }
    }
    public void Exit(){
        
    }
}
