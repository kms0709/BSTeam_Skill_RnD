using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class _PlayerRunState : _IPlayerState<_Player>
{
    public void Enter(_Player handle)
    {
        Debug.Log("State : Run");
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
