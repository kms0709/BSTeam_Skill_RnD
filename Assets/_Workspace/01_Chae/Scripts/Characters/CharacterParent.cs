using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
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

    [Space(10f)]
    [Tooltip("경사면 감지 Raycast 길이")]
    public float colSlopeRayDist = 0.5f;
    [Tooltip("이 각도(도) 이상이면 경사면으로 판정")]
    public float slopeAngleLimit = 5f;
    protected Vector2 slopeNormalPerp;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Collider2D col;
    [HideInInspector] public Animator anim;

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
        anim = GetComponent<Animator>();
    }
    protected virtual void Update(){
        //물리상태
        UpdateState_Physics();
        currentState_Physics?.Update();

        //애니메이션 상태
        UpdateState_Animate();
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
        // Collider2D hit = Physics2D.OverlapBox(checkPos, Vector2.one, 0f, tileMapLayer);

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

    /// <summary>
    /// 지면 법선(Normal)을 기반으로 경사면 여부를 판정하는 함수
    /// </summary>
    /// <returns>
    /// 캐릭터 발 아래 Raycast 가 tileMapLayer 에 닿았을 때,
    /// 지면 법선과 Vector2.up 의 각도가 slopeAngleLimit 이상이면 true
    /// </returns>
    /// 

    
    // protected virtual bool IsOnSlope()
    // {
    //     // 발 중앙 아래에서 레이 시작점 설정
    //     Vector2 rayOrigin = new Vector2(col.bounds.center.x, col.bounds.min.y);

    //     RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, colSlopeRayDist, tileMapLayer);

    //     // Debug: 레이 경로 (흰색)
    //     Debug.DrawRay(rayOrigin, Vector2.down * colSlopeRayDist, Color.white);

    //     if (hit.collider != null)
    //     {
    //         // 지면 법선과 위쪽 방향의 각도 계산
    //         float angle = Vector2.Angle(hit.normal, Vector2.up);
    //         slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

    //         // Debug: 지면 법선 방향 (노란색)
    //         Debug.DrawRay(hit.point, hit.normal * 0.4f, Color.yellow);

    //         return angle >= slopeAngleLimit;
    //     }

    //     return false;
    // }


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
        float newVelocity = rb.linearVelocity.y - (gravity * gravityModifier * Time.deltaTime);
        // newVelocity = Mathf.Max(newVelocity,-maxFallSpeed);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x,newVelocity);
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

            // Slope Ray Gizmo
            Vector2 slopeRayOrigin = new Vector2(gizmoCol.bounds.center.x, gizmoCol.bounds.min.y);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(slopeRayOrigin, slopeRayOrigin + Vector2.down * colSlopeRayDist);
        }
    }

    #endregion
    public abstract void Move();
    protected abstract void Attack();

    
}
