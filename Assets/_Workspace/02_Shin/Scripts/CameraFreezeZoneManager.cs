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
    [SerializeField] private GameObject freezeZonePrefab;
    public GameObject FreezeZonePrefab => freezeZonePrefab;

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
            Transform boundary = zone.rootObject.transform.Find("CameraBoundary");

            // Stop 적용
            if (stop != null)
            {
                var trigger = stop.GetComponent<CameraFreezeZoneTrigger>();
                if (trigger != null)
                {
                    trigger.cameraZonePosition = zone.stopData.cameraPosition;// 고정 카메라 위치 설정
                    trigger.cameraZoneSize = zone.stopData.cameraSize;        // 고정 카메라 사이즈 설정
                }

                if (boundary != null)
                {
                    trigger.cameraBoundary = boundary.gameObject;

                    // 기본적으로 벽은 꺼둡니다 (플레이어가 진입할 때만 켜짐)
                    boundary.gameObject.SetActive(false);
                    Debug.Log("벽 설정 완료");
                }
                stop.localPosition = zone.stopData.pointPosition;
            }

            // Move 적용
            if (move != null)
            {
                var trigger = move.GetComponent<CameraUnFreezeZoneTrigger>();

                if (boundary != null)
                {
                    trigger.cameraBoundary = boundary.gameObject;

                    // 기본적으로 벽은 꺼둡니다 (플레이어가 진입할 때만 켜짐)
                    Debug.Log("비활성화 벽 설정 완료");
                }

                move.localPosition = zone.moveData.position; // 해제 트리거 위치 설정
            }
            


            if (boundary != null)
            {
                // 1. 경계 오브젝트의 위치를 카메라 고정 위치와 일치시킴
                boundary.position = zone.stopData.cameraPosition;

                var edgeCol = boundary.GetComponent<EdgeCollider2D>();
                if (edgeCol != null)
                {
                    // 2. 카메라 절반 높이(halfHeight)와 절반 너비(halfWidth) 계산
                    float halfHeight = zone.stopData.cameraSize;
                    float halfWidth = halfHeight * Camera.main.aspect;

                    // 3. 네 꼭짓점 정의 (로컬 좌표계 기준)
                    // 시계 방향 혹은 반시계 방향으로 순서대로 연결해야 합니다.
                    Vector2[] points = new Vector2[5];

                    points[0] = new Vector2(-halfWidth, -halfHeight); // 좌측 하단
                    points[1] = new Vector2(-halfWidth, halfHeight);  // 좌측 상단
                    points[2] = new Vector2(halfWidth, halfHeight);   // 우측 상단
                    points[3] = new Vector2(halfWidth, -halfHeight);  // 우측 하단
                    points[4] = points[0];                            // 다시 좌측 하단 (폐쇄 루프)

                    // 4. 엣지 콜라이더에 점 할당
                    edgeCol.points = points;
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
