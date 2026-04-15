using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterParent : MonoBehaviour
{
    // [Header("=== Character State ===")]
    // [Header("=== States ===")]
    
    protected enum StateTypePhysics{
        ON_GROUND,
        ON_AIR,
        ON_WALL
    }
    [SerializeField] protected StateTypePhysics currentPhysicsStateType;
    protected IState currentPhysicsState;
    public Vector2 dir;
 
    [Header("=== Physics ===")]
    protected float gravityModifier = 1f;
    public float gravity = 5f;
    public float maxFallSpeed = 20f;
    //Layers Vars <- Inspector 에서 받음
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public float rayDistanceGround = 1.0f;
    public float rayDistanceWall = 1.0f;

    [HideInInspector] public Rigidbody2D rb;







    [Header("=== Character Values ===")]
    [SerializeField] private int _hpMax;
    [SerializeField] private int _hpCur;

    public int HpCur {
        get { return _hpCur; }
        set{
            _hpCur = Mathf.Clamp(value,0,_hpMax);

            if( _hpCur <= 0 ){
                //Die Code 
            }
        }
    }

    [Space(10f)]
    public int atk;
    [Space(10f)]
    public float moveSpeed;

    protected virtual void Update(){
        UpdateState();
        currentPhysicsState?.Update();
    }
    protected virtual void FixedUpdate(){
        
    }
    public void ChangeState(IState newState){
        currentPhysicsState?.Exit();
        currentPhysicsState = newState;
        currentPhysicsState.Enter();
    }
    //V
    protected virtual void UpdateState(){
        // StateTypePhysics newState;
        // //상태구별 후 새상태 부여
        // if(IsGrounded()){
        //     newState = StateTypePhysics.ON_GROUND;
        // }else if(IsOnWall() && !IsGrounded()){
        //     newState = StateTypePhysics.ON_WALL;
        // }else{
        //     newState = StateTypePhysics.ON_AIR;
        // }
        // //새상태 체크 후 현재 상태를 새상태로 바꿈
        // if(currentState == null || currentStateType != newState){
        //     switch(newState){
        //         case StateTypePhysics.ON_GROUND:
        //             ChangeState(new GroundState(this));
        //             break;
        //         case StateTypePhysics.ON_WALL:
        //             ChangeState(new WallState(this));
        //             break;
        //         case StateTypePhysics.ON_AIR:
        //             ChangeState(new AirState(this));
        //             break;
        //     }
        //     currentStateType = newState;
        // }
    }
    protected bool IsGrounded(){ // 지면확인
        RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.down,rayDistanceGround,groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * hit.distance, Color.red);
        return hit.collider != null;
    }
    protected virtual bool IsOnWall(){
        float angle = (dir.x > 0)? -45f : -135f;
        Vector2 rayDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        RaycastHit2D hit = Physics2D.Raycast(transform.position,rayDir,rayDistanceWall,wallLayer);
        Debug.DrawRay(transform.position, rayDir * rayDistanceWall, Color.blue);
        return hit.collider != null;
    }

    public void SetGravity(float modi){
        gravityModifier = modi;
    }
    protected void ApplyGravity(){
        if(gravityModifier == 0) return;
        float newVelocity = rb.velocity.y - (gravity * gravityModifier * Time.deltaTime);
        newVelocity = Mathf.Max(newVelocity,-maxFallSpeed);
        rb.velocity = new Vector2(rb.velocity.x,newVelocity);
    }
    public abstract void Move();
    protected abstract void Attack();

    
}
