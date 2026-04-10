using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Player : CharacterParent
{   
    //Value vars
    public float jumpForce;
    //wallJump Vars
    [Header("=== Wall Jump Vars ===")]
    public float wallJumpDelayTime;
    //public float wallJumpCoolTime;
    [HideInInspector] public bool isWallJumping;
    //public bool canWallJump;
    public float slideSpeed;
    private float wallDir;
    
    [Header("=== Dash Vars ===")]
    public float dashForce;
    public float dashDelayTime;
    [HideInInspector] public bool canDash;
    [HideInInspector] public bool isDashing;


    //Get componet Vars
    [HideInInspector] public Rigidbody2D rb;



    [Header("=== States ===")]
    public Vector2 dir;
    //States
    enum PlayerStateType{
        ON_GROUND,
        ON_AIR,
        ON_WALL
    }
    [SerializeField] PlayerStateType currentStateType;
    IState currentState;

    
    [Header("=== Layers ===")] 
    //Layers Vars <- Inspector 에서 받음
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float rayDistance = 1.0f;


    [Header("=== key Binding ===")]
    //Key binding
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;

    //Main
    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }
    void Update(){
        MyInput();
        UpdateState();
        currentState?.Update();
    }

    //State Manager Funcs
    public void ChangeState(IState newState){
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    void UpdateState(){
        PlayerStateType newState;
        //상태구별 후 새상태 부여
        if(IsGrounded()){
            newState = PlayerStateType.ON_GROUND;
        }else if(IsOnWall() && !IsGrounded()){
            newState = PlayerStateType.ON_WALL;
        }else{
            newState = PlayerStateType.ON_AIR;
        }
        //새상태 체크 후 현재 상태를 새상태로 바꿈
        if(currentState == null || currentStateType != newState){
            switch(newState){
                case PlayerStateType.ON_GROUND:
                    ChangeState(new GroundState(this));
                    break;
                case PlayerStateType.ON_WALL:
                    ChangeState(new WallState(this));
                    break;
                case PlayerStateType.ON_AIR:
                    ChangeState(new AirState(this));
                    break;
            }
            currentStateType = newState;
        }
    }

    //Input Funcs
    void MyInput(){
        dir.x = Input.GetAxis("Horizontal");
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
    public override void Move(){
        if(!isWallJumping && !isDashing)
            rb.velocity = new Vector2(dir.x * moveSpeed,rb.velocity.y);  
    }
    //--01_Wall Jump Funcs
    public void SlideWall(){
        //삼항연산자 -> 현재 방향(입력 방향)과 벽방향이 일치? T : velocity.x 의 값을 0으로 만듬 / F : 그대로 움직임
        // T -> X가 0이아니면 벽에 달라붙어서 안움직여서,velocity.y -= slideSpped 가 동작이안됌;
        // F -> 그대로 움직입니다 : 벽에서 탈출; 
        rb.velocity = new Vector2((wallDir == Mathf.Sign(dir.x)? 0 : rb.velocity.x),-slideSpeed);
    }
    public void JumpWall(){
        rb.velocity = new Vector2(-Mathf.Sign(dir.x) * moveSpeed * .9f, jumpForce);
        isWallJumping = true;
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
        Vector2 dirDash = (mousePos - transform.position).normalized;

        //물리 작용
        rb.velocity = dirDash * dashForce;
        Invoke(nameof(TriggerDash),dashDelayTime);
    } 
    void TriggerDash(){
        isDashing = false;
    }
    
    //boolean Funcs
    public bool IsGrounded(){ // 지면확인
        RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.down,rayDistance,groundLayer);
        return hit.collider != null;
    }
    public bool IsOnWall(){ // 벽 확인
        RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.right * dir.x ,rayDistance,wallLayer);
        return hit.collider != null;
    }
    protected override void Attack(){
        //애니메이션 함수 -> 애니메이션 내부에서 attack area 구현 후 area script에서 부모 atk를 가져와서 처리
    }
}
