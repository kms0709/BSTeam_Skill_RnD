using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum ACTOR_STATE
{
    IDLE,
    RUN,
    JUMP,
    NONE
}

public class Test_Player : MonoBehaviour
{
    Rigidbody2D rigidbody;

    [Header("Movement")]
    [Range(0f, 5f)]
    [SerializeField] float speed;

    [Range(1f, 10f)]
    [SerializeField]float maxSpeed;

    [Range(1f, 10f)]
    [SerializeField] float jumpForce;

    public ACTOR_STATE state;


    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        state = ACTOR_STATE.IDLE;
    }

    void Update()
    {
        // 방향키 인풋
        float x = Input.GetAxisRaw("Horizontal");
        //float y = Input.GetAxisRaw("Vertical");

        // 좌, 우 이동
        Vector2 moveVector = new Vector2(x * speed * Time.deltaTime, 0f);
        transform.Translate(moveVector, Space.World);

        // 점프
        if (Input.GetKeyDown(KeyCode.Space)/* && state != ACTOR_STATE.JUMP*/)
        {
            rigidbody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            state = ACTOR_STATE.JUMP;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // 타일에 닿으면 상태 변경
        if (collision.gameObject.name == "Tilemap"
            && state == ACTOR_STATE.JUMP)
        {
            // 나중에 Animation 에서 관리
            state = ACTOR_STATE.IDLE;
        }
    }
}
