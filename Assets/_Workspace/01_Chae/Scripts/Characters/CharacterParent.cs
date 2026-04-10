using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterParent : MonoBehaviour
{
    [Header("=== Character Values ===")]
    public int hpMax;
    public int hpCur;
    public int atk; 
    public float moveSpeed;
    public abstract void Move();
    protected abstract void Attack();
}
