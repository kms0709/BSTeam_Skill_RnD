using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class _TileChecker : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != "Test Player")
            return;

        _Player player = collision.gameObject.GetComponent<_Player>();

        // 타일들의 중간 값 찾기
        Vector2 hitPos = Vector3.zero;

        // 부딪힌 타일들 위치 더하기
        foreach (ContactPoint2D hit in collision.contacts)
        {
            hitPos += hit.point;
        }

        // 부딪힌 곳의 중간 값
        hitPos /= collision.contacts.Length;

        // 충돌 위치와 플레이어의 좌표를 뺀다.
        Vector2 dir = hitPos - (Vector2)collision.transform.position;
        Vector2 normal = dir.normalized;
        Debug.DrawLine(hitPos, hitPos + normal, Color.red, 1f);

        // 각도 구하기
        float rad = Mathf.Atan2(normal.y, normal.x);
        float deg = rad * Mathf.Rad2Deg;

        // 바닥 -90
        // 왼쪽 -180
        // 오른쪽 0
        // 천장 90

        float margin = 10f;
        // 바닥
        if (deg > -90f - margin && deg < -90f + margin)
        {
            player.ChangeState("Idle");
        }

        // 왼쪽 벽
        if (deg > -180f - margin && deg < -180f + margin)
        {
            player.ChangeState("WallSlide");
            player.SetJumpDirection(1);
        }

        // 오른쪽 벽
        if (deg > 0f - margin && deg < 0f + margin)
        {
            player.ChangeState("WallSlide");
            player.SetJumpDirection(-1);
        }

        // 플레이어가 타일 각도 받아서 조절 해야 할 듯.
        // + Ray 쏴서 2차 확인

        Debug.Log(string.Format("angle = {0}", deg));
    }
}
