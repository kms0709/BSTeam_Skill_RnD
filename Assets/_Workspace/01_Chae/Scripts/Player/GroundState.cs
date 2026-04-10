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
        player.isWallJumping = false;
    }
    public void Update(){
        player.Move();
        if(player.IsJumped() && player.IsGrounded()){
            player.rb.velocity = new Vector2(player.rb.velocity.x,player.jumpForce);
        }
    }
    public void Exit(){
        
    }
}
