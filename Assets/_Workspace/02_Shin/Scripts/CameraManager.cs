using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraZoneData
{
    public CinemachineVirtualCamera vcam;
    public PolygonCollider2D collider;
    public string zoneName;
}

public class CameraManager : MonoBehaviour
{
    [SerializeField] private List<CameraZoneData> cameraZones = new List<CameraZoneData>();
    
    // 카메라 구역을 인스팩터 에서 만들 수 있어야함
    // 에디터 에서 "카메라 구역 생성" 버튼을 누르면
    //  - 카메라 오브젝트 생성
    //      - 시네머신 버추얼 카메라
    //      - 폴리곤 콜라이더
    //      - CameraZoneTrigger 스크립트
    // 생성하고 cameraZone list에 추가해서 카메라 매니저가 관리를 할 수 있게
    // 
    
    // 리스트에 카메라 구역 추가
    public void AddCamera(CameraZoneData cam)
    {
        cameraZones.Add(cam);
    }

    public List<CameraZoneData> GetCameraZoneList()
    {
        return cameraZones;
    }

    // 에디터 씬 뷰에서 방 구역(Room Zone)들을 직관적으로 그리기 위한 Gizmo
    void OnDrawGizmos()
    {
        foreach (var zone in cameraZones)
        {
            //오브젝트 삭제시 null오류 예방
            if (zone == null || zone.vcam == null) continue;

            Gizmos.color = new Color(0f, 1f, 1f, 0.4f); // 시안색 (반투명)
            
            float height = zone.vcam.m_Lens.OrthographicSize * 2;
            float width = height * Camera.main.aspect;

            Vector3 center = zone.vcam.transform.position;

            Gizmos.DrawWireCube(center, new Vector3(width, height, 0));

            // 실행 중일 때는 현재 활성화 된 방을 다른 색으로 강조
            //if (Application.isPlaying && activeZone == zone)
            //{
            //    Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.8f); // 노란색
            //    Gizmos.DrawWireCube(zone.center, zone.size);
            //}
        }
    }
}
