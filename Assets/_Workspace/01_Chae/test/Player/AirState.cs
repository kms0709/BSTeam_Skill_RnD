using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : IState
{
    Player player;

    public AirState(Player player){
        this.player = player;
    }
    public void Enter(){
        Debug.Log("OnAir");
    }
    public void Update(){
        player.Move();
    }
    public void Exit(){
        
    }
}
