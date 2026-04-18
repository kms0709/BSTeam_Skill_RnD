using Unity.Cinemachine;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraZoneData
{
    public string zoneName;

    public CinemachineVirtualCamera mainVCam;
    public CinemachineVirtualCamera subVCam;
    public GameObject camRoot;

    public PolygonCollider2D collider;
    
}

public class CameraManager : MonoBehaviour
{
    [SerializeField] private List<CameraZoneData> cameraZones = new List<CameraZoneData>();

    [SerializeField] private Transform player;
    public Transform Player => player;

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
            if (zone == null || zone.mainVCam == null || zone.subVCam == null) continue;

            Gizmos.color = new Color(0f, 1f, 1f, 0.4f); // 시안색 (반투명)
            
            float height = zone.mainVCam.m_Lens.OrthographicSize * 2;
            float width = height * Camera.main.aspect;

            Vector3 center = zone.mainVCam.transform.position;

            Gizmos.DrawWireCube(center, new Vector3(width, height, 0));

            Gizmos.color = new Color(1f, 0.5f, 1f, 0.4f); // 시안색 (반투명)

            float subHeight = zone.subVCam.m_Lens.OrthographicSize * 2;
            float subWidth = subHeight * Camera.main.aspect;

            Vector3 subCenter = zone.subVCam.transform.position;

            Gizmos.DrawWireCube(subCenter, new Vector3(subWidth, subHeight, 0));
        }
    }
}
