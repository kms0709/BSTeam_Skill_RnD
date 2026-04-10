using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEditor.Callbacks;
using UnityEngine;

public class WallState : MonoBehaviour,IState
{   
    Player player;

    public WallState(Player player){
        this.player = player;
    }
    public void Enter(){
        Debug.Log("OnWall");
    }
    public void Update(){
        player.Move();
        player.SlideWall();
        if(player.IsJumped()){
            player.JumpWall();
        }
    }
    public void Exit(){
        
    }
    private void EscapeWall(){
        player.rb.velocity = new Vector2(-player.transform.localScale.x * 10,0);
    }
}
