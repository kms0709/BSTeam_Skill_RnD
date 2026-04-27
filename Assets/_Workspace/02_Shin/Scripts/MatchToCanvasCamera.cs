using UnityEngine;

public class MatchToCanvasCamera : MonoBehaviour
{
    public Camera targetCamera;
    public float distance = 10f; // 카메라로부터의 거리

    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update나 LateUpdate에서 카메라 변화에 맞춰 크기 조정
    void LateUpdate()
    {
        if (targetCamera == null || rectTransform == null) return;

        float height;
        float width;

        if (targetCamera.orthographic)
        {
            // 1. 직교 카메라 (Orthographic)
            height = targetCamera.orthographicSize * 2f;
            width = height * targetCamera.aspect;
        }
        else
        {
            // 2. 원근 카메라 (Perspective)
            // 수식: 2 * 거리 * tan(FOV / 2)
            height = 2.0f * distance * Mathf.Tan(targetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            width = height * targetCamera.aspect;
        }

        // 캔버스 사이즈 설정
        rectTransform.sizeDelta = new Vector2(width, height);

        // 카메라 정면의 지정된 거리만큼 위치시키고 방향을 맞춤
        transform.position = targetCamera.transform.position + targetCamera.transform.forward * distance;
        transform.rotation = targetCamera.transform.rotation;
    }
}
