using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class _Player : MonoBehaviour
{
    public Rigidbody2D Rigidbody2D
    {
        get; private set;
    }

    [Header("Movement")]
    [Range(0f, 5f)]
    [SerializeField] float speed;

    [Range(1f, 10f)]
    [SerializeField] float maxSpeed;

    [Range(1f, 10f)]
    [SerializeField] float jumpForce;

    [Range(0f, 90f)]
    [SerializeField] float jumpAngle;

    private float inputX = 0;
    private int jumpDirection = 0;

    public float Speed { get { return speed; } }
    public float JumpForce { get { return jumpForce; } }
    public float JumpAngle { get { return jumpAngle; } }
    public int JumpDirection { get { return jumpDirection; } }
    public float InputX { get { return inputX; } }

    public void SetJumpDirection(int val)
    {
        jumpDirection = val;
        if (jumpDirection < -1) jumpDirection = -1;
        if (jumpDirection > 1) jumpDirection = 1;
    }

    // FSM
    //=====================================================

    [SerializeField] _IPlayerState<_Player> currentState;
    Dictionary<string, _IPlayerState<_Player>> states = new Dictionary<string, _IPlayerState<_Player>>();

    public void ChangeState(string key)
    {
        if (!states.ContainsKey(key))
        {
            Debug.LogWarning("키 값이 없습니다.");
            return;
        }

        currentState?.Exit(this);
        currentState = states[key];
        currentState?.Enter(this);
    }

    private void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();

        _IPlayerState<_Player> idle = new _PlayerIdleState();
        _IPlayerState<_Player> run = new _PlayerRunState();
        _IPlayerState<_Player> jump = new _PlayerJumpState();
        _IPlayerState<_Player> wallSlide = new _PlayerWallSlideState();

        states.Add("Idle", idle);
        states.Add("Run", run);
        states.Add("Jump", jump);
        //states.Add("Fall", );
        states.Add("WallSlide", wallSlide);

        ChangeState("Idle");
    }

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        if (inputX != 0f)
        {
            if (currentState == states["Idle"])
                ChangeState("Run");
        }
        else
        {
            if (currentState == states["Run"])
                ChangeState("Idle");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(currentState == states["WallSlide"])
            {
                jumpAngle = 45f;
                ChangeState("Jump");
            }
            else
            {
                jumpAngle = 0f;
                ChangeState("Jump");
            }
        }

        currentState?.Execute(this);
    }
}
