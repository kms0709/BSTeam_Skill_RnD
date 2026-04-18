using UnityEngine;

// ===================================================
// Player_WallJump.cs — 벽점프 변수 및 관련 함수
// ===================================================
public partial class Player
{
    [Header("=== Wall Jump ===")]

    [Header("-01_Force Value")]
    public float slideSpeed;

    [Header("-02_Times")]
    public float wallAttachTime;
    public float wallJumpDelayTime;

    [HideInInspector] public float lastWallDir;
    [HideInInspector] public bool isWallJumping;

    //--Wall Jump Funcs--
    public void SlideWall(){
        SetGravity(0);
        float smooth = Mathf.Lerp(rb.linearVelocity.y, -slideSpeed, Time.deltaTime);
        rb.linearVelocity = new Vector2(0, smooth);
    }

    public void AttatchWall(){
        SetGravity(0);
        rb.linearVelocity = Vector2.zero;
    }

    public void JumpWall(){
        SetGravity(1);

        lastWallDir = dirX;
        rb.linearVelocity = new Vector2(-dirX * moveSpeed * 0.5f, jumpForce);

        isWallJumping = true;
        Invoke(nameof(TriggerWallJump), wallJumpDelayTime);
    }

    public void TriggerWallJump(){
        isWallJumping = false;
    }
}
