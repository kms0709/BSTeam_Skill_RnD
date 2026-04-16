using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Player : CharacterParent
{   
    //Player Only
    public float jumpForce; 

    // 이거 가져가고
    //wallJump Vars
    [Header("=== Wall Jump Logic ===")]
    
    [Header("-01_Force Value")]
    public float slideSpeed;

    [Header("-02_Times")]
    public float wallAttachTime;
    public float wallJumpDelayTime;

    [HideInInspector] public float lastWallDir;
    
    //boolean
    [HideInInspector] public bool isWallJumping;
    
    [Header("=== Dash Logic ===")]
    public float dashForce;
    public float dashDelayTime;
    [HideInInspector] public bool canDash;
    [HideInInspector] public bool isDashing;

    [Header("=== Inputs ===")]
    //Key binding
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    public float inputX;

    //Main
    private void Start(){
        rb.gravityScale = 0;
    }
    protected override void Update(){
        base.Update();
        // MyInput();
    }
    protected override void FixedUpdate(){
        MyInput();
    }
    protected override void UpdateState(){
        StateTypePhysics newState;
        //상태구별 후 새상태 부여
        if(IsGrounded()){
            newState = StateTypePhysics.ON_GROUND;
        }else if(IsOnWall() && !IsGrounded()){
            newState = StateTypePhysics.ON_WALL;
        }else{
            newState = StateTypePhysics.ON_AIR;
        }
        //새상태 체크 후 현재 상태를 새상태로 바꿈
        if(currentPhysicsState == null || currentPhysicsStateType != newState){
            switch(newState){
                case StateTypePhysics.ON_GROUND:
                    ChangeState(new GroundState_PLAYER(this));
                    break;
                case StateTypePhysics.ON_WALL:
                    ChangeState(new WallState_PLAYER(this));
                    break;
                case StateTypePhysics.ON_AIR:
                    ChangeState(new AirState_PLAYER(this));
                    break;
            }
            currentPhysicsStateType = newState;
        }
    }
    //Input Funcs
    //VI
    void MyInput(){
        inputX = Input.GetAxis("Horizontal");
        if(IsDashed() && canDash){
            Debug.Log("Dash!");
            Dash();
        }
    }
    //--01_Get Input boolean Funcs
    public bool IsJumped(){
        return Input.GetKey(jumpKey);
    }
    public bool IsDashed(){
        return Input.GetKey(dashKey);
    }

    //Physics Funcs
    //@V_M
    public override void Move(){
        if(inputX != 0 && !isDashing){
            dirX = Mathf.Sign(inputX);
        }
        if(!isWallJumping && !isDashing){
            rb.velocity = new Vector2(inputX * moveSpeed,rb.velocity.y);
        }
        if(!IsGrounded()){
            ApplyGravity();
        }

    }
    public void Jump(){
        rb.velocity = new Vector2(rb.velocity.x,jumpForce);
    }


    //--01_Wall Jump Funcs
    //@V_W
    public void SlideWall(){
        SetGravity(0);

        //벽에 달라붙어있다면 -> Vel.x를 0 으로설정 아니면 벽에서 빠져나옴;
        // float velX = IsOnWall()? 0 : rb.velocity.x;
        float smooth = Mathf.Lerp(rb.velocity.y, -slideSpeed,Time.deltaTime);
        rb.velocity = new Vector2(0,smooth);
    }

    public void AttatchWall(){
        SetGravity(0);
        // float velX = IsOnWall()? 0 : rb.velocity.x;
        rb.velocity = new Vector2(0,0);
    }
    // public void EscapeWall(){
    //     SetGravity(1);
    //     rb.velocity = new Vector2(-dir.x * moveSpeed * 0.1f, jumpForce * 0.1f);
    // }
    public void JumpWall(){

        SetGravity(1);

        //현재 벽 방향을 저장;
        lastWallDir = dirX;
        
        //물리 적용
        rb.velocity = new Vector2(-dirX * moveSpeed * 0.5f, jumpForce);
        
        //Set boolean
        isWallJumping = true;

        //Invoke 점프딜레이타임후 isWallJumping -> false
        Invoke(nameof(TriggerWallJump),wallJumpDelayTime);
    }
    public void TriggerWallJump(){
        isWallJumping = false;
    }
    //--02_Dash Funcs
    void Dash(){
        if(!isDashing)
        //booelan 조정
        isDashing = true;
        canDash = false;

        //마우스 좌표값 구하기
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        if(mousePos.x != 0){
            dirX = (mousePos.x > 0)? 1f : -1f;
        }
        Vector2 dirDash = (mousePos - transform.position).normalized;

        //물리 작용
        rb.velocity = dirDash * dashForce;
        Invoke(nameof(TriggerDash),dashDelayTime);
    } 
    void TriggerDash(){
        isDashing = false;
        // 대시가 끝난 후 위로 계속 날아가는 관성을 제거하여 가로대시와 이동 거리를 맞춤
        rb.velocity = new Vector2(0f, Mathf.Min(rb.velocity.y, 0f));
    }
    

    protected override void Attack(){
        //애니메이션 함수 -> 애니메이션 내부에서 attack area 구현 후 area script에서 부모 atk를 가져와서 처리
    }
}
