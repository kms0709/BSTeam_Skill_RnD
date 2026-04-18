using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ===================================================
// Player.cs — 메인 (라이프사이클, 상태머신, 입력, 이동)
// ===================================================
public partial class Player : CharacterParent
{
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

    [Header("=== Base ===")]
    public float jumpForce;

    [Header("=== Inputs ===")]
    public KeyCode jumpKey = KeyCode.Space;
    public float inputX;

    //Unity LifeCycle :
    #region Main
    protected override void Awake(){
        base.Awake();
        DashAwake();
    }
    private void Start(){
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        DashStart();
    }
    protected override void Update(){
        base.Update();
        MyInput();
        DashUpdate();
    }
    protected override void FixedUpdate(){
        //쓸모없는 Fixe녀석
    }
    #endregion

    //PhysicsState, AnimateState :
    #region State
    protected override void UpdateState_Physics(){
        StateType_Physics newState;
        if(IsGrounded()){
            newState = StateType_Physics.ON_GROUND;
        }else if(IsOnWall() && !IsGrounded()){
            newState = StateType_Physics.ON_WALL;
        }else{
            newState = StateType_Physics.ON_AIR;
        }
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
            newState = rb.linearVelocity.x != 0 ? StateType_Animate.MOVING : StateType_Animate.IDLE;
        }else if(IsOnWall()){
            newState = rb.linearVelocity.y == 0f ? StateType_Animate.ATTATCHING_WALL : StateType_Animate.SLIDING_WALL;
        }else{
            newState = rb.linearVelocity.y > 0 ? StateType_Animate.JUMPING : StateType_Animate.IN_AIR;
            if(isDashing) newState = StateType_Animate.DASHING;
        }
        if(newState != currentStateType_Animate){
            switch(newState){
                case StateType_Animate.MOVING:          anim.SetTrigger("Do_Move");    break;
                case StateType_Animate.DASHING:         anim.SetTrigger("Do_Dash");    break;
                case StateType_Animate.JUMPING:         anim.SetTrigger("Do_Jump");    break;
                case StateType_Animate.IN_AIR:          anim.SetTrigger("Do_Fall");    break;
                case StateType_Animate.ATTATCHING_WALL: anim.SetTrigger("Do_Attatch"); break;
                case StateType_Animate.SLIDING_WALL:    anim.SetTrigger("Do_Slide");   break;
                default:                                anim.SetTrigger("Do_Idle");    break;
            }
            currentStateType_Animate = newState;
        }
    }
    #endregion

    //Input System :
    #region Input
    void MyInput(){
        inputX = Input.GetAxis("Horizontal");
        if(IsDashed() && canDash) Dash();
    }
    public bool IsJumped() => Input.GetKeyDown(jumpKey);
    public bool IsDashed() => Input.GetMouseButtonDown(1);
    #endregion

    //Move, Jump :
    #region Physics
    public override void Move(){
        if(inputX != 0 && !isDashing){
            dirX = Mathf.Sign(inputX);
        }
        if(!isWallJumping && !isDashing){
            rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);
        }
        if(!IsGrounded()){
            ApplyGravity();
        }
    }
    public void Jump(){
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
    #endregion

    protected override void Attack(){
        //애니메이션 함수 → 애니메이션 내부에서 attack area 구현 후 area script에서 부모 atk를 가져와서 처리
    }
}
