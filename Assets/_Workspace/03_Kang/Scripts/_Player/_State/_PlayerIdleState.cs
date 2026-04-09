using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _PlayerIdleState : _IPlayerState<_Player>
{
    public void Enter(_Player handle)
    {
        Debug.Log("State : Idle");
    }

    public void Execute(_Player handle)
    {
    }

    public void Exit(_Player handle)
    {
    }
}
