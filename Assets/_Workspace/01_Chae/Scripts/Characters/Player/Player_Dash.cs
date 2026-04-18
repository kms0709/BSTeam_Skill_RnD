using UnityEngine;

// ===================================================
// Player_Dash.cs — 대쉬 변수 및 관련 함수
// ===================================================
public partial class Player
{
    [Header("=== Dash ===")]
    public float dashForce;
    public float dashDelayTime;
    public float dashCoolTime;

    [HideInInspector] public bool canDash = true;
    [HideInInspector] public bool isDashing;
    [HideInInspector] public CountDownTimer dashTimer;

    //-- 라이프사이클 훅 (Player.cs 의 Awake/Start/Update 에서 호출) --
    public void DashAwake(){
        dashTimer = CountDownTimer.Create(dashCoolTime, () => canDash = true);
    }
    public void DashStart(){
        dashTimer.StartTimer(0);
    }
    public void DashUpdate(){
        if(!canDash && IsGrounded()){
            dashTimer.Tick(Time.deltaTime);
        }
    }

    //-- Dash Funcs --
    void Dash(){
        if(isDashing) return;

        isDashing = true;
        canDash = false;
        dashTimer.StartTimer(dashCoolTime);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        if(mousePos.x != 0){
            dirX = (mousePos.x > 0) ? 1f : -1f;
        }
        Vector2 dirDash = (mousePos - transform.position).normalized;

        rb.linearVelocity = dirDash * dashForce;
        Invoke(nameof(TriggerDash), dashDelayTime);
    }

    void TriggerDash(){
        isDashing = false;
        rb.linearVelocity = new Vector2(0f, Mathf.Min(rb.linearVelocity.y, 0f));
    }
}
