using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _PlayerJumpState : _IPlayerState<_Player>
{
    public void Enter(_Player handle)
    {
        Debug.Log("State : Jump");

        float xForce = Mathf.Cos(handle.JumpAngle * Mathf.Deg2Rad) * handle.Speed * handle.JumpDirection;
        handle.Rigidbody2D.AddForce(new Vector2(xForce, handle.JumpForce), ForceMode2D.Impulse);

        handle.SetJumpDirection(0);
    }

    public void Execute(_Player handle)
    {
        Vector2 moveVector = new Vector2(handle.InputX * handle.Speed * Time.deltaTime, 0f);
        handle.transform.Translate(moveVector, Space.World);
    }

    public void Exit(_Player handle)
    {
    }
}
