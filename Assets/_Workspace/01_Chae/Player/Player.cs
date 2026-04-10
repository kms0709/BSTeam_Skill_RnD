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
    public float wallJumpForce;
    public float wallJumpCool;
    public bool isWallJumping;
    public float slideSpeed;
    private float wallDir;
    //Get componet Vars
    [HideInInspector] public Rigidbody2D rb; 
    public Vector2 dir;
    //States
    enum PlayerStateType{
        ON_GROUND,
        ON_AIR,
        ON_WALL
    }
    [Header("State")]
    PlayerStateType currentStateType;
    IState currentState;

    
    [Header("Layers")] 
    //Layers Vars <- Inspector 에서 받음
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float rayDistance = 1.0f;


    [Header("key Binding")]
    //Key binding
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey = KeyCode.LeftShift;
    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }
    void Start(){
        
    }
    void Update(){
        dir.x = Input.GetAxis("Horizontal");
        UpdateState();
        currentState?.Update();
    }
    void FixedUpdate(){
        
    }
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
    //Physics Funcs
    public override void Move(){
        if(!isWallJumping){
            rb.velocity = new Vector2(dir.x * moveSpeed,rb.velocity.y);    
        }else{
            rb.velocity = Vector2.Lerp(rb.velocity,(new Vector2(dir.x * moveSpeed,rb.velocity.y)),.5f * Time.deltaTime);    
        }
        
    }
    public bool IsJumped(){
        return Input.GetKey(jumpKey);
    }
    //--Wall Funcs
    public void SlideWall(){
        rb.velocity = new Vector2(rb.velocity.x,-slideSpeed);
    }
    public void JumpWall(){
        rb.velocity = Vector2.Lerp(rb.velocity,(new Vector2(dir.x * moveSpeed,rb.velocity.y)),.5f * Time.deltaTime);
        // rb.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpForce, wallJumpForce);
        isWallJumping = true;
        Invoke(nameof(TriggerWallJump),0.2f);
    }
    public void TriggerWallJump(){
        isWallJumping = false;
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
