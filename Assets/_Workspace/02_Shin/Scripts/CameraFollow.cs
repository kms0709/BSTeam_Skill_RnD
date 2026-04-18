using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("대상 및 카메라 참조")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private CinemachineBrain subCamBrain;

    [Header("고정용 카메라 설정")]
    [SerializeField] private CinemachineVirtualCamera freezeVCam; // 고정 전용 VCam
    [SerializeField] private int activePriority = 100;   // 활성화 시 우선순위
    [SerializeField] private int inactivePriority = 0; // 비활성화 시 우선순위

    [Header("UI 설정 (RawImage)")]
    [SerializeField] private RectTransform pipRawImageRect; // Render Texture를 보여주는 RawImage의 RectTransform
    [SerializeField] private RectTransform canvasRect;      // UI가 속한 Canvas의 RectTransform (좌표 계산용)
    [SerializeField] private float transitionSmoothTime = 0.25f;
    
    // 내부 변수
    private bool isFrozen = false;

    // SmoothDamp 용 목표값 및 속도 변수
    private Vector2 targetUIPos;
    private Vector2 targetUISize;
    private Vector2 posVel, sizeVel;

    //현재 서브 버추얼 카메라
    private ICinemachineCamera currentActiveVCam;

    void Awake()
    {
        // Debug.unityLogger.logEnabled = false; 디버그 로그 제외

        if (pipRawImageRect == null || canvasRect == null) Debug.LogError("[CameraFollowUI] UI RectTransform 참조가 누락되었습니다.");

        if(subCamBrain == null) subCamBrain = subCamBrain.GetComponent<CinemachineBrain>();
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
        // NULL 예외 방지
        if (subCamBrain == null || mainCam == null) return;
        currentActiveVCam = subCamBrain.ActiveVirtualCamera;
        if (currentActiveVCam == null) return;

        UpdateUIMetrics();
        ApplySmoothTransitions();
    }

    /// <summary>
    /// 월드 카메라 위치와 UI(RawImage)의 목표 위치/크기를 계산합니다.
    /// </summary>
    private void UpdateUIMetrics()
    {
        // 시네머신이 계산한 최종 렌즈 정보와 위치를 가져옴
        // (Follow -> Freeze로 변하는 도중의 중간값도 실시간으로 반영됨)
        LensSettings lens = currentActiveVCam.State.Lens;

        // 1. 메인 카메라의 월드 영역 정보 (분모가 될 값)
        float mainWorldH = mainCam.orthographicSize * 2f;
        float mainWorldW = mainWorldH * mainCam.aspect;

        // 2. 현재 활성화된 카메라의 상태(State)에서 렌즈 정보를 가져옵니다.
        // 이 방식은 vCam이 여러 개여도 현재 '찍고 있는' 카메라의 값을 가져옵니다.
        float targetScaleY = lens.OrthographicSize * 2f;
        float targetScaleX = targetScaleY * lens.Aspect;

        // 4. UI (RawImage) 위치 및 크기 계산
        // 서브 카메라가 커지면 targetScale이 커지므로, uiW와 uiH도 자동으로 커집니다.
        float uiW = (targetScaleX / mainWorldW) * canvasRect.rect.width;
        float uiH = (targetScaleY / mainWorldH) * canvasRect.rect.height;
        targetUISize = new Vector2(uiW, uiH);


        // 2. UI 위치 계산: 플레이어의 월드 위치 -> 메인 카메라의 뷰포트 좌표 -> 캔버스 좌표
        // 동결 모드일 때는 방의 중앙을 기준으로, 팔로우 모드일 때는 플레이어를 기준으로 UI 위치를 잡습니다.
        //Vector3 lookAtTarget = isFrozen ? frozenPosition : target.position;
        // 3. 현재 카메라의 월드 위치 (전환 중일 때의 중간 위치도 포함됨)
        Vector3 currentCamWorldPos = currentActiveVCam.State.FinalPosition;
        Vector3 viewportPos = mainCam.WorldToViewportPoint(currentCamWorldPos);

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

    // 트리거에서 호출됨
    public void FreezeCamera(float size, Vector3 pos)
    {
        if (isFrozen || freezeVCam == null) return;

        // 1. 고정 카메라의 위치와 렌즈 크기를 즉시 설정
        freezeVCam.transform.position = new Vector3(pos.x, pos.y, -10);
        freezeVCam.m_Lens.OrthographicSize = size;

        // 2. 우선순위를 높여서 시네머신이 이 카메라를 선택하게 만듦
        freezeVCam.Priority = activePriority;
        isFrozen = true;
    }

    public void UnfreezeCamera()
    {
        if (!isFrozen || freezeVCam == null) return;

        // 우선순위를 낮춰서 다시 원래의 Follow 카메라로 돌아가게 만듦
        freezeVCam.Priority = inactivePriority;
        isFrozen = false;
    }
}