using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("기본 플레이어 카메라")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;

    [Header("플레이어 추적 대상")]
    [SerializeField] private Transform playerTarget;

    private Dictionary<CameraPoint, CinemachineVirtualCamera> pointCameraMap
        = new Dictionary<CameraPoint, CinemachineVirtualCamera>();

    private CinemachineVirtualCamera currentCamera;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CreateCameras();
        SetPlayerCamera();
    }

    private void CreateCameras()
    {
        CameraPoint[] points = FindObjectsOfType<CameraPoint>();

        foreach (CameraPoint point in points)
        {
            GameObject camObj = new GameObject($"VCam_{point.name}");
            camObj.transform.SetParent(transform);

            CinemachineVirtualCamera vcam = camObj.AddComponent<CinemachineVirtualCamera>();
            
            // 시네머신 body 설정이 필요할때
            //var body = vcam.GetCinemachineComponent<CinemachineTransposer>();
            var noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            vcam.Priority = 0;

            // 카메라 위치를 CameraPoint 위치로
            Vector3 pointPos = point.transform.position;
            pointPos.z = -10;            
            vcam.transform.position = pointPos;
            vcam.transform.rotation = point.transform.rotation;

            // 카메라 사이즈 설정
            vcam.m_Lens.OrthographicSize = point.cameraSize;

            // todo : 코루틴으로 만들어서 원하는 만큼만 흔들릴지 아니면 계속 흔들리게 할지
            //noise.m_NoiseProfile = NoiseSettings 

            // Body과련 설정
            // 
            //body.m_LookaheadTime = 
            //body.m_XDamping = 

            // Follow 지정
            if (point.followTarget != null) vcam.Follow = point.followTarget;

            pointCameraMap.Add(point, vcam);
        }
    }

    // 트리거 충돌시 지정된 포인트로 카메라 이동
    public void ActivatePoint(CameraPoint point)
    {
        if (!pointCameraMap.ContainsKey(point)) return;

        if (currentCamera != null) currentCamera.Priority = 0;

        CinemachineVirtualCamera targetCam = pointCameraMap[point];
        targetCam.Priority = point.priority;
        currentCamera = targetCam;
    }

    // 트리거 탈출시 다시 플레이어로 카메라 이동
    public void SetPlayerCamera()
    {
        if (currentCamera != null) currentCamera.Priority = 0;

        playerCamera.Follow = playerTarget;
        playerCamera.Priority = 100;

        currentCamera = playerCamera;
    }
}