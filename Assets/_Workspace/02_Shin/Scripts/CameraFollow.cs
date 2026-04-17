using UnityEngine;
using UnityEngine.Windows.WebCam;

public class CameraFollow : MonoBehaviour
{
    [Header("대상 및 카메라 참조")]
    [SerializeField] private Transform target;      // 따라갈 대상 (플레이어)
    [SerializeField] private Camera mainCam;        // 메인 카메라
    private Camera subCam;                        // 이 스크립트가 붙은 서브 카메라

    [Header("UI 설정 (RawImage)")]
    [SerializeField] private RectTransform pipRawImageRect; // Render Texture를 보여주는 RawImage의 RectTransform
    [SerializeField] private RectTransform canvasRect;      // UI가 속한 Canvas의 RectTransform (좌표 계산용)

    [Header("팔로우 설정")]
    [SerializeField] private float smoothSpeed = 5f; // 월드 추적 시 서브 카메라의 부드러움
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f); // 기본 오프셋

    [Header("월드 유닛 기준 PiP 창 크기")]
    [Tooltip("기본 상태 시 SubCamera 가 보여줄 가로 영역 크기 (World Units)")]
    public float targetScaleX = 10f;
    [Tooltip("기본 상태 시 SubCamera 가 보여줄 세로 영역 크기 (World Units)")]
    public float targetScaleY = 10f;

    [Header("부드러운 전환 (Smooth Time)")]
    [Tooltip("모드 변경 시 UI 창의 크기/위치를 자연스럽게 이어주는 시간입니다.")]
    public float transitionSmoothTime = 0.25f;

    // ─────────────────────────────────────────
    // 내부 변수
    // ─────────────────────────────────────────
    private bool isFrozen = false;
    private Vector3 frozenPosition;

    // SmoothDamp 용 목표값 및 속도 변수
    private Vector2 targetUIPos;
    private Vector2 targetUISize;

    private Vector2 posVel, sizeVel;
    private Vector3 offsetVel;
    private Vector3 currentDynamicOffset;

    private float preSubCamSize;

    void Awake()
    {
        // Debug.unityLogger.logEnabled = false; 디버그 로그 제외
        subCam = GetComponent<Camera>();
        if (subCam == null)
            Debug.LogError("[CameraFollowUI] Camera 컴포넌트가 없습니다.");

        if (pipRawImageRect == null || canvasRect == null)
            Debug.LogError("[CameraFollowUI] UI RectTransform 참조가 누락되었습니다.");

        currentDynamicOffset = offset;

        preSubCamSize = subCam.orthographicSize;
    }

    private void OnEnable()
    {
        CameraFreezeZoneTrigger.OnCameraTriggerEnter += FreezeCamera;
        CameraUnFreezeZoneTrigger.OnFreezeCameraTriggerEnter += UnfreezeCamera;
    }

    private void OnDisable()
    {
        CameraFreezeZoneTrigger.OnCameraTriggerEnter -= FreezeCamera;
        CameraUnFreezeZoneTrigger.OnFreezeCameraTriggerEnter -= UnfreezeCamera;
    }

    void LateUpdate()
    {
        if (target == null || mainCam == null || subCam == null || pipRawImageRect == null || canvasRect == null) return;

        // 1. 계산
        CalculateWorldAndUITargets();

        // 2. 적용
        ApplySmoothTransitions();
    }

    /// <summary>
    /// 월드 카메라 위치와 UI(RawImage)의 목표 위치/크기를 계산합니다.
    /// </summary>
    private void CalculateWorldAndUITargets()
    {
        // 1. 메인 카메라의 월드 영역 정보 (분모가 될 값)
        float mainWorldH = mainCam.orthographicSize * 2f;
        float mainWorldW = mainWorldH * mainCam.aspect;

        // 2. [역발상] 서브 카메라의 현재 사이즈로부터 월드 유닛 크기를 역산
        // 이제 targetScaleX, Y는 우리가 입력하는 게 아니라 서브 카메라가 비추는 실제 크기가 됩니다.
        targetScaleY = subCam.orthographicSize * 2f;
        targetScaleX = targetScaleY * subCam.aspect;

        // 3. 월드 카메라 위치 결정
        Vector3 targetWorldPos;
        if (isFrozen)
        {
            targetWorldPos = frozenPosition;
        }
        else
        {
            // 오프셋 부드럽게 적용 (기존 로직 유지)
            currentDynamicOffset = Vector3.SmoothDamp(currentDynamicOffset, offset, ref offsetVel, transitionSmoothTime);
            targetWorldPos = target.position + currentDynamicOffset;
        }

        // 서브 카메라 위치 적용
        transform.position = targetWorldPos;

        // 4. UI (RawImage) 위치 및 크기 계산
        // 서브 카메라가 커지면 targetScale이 커지므로, uiW와 uiH도 자동으로 커집니다.
        float uiW = (targetScaleX / mainWorldW) * canvasRect.rect.width;
        float uiH = (targetScaleY / mainWorldH) * canvasRect.rect.height;
        targetUISize = new Vector2(uiW, uiH);


        // 2. UI 위치 계산: 플레이어의 월드 위치 -> 메인 카메라의 뷰포트 좌표 -> 캔버스 좌표
        // 동결 모드일 때는 방의 중앙을 기준으로, 팔로우 모드일 때는 플레이어를 기준으로 UI 위치를 잡습니다.
        Vector3 lookAtTarget = isFrozen ? frozenPosition : target.position;
        Vector3 viewportPos = mainCam.WorldToViewportPoint(lookAtTarget);

        // 뷰포트(0~1)를 캔버스 좌표계로 변환 (0~CanvasWidth, 0~CanvasHeight)
        float uiX = viewportPos.x * canvasRect.rect.width;
        float uiY = viewportPos.y * canvasRect.rect.height;

        // UI 창의 피벗이 중앙이라고 가정할 때, 화면 밖으로 완전히 나가지 않게 클램핑
        float halfW = uiW * 0.5f;
        float halfH = uiH * 0.5f;
        uiX = Mathf.Clamp(uiX, halfW, canvasRect.rect.width - halfW);
        uiY = Mathf.Clamp(uiY, halfH, canvasRect.rect.height - halfH);

        targetUIPos = new Vector2(uiX, uiY);
    }

    /// <summary>
    /// 계산된 목표값들을 RawImage에 부드럽게 적용합니다.
    /// </summary>
    private void ApplySmoothTransitions()
    {
        if (transitionSmoothTime > 0f)
        {
            // UI 위치 부드럽게 이동
            pipRawImageRect.anchoredPosition = Vector2.SmoothDamp(pipRawImageRect.anchoredPosition, targetUIPos, ref posVel, transitionSmoothTime);
            // UI 크기 부드럽게 조절
            pipRawImageRect.sizeDelta = Vector2.SmoothDamp(pipRawImageRect.sizeDelta, targetUISize, ref sizeVel, transitionSmoothTime);
        }
        else
        {
            pipRawImageRect.anchoredPosition = targetUIPos;
            pipRawImageRect.sizeDelta = targetUISize;
        }
    }

    public void FreezeCamera(float size, Vector3 pos)
    {
        if (isFrozen) return;
        frozenPosition = new Vector3(pos.x, pos.y, transform.position.z);
        subCam.orthographicSize = size;
        isFrozen = true;
    }

    public void UnfreezeCamera()
    {
        subCam.orthographicSize = preSubCamSize;
        isFrozen = false;
    }
}