using Unity.Cinemachine;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Collider2D))]
public class CameraZoneTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera MainVcam; 
    [SerializeField] private CinemachineVirtualCamera SubVcam; 

    // 외부(에디터 스크립트 등)에서 접근해야 한다면 프로퍼티를 따로 만들어줍니다.
    public CinemachineVirtualCamera mainVcam { get => MainVcam; set => MainVcam = value; }
    public CinemachineVirtualCamera subVcam { get => SubVcam; set => SubVcam = value; }

    [Header("우선순위 설정")]
    [SerializeField] private int activePriority = 20;
    [SerializeField] private int idlePriority = 10;

    private PolygonCollider2D pol;

    private void Awake()
    {
        // 런타임에 vCam이 누락되었다면 컴포넌트에서 시도 (mainVCam 위주)
        if (mainVcam == null) mainVcam = GetComponent<CinemachineVirtualCamera>();
        pol = GetComponent<PolygonCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 구역 진입 시 우선순위 상승
            SetPriorities(activePriority);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 구역 이탈 시 우선순위 하락
            SetPriorities(idlePriority);
        }
    }

    // 카메라 우선순위 설정
    private void SetPriorities(int priority)
    {
        if(mainVcam != null) mainVcam.Priority = priority;
        if(subVcam != null) subVcam.Priority = priority;
    }

    private void OnValidate()
    {
        if (pol == null) pol = GetComponent<PolygonCollider2D>();
        if (mainVcam == null) mainVcam = GetComponent<CinemachineVirtualCamera>();
        if (mainVcam != null && pol != null) ColliderSync();
    }

    private void ColliderSync()
    {
        float height = mainVcam.m_Lens.OrthographicSize * 2;
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