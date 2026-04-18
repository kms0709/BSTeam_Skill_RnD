using UnityEngine;

/// <summary>
/// 2D 횡스크롤 플랫포머 플레이어 컨트롤러 (Unity 2022.3.x / Legacy Input System)
///
/// [기능]
/// 1. 좌우 이동 (A/D 또는 방향키)
/// 2. 점프 - 바닥에 있을 때만 (더블점프 X, 연속점프 X)
/// 3. 벽점프 - 공중에서 벽에 닿은 순간 점프 권한 재부여
/// 4. 대쉬 - 플레이어→마우스 방향 대쉬, 쿨타임 존재 (마우스 우클릭)
///
/// [감지 방식]
/// Raycast 대신 OnCollisionEnter2D/Exit2D의 ContactPoint.normal로 바닥/벽 구분
/// → Inspector에 거리 값 노출 불필요, Collider 크기와 무관하게 정확히 동작
/// </summary>
public class PlayerMove : MonoBehaviour
{
    // ────────────────────────────────────────────────────
    //  Inspector 설정값
    // ────────────────────────────────────────────────────

    [Header("Move Settings")]
    [Tooltip("최대 이동 속도 (x축)")]
    public float maxSpeed = 5.0f;

    [Tooltip("이동 시 가하는 힘의 크기")]
    public float moveForce = 2.0f;

    [Tooltip("점프 시 가하는 힘의 크기")]
    public float jumpForce = 10.0f;

    [Header("Wall Jump Settings")]
    [Tooltip("벽 점프 시 수평 방향(벽 반대)으로 가하는 힘")]
    public float wallJumpHorizontalForce = 6.0f;

    [Tooltip("벽 점프 시 수직으로 가하는 힘")]
    public float wallJumpVerticalForce = 10.0f;

    [Header("Dash Settings")]
    [Tooltip("대쉬 이동 속도")]
    public float dashSpeed = 20.0f;

    [Tooltip("대쉬 유지 시간 (초)")]
    public float dashDuration = 0.2f;

    [Tooltip("대쉬 쿨타임 (초)")]
    public float dashCoolTime = 1.0f;

    // ────────────────────────────────────────────────────
    //  내부 변수
    // ────────────────────────────────────────────────────

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    // 이동
    float h;

    // 점프
    bool isJumpPressed;
    bool canJump;
    bool isOnGround;
    bool isOnWall;
    int wallDirection; // +1: 오른쪽 벽, -1: 왼쪽 벽
    bool isWallJump;

    // 대쉬
    bool isDashing;
    Vector2 dashDirection;
    float dashTimeLeft;
    float lastDashTime = -100f;
    float originalGravity;

    // 충돌 카운터 (Enter/Exit 쌍으로 정확한 접촉 추적)
    int groundContactCount;
    int wallContactCount;

    // ────────────────────────────────────────────────────
    //  Unity 생명주기
    // ────────────────────────────────────────────────────

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalGravity = rigid.gravityScale;

        // ── 모서리 걸림(Seam 멈춤) 방지 처리 ──
        // 마찰력을 0으로 설정하여 타일 사이 접합부에서 걸리지 않도록 만듭니다.
        // 마찰력을 없앴기 때문에 FixedUpdate에서 별도로 감속 처리(브레이크)를 담당합니다.
        PhysicsMaterial2D noFrictionMat = new PhysicsMaterial2D("NoFriction");
        noFrictionMat.friction = 0f;
        noFrictionMat.bounciness = 0f;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.sharedMaterial = noFrictionMat;
        }
    }

    void Update()
    {
        // ── 대쉬 진행 중 ──────────────────────────────
        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0f)
                EndDash();
            return;
        }

        // ── 대쉬 입력: 마우스 우클릭 ─────────────────
        if (Input.GetMouseButtonDown(1) && Time.time >= lastDashTime + dashCoolTime)
        {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            Vector2 dir = ((Vector2)mouseWorld - (Vector2)transform.position).normalized;
            StartDash(dir);
            return;
        }

        // ── 좌우 이동 입력 ───────────────────────────
        h = Input.GetAxisRaw("Horizontal");

        // ── 점프 입력 ────────────────────────────────
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            isJumpPressed = true;
            isWallJump = isOnWall && !isOnGround; // 공중 + 벽 접촉 → 벽점프
        }

        // ── 스프라이트 방향: 마우스 포인터 기준 ──────
        Vector3 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        spriteRenderer.flipX = (mp.x < transform.position.x);

        // ── 애니메이터 파라미터 ───────────────────────
        if (anim != null)
        {
            anim.SetBool("isRun", h != 0f);
            anim.SetBool("isJump", !isOnGround);
        }
    }

    void FixedUpdate()
    {
        // 대쉬 중: 대쉬 방향으로 속도 고정
        if (isDashing)
        {
            rigid.velocity = dashDirection * dashSpeed;
            return;
        }

        // ── 좌우 이동 ────────────────────────────────
        rigid.AddForce(Vector2.right * h * moveForce, ForceMode2D.Impulse);

        // x축 최대 속도 제한
        if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
        {
            rigid.velocity = new Vector2(
                Mathf.Sign(rigid.velocity.x) * maxSpeed,
                rigid.velocity.y);
        }

        // 입력이 없을 때 강제 감속 (마찰력이 0이므로 코드에서 브레이크를 잡아줌)
        if (Mathf.Abs(h) < 0.01f)
        {
            float decelerate = isOnGround ? 0.6f : 0.95f; // 바닥이면 빨리 멈추고 공중이면 서서히 멈춤
            rigid.velocity = new Vector2(rigid.velocity.x * decelerate, rigid.velocity.y);

            // 속도가 너무 작아지면 완전히 0으로 고정
            if (Mathf.Abs(rigid.velocity.x) < 0.1f)
                rigid.velocity = new Vector2(0f, rigid.velocity.y);
        }

        // ── 점프 ─────────────────────────────────────
        if (isJumpPressed)
        {
            isJumpPressed = false;
            canJump = false;

            if (isWallJump)
            {
                // 벽점프: 벽 반대 방향 + 위로
                rigid.velocity = Vector2.zero;
                rigid.AddForce(
                    new Vector2(-wallDirection * wallJumpHorizontalForce,
                                wallJumpVerticalForce),
                    ForceMode2D.Impulse);
            }
            else
            {
                // 일반 점프
                rigid.velocity = new Vector2(rigid.velocity.x, 0f);
                rigid.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    // ────────────────────────────────────────────────────
    //  충돌 감지 (ContactPoint.normal로 바닥/벽 구분)
    //  → groundCheckDistance / wallCheckDistance 불필요
    // ────────────────────────────────────────────────────

    void OnCollisionEnter2D(Collision2D col)
    {
        ProcessContacts(col, +1);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        ProcessContacts(col, -1);
    }

    /// <summary>
    /// delta = +1 (Enter) 또는 -1 (Exit)
    /// ContactPoint.normal 방향으로 바닥/벽 판별
    /// </summary>
    void ProcessContacts(Collision2D col, int delta)
    {
        bool touchedGround = false;
        bool touchedWall = false;

        foreach (ContactPoint2D contact in col.contacts)
        {
            // normal.y > 0.5 → 아래에서 올라오는 면 = 바닥
            if (contact.normal.y > 0.5f)
                touchedGround = true;

            // |normal.x| > 0.7 → 옆면 = 벽
            if (Mathf.Abs(contact.normal.x) > 0.7f)
            {
                touchedWall = true;
                // 벽 방향 기록 (Enter 시에만 갱신)
                if (delta > 0)
                    wallDirection = contact.normal.x < 0 ? 1 : -1;
            }
        }

        // 카운터 증감 (음수 방지)
        if (touchedGround)
            groundContactCount = Mathf.Max(0, groundContactCount + delta);
        if (touchedWall)
            wallContactCount = Mathf.Max(0, wallContactCount + delta);

        // 상태 갱신
        isOnGround = groundContactCount > 0;
        isOnWall = wallContactCount > 0;

        // 바닥 또는 벽에 닿으면 점프 권한 부여
        if (isOnGround || isOnWall)
            canJump = true;
    }

    // ────────────────────────────────────────────────────
    //  대쉬
    // ────────────────────────────────────────────────────

    void StartDash(Vector2 dir)
    {
        isDashing = true;
        dashDirection = dir;
        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;
        rigid.gravityScale = 0f;
        rigid.velocity = Vector2.zero;

        if (anim != null) anim.SetBool("isRun", false);
    }

    void EndDash()
    {
        isDashing = false;
        rigid.gravityScale = originalGravity;
        rigid.velocity *= 0.2f;
    }
}
