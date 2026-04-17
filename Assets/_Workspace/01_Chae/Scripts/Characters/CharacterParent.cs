using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.ShaderGraph;
using UnityEngine;

public abstract class CharacterParent : MonoBehaviour
{
    [SerializeField] private float _dirX = 1f;
    
    public float dirX { //dirX를 -1 또는 1로만 고정하는 프로퍼티
        get => _dirX;
        set{
            if(value != 0)
            _dirX = Mathf.Sign(value);
        }
    }
    protected IState currentState_Physics;
    protected enum StateType_Physics{
        ON_GROUND,
        ON_AIR,
        ON_WALL
    }
    
    [SerializeField] protected StateType_Physics currentStateType_Physics;
    protected IState currentState_Animate;
 
    [Header("=== Physics ===")]
    public float gravityModifier = 1f;
    public float gravity = 5f;
    //Layers Vars <- Inspector 에서 받음
    public LayerMask tileMapLayer;
    [Header("=== Colision ===")]
    //혹시 모르니 콜라이더를 직접 조정할 수 있게 하는 변수
    public Vector2 colWallOffSet;
    public float colWallSize = 0.9f;

    [Space(10f)]

    public float colGroundOffSet = 1.0f;
    public float colGroundSize = 0.9f;


    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Collider2D col;

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
    #region Main
    protected virtual void Awake(){
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    protected virtual void Update(){
        //물리상태
        UpdateState_Physics();
        currentState_Physics?.Update();

        //애니메이션 상태
        UpdateState_Animate();
        currentState_Animate?.Update();
    }
    protected virtual void FixedUpdate(){
        
    }

    #endregion


    //State 관련 로직
    #region State
    public void ChangeState_Physics(IState newState){
        currentState_Physics?.Exit();
        currentState_Physics = newState;
        currentState_Physics.Enter();
    }
    public void ChangeState_Animate(IState newState){
        currentState_Animate?.Exit();
        currentState_Animate = newState;
        currentState_Animate.Enter();
    }
    protected virtual void UpdateState_Physics(){
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
    protected abstract void UpdateState_Animate();

    #endregion
    
    
    // 땅감지, 벽감지 :
    #region Detection
    /// <summary>
    /// 땅 감지 boolean식 함수
    /// </summary>
    /// <returns>OverlapCircle 이 땅Layer 에 닿았을때 true를 반환함</returns>
    
    protected bool IsGrounded(){ // 지면확인

        //콜라이더 크기 체크 후 onGround OverlapCircle 생성
        Vector2 checkPos = new Vector2(col.bounds.center.x,col.bounds.min.y - colGroundOffSet);

        //반지름 설정
        float checkRadius = col.bounds.extents.x * colGroundSize;

        Collider2D hit = Physics2D.OverlapCircle(checkPos,checkRadius,tileMapLayer);
        // RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.down,rayDistanceGround,tileMapLayer);
        // Debug.DrawRay(transform.position, Vector2.down * hit.distance, Color.red);
        return hit != null;
    }

    /// <summary>
    /// 벽 감지 boolean식 함수
    /// </summary>
    /// <returns>OverlapCircle 이 벽Layer 에 닿았을때 true를 반환함</returns>

    protected virtual bool IsOnWall(){
        Vector2 checkPos = new Vector2(col.bounds.center.x + (dirX * colWallOffSet.x),col.bounds.center.y + colWallOffSet.y);
        
        //반지름 확인; * float 값 만큼 크기조정
        float checkRadius = col.bounds.extents.x * colWallSize;
        Collider2D hit = Physics2D.OverlapCircle(checkPos, checkRadius, tileMapLayer);
        return hit != null;
    }

    #endregion
    

    // SetGravity, ApplyGeavity :
    #region Gravity
    /// <summary>
    /// 중력 가속도 * 곱의 값을 설정하는 함수
    /// </summary>
    /// <param name="modi"></param>
    public void SetGravity(float modi){
        gravityModifier = modi;
    }

    /// <summary>
    /// 중력 가속도 * 중력 곱 의 물리적 작용을 하는 함수
    /// </summary>
    protected void ApplyGravity(){
        if(gravityModifier == 0) return;
        float newVelocity = rb.velocity.y - (gravity * gravityModifier * Time.deltaTime);
        // newVelocity = Mathf.Max(newVelocity,-maxFallSpeed);
        rb.velocity = new Vector2(rb.velocity.x,newVelocity);
    }
        protected virtual void OnDrawGizmos(){
        Collider2D gizmoCol = col != null ? col : GetComponent<Collider2D>();
        if(gizmoCol != null){
            // Ground Gizmo
            Vector2 checkPosGround = new Vector2(gizmoCol.bounds.center.x, gizmoCol.bounds.min.y - colGroundOffSet);
            float checkRadiusGround = gizmoCol.bounds.extents.x * colGroundSize;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(checkPosGround, checkRadiusGround);

            // Wall Gizmo
            Vector2 checkPosWall = new Vector2(gizmoCol.bounds.center.x + (dirX * colWallOffSet.x), gizmoCol.bounds.center.y + colWallOffSet.y);
            float checkRadiusWall = gizmoCol.bounds.extents.x * colWallSize;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(checkPosWall, checkRadiusWall);
        }
    }

    #endregion
    public abstract void Move();
    protected abstract void Attack();

    
}
