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
    private enum StateType_Animate {
        IDLE,
        MOVING,
        JUMPING,
        IN_AIR,
        DASHING,
        ATTATCHING_WALL,
        SLIDING_WALL,
        DEATH
    }
    [SerializeField] private StateType_Animate currentStateType_Animate;
    public float jumpForce; 
    
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
    [HideInInspector] public bool canDash = true;
    [HideInInspector] public bool isDashing;
    public float dashCoolTime;
    public CountDownTimer dashTimer; 

    [Header("=== Inputs ===")]
    //Key binding
    public KeyCode jumpKey = KeyCode.Space;
    public float inputX;

    //Unity LifetCycle : 
    #region Main 
    protected override void Awake(){
        base.Awake();
        dashTimer = CountDownTimer.Create(dashCoolTime,() => canDash = true);
    }
    private void Start(){
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        dashTimer.StartTimer(0);
    }
    protected override void Update(){
        base.Update();
        MyInput();
        if(!canDash && IsGrounded()){
            dashTimer.Tick(Time.deltaTime);
        }
    }
    protected override void FixedUpdate(){
       //쓸모없는 Fixe녀석
    }
    #endregion

    //PhysicsState , AnimateState :
    #region State
    protected override void UpdateState_Physics(){
        StateType_Physics newState;
        //상태구별 후 새상태 부여
        if(IsGrounded()){
            newState = StateType_Physics.ON_GROUND;
        }else if(IsOnWall() && !IsGrounded()){
            newState = StateType_Physics.ON_WALL;
        }else{
            newState = StateType_Physics.ON_AIR;
        }
        //새상태 체크 후 현재 상태를 새상태로 바꿈
        if(currentState_Physics == null || currentStateType_Physics != newState){
            switch(newState){
                case StateType_Physics.ON_GROUND:
                    ChangeState_Physics(new GroundState_PLAYER(this));
                    break;
                case StateType_Physics.ON_WALL:
                    ChangeState_Physics(new WallState_PLAYER(this));
                    break;
                case StateType_Physics.ON_AIR:
                    ChangeState_Physics(new AirState_PLAYER(this));
                    break;
            }
            currentStateType_Physics = newState;
        }
    }
    protected override void UpdateState_Animate(){
        StateType_Animate newState;
        if(IsGrounded()){
            if(rb.velocity.x != 0){
                newState = StateType_Animate.MOVING;
            }else{
                newState = StateType_Animate.IDLE;
            }
        }else if(IsOnWall()){
            if(rb.velocity.y == 0f){
                newState = StateType_Animate.ATTATCHING_WALL;
            }else{
                newState = StateType_Animate.SLIDING_WALL;
            }
        }else{
            if(rb.velocity.y > 0){
                newState = StateType_Animate.JUMPING;
            }else{
                newState = StateType_Animate.IN_AIR;
            }
        if(isDashing){
            newState = StateType_Animate.DASHING;
        }
        }
        if(newState != currentStateType_Animate)
        switch(newState){
            case StateType_Animate.MOVING : anim.SetTrigger("Do_Move"); Debug.Log("Move"); break;
            case StateType_Animate.DASHING : anim.SetTrigger("Do_Dash"); Debug.Log("Dash");break;
            case StateType_Animate.JUMPING : anim.SetTrigger("Do_Jump"); Debug.Log("Jump");break;
            case StateType_Animate.IN_AIR : anim.SetTrigger("Do_Fall"); Debug.Log("Fall");break;
            case StateType_Animate.ATTATCHING_WALL : anim.SetTrigger("Do_Attatch"); Debug.Log("Attatch");break;
            case StateType_Animate.SLIDING_WALL : anim.SetTrigger("Do_Slide"); Debug.Log("Slide");break;
            default : anim.SetTrigger("Do_Idle"); Debug.Log("Idle"); break;
        }
        currentStateType_Animate = newState;
    }

    #endregion

    //Input System : 
    #region Input
    void MyInput(){
        inputX = Input.GetAxis("Horizontal");
        if(IsDashed() && canDash){
            Debug.Log("Dash!");
            Dash();
        }
    }
    //--01_Get Input boolean Funcs
    public bool IsJumped(){
        return Input.GetKeyDown(jumpKey);
    }
    public bool IsDashed(){
        return Input.GetMouseButtonDown(1);
    }

    #endregion

    //Move, Jump, SlideWall, AttatchWall, JumpWall, Dash :
    #region Physics 
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

        #region -ㄴ01_Wall
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

        #endregion
    
        #region -ㄴ02_Dash
    void Dash(){
        if(isDashing) return;

        //booelan 조정
        isDashing = true;
        canDash = false;
        dashTimer.StartTimer(dashCoolTime);

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
    
        #endregion

    #endregion
    protected override void Attack(){
        //애니메이션 함수 -> 애니메이션 내부에서 attack area 구현 후 area script에서 부모 atk를 가져와서 처리
    }
}
