using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FreezeZoneData
{
    public string name;
    public StopData stopData;
    public MoveData moveData;
    public GameObject rootObject; // 오브젝트 삭제용 경로 저장
}

[System.Serializable]
public class StopData
{
    public Vector2 pointSize;
    public Vector2 pointPosition;
    public Vector3 cameraPosition;
    public float cameraSize;
}

[System.Serializable]
public class MoveData
{
    public Vector2 size;
    public Vector2 position;
}

public class CameraFreezeZoneManager : MonoBehaviour
{
    [SerializeField] private List<FreezeZoneData> freezeZonelist = new List<FreezeZoneData>();
    
    // 리스트에 frezze 구역 추가
    public void AddPoint(FreezeZoneData zone)
    {
        freezeZonelist.Add(zone);
    }

    public List<FreezeZoneData> GetFreezeZoneList()
    {
        return freezeZonelist;
    }

    // 변경 사항 저장
    public void ApplyToScene()
    {
        foreach (var zone in freezeZonelist)
        {
            if (zone == null || zone.rootObject == null)
                continue;

            // todo : 오브젝트 이름 하드 코딩 방식 교체 필요
            Transform stop = zone.rootObject.transform.Find("stopPoint");
            Transform move = zone.rootObject.transform.Find("movePoint");

            // Stop 적용
            if (stop != null)
            {
                var col = stop.GetComponent<BoxCollider2D>();

                stop.localPosition = zone.stopData.pointPosition;

                if (col != null)
                {
                    col.size = zone.stopData.pointSize; // 트리거 사이즈 설정
                    col.offset = Vector2.zero;
                }

                // 카메라 값도 여기서 넘겨줘도 됨
                var trigger = stop.GetComponent<CameraFreezeZoneTrigger>();
                if (trigger != null)
                {
                    trigger.cameraZonePosition = zone.stopData.cameraPosition;// 고정 카메라 위치 설정
                    trigger.cameraZoneSize = zone.stopData.cameraSize;        // 고정 카메라 사이즈 설정
                }
            }

            // Move 적용
            if (move != null)
            {
                var col = move.GetComponent<BoxCollider2D>();

                move.localPosition = zone.moveData.position; // 해제 트리거 위치 설정

                if (col != null)
                {
                    col.size = zone.moveData.size; // 해제 트리거 사이즈 설정
                    col.offset = Vector2.zero;
                }
            }
        }
    }

    // 에디터 씬 뷰에서 stop, move 구역들을 직관적으로 그리기 위한 Gizmo
    void OnDrawGizmos()
    {
        foreach (var zone in freezeZonelist)
        {
            //오브젝트 삭제시 null오류 예방
            if (zone == null) continue;

            Gizmos.color = Color.red; // 빨간색
            // stop 포인트 기즈모 그리기
            Gizmos.DrawWireCube(zone.stopData.pointPosition, new Vector3(zone.stopData.pointSize.x, zone.stopData.pointSize.y, 0));

            Gizmos.color = Color.blue; // 파란색
            // move 포인트 기즈모 그리기
            Gizmos.DrawWireCube(zone.moveData.position, new Vector3(zone.moveData.size.x, zone.moveData.size.y, 0));

            float height = zone.stopData.cameraSize * 2;
            float width = height * Camera.main.aspect;

            // 고정 카메라 크기 및 위치 기즈모
            Gizmos.color = Color.green; // 파란색
            Gizmos.DrawWireCube(zone.stopData.cameraPosition, new Vector3(width, height, 0));


        }
    }
}
