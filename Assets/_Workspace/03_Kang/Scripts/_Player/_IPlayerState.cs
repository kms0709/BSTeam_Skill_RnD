using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface _IPlayerState<T>
{
    public void Enter(T handle);
    public void Execute(T handle);
    public void Exit(T handle);
}
