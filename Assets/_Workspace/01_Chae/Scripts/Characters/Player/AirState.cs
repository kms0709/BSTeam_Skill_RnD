using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class AirState : IState
{
    Player player;

    public AirState(Player player){
        this.player = player;
    }
    public void Enter(){
        Debug.Log("OnAir");
        player.SetGravity(1);
    }
    public void Update(){
        player.Move();
    }
    public void Exit(){
        
    }
}
