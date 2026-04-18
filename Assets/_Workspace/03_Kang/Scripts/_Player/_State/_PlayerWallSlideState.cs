using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _PlayerWallSlideState : _IPlayerState<_Player>
{
    public void Enter(_Player handle)
    {
        Debug.Log("State : Wall Slide");

        handle.Rigidbody2D.linearVelocity = Vector3.zero;
        handle.Rigidbody2D.gravityScale = 0f;
    }

    public void Execute(_Player handle)
    {

    }

    public void Exit(_Player handle)
    {
        handle.Rigidbody2D.gravityScale = 1f;
    }
}
