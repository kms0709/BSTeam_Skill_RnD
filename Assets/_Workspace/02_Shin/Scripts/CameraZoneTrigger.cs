using Cinemachine;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Collider2D))]
public class CameraZoneTrigger : MonoBehaviour
{
    
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int idlePriority = 10;

    private CinemachineVirtualCamera vCam;
    private PolygonCollider2D pol;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            vCam.Priority = activePriority; // 구역 진입 시 우선순위 상승
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            vCam.Priority = idlePriority; // 구역 이탈 시 우선순위 하락
        }
    }

    private void OnValidate()
    {
        if (pol == null) pol = GetComponent<PolygonCollider2D>();
        if(vCam == null) vCam = GetComponent<CinemachineVirtualCamera>();

        colliderSync();
    }

    private void colliderSync()
    {
        float height = vCam.m_Lens.OrthographicSize * 2;
        float width = height * Camera.main.aspect;

        // 콜라이더 설정
        Vector2[] point = new Vector2[4];

        point[0] = new Vector2(-width / 2, height / 2);
        point[1] = new Vector2(width / 2, height / 2);
        point[2] = new Vector2(width / 2, -height / 2);
        point[3] = new Vector2(-width / 2, -height / 2);

        pol.points = point;
    }
}