using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class _TileChecker : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name != "Test Player")
            return;

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
        Vector2 dir = (Vector2)collision.transform.position - hitPos;
        Vector2 normal = dir.normalized;
        Debug.DrawLine(hitPos, hitPos + normal, Color.magenta, 1f);

        // 각도 구하기
        float rad = Mathf.Atan2(normal.y, normal.x);
        float deg = rad * Mathf.Rad2Deg + 90f;
        Debug.Log(string.Format("부딪힌 각도 : {0}", deg));

        float ramp = 45f;
        float min = 180f - ramp;
        float max = 180f + ramp;

        if (deg < 0) deg += 90f;
        if (min <= deg && deg <= max)
        {
            Debug.Log(string.Format("Player State = GROUND, angle = {0}", deg));
        }
        else
        {
            Debug.Log(string.Format("Player State = WALL OR FALL, angle = {0}", deg));
        }
    }
}
