using UnityEditor;
using UnityEngine;
using Unity.Cinemachine;

[CustomEditor(typeof(CameraManager))]
public class CameraManagerEditor : Editor
{
    private SerializedProperty cameraZones;
    private bool[] foldouts;

    private void OnEnable()
    {
        // 카메라 매니저의 카메라 리스트 불러오기
        cameraZones = serializedObject.FindProperty("cameraZones");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        if (foldouts == null || foldouts.Length != cameraZones.arraySize)
        {
            foldouts = new bool[cameraZones.arraySize];
        }

        CameraManager manager = (CameraManager)target;

        EditorGUILayout.Space(10);

        for (int i = 0; i < cameraZones.arraySize; i++)
        {
            SerializedProperty element = cameraZones.GetArrayElementAtIndex(i);
            SerializedProperty name = element.FindPropertyRelative(nameof(CameraZoneData.zoneName));
            SerializedProperty vcam = element.FindPropertyRelative(nameof(CameraZoneData.mainVCam));
            SerializedProperty subVcam = element.FindPropertyRelative(nameof(CameraZoneData.subVCam));
            SerializedProperty collider = element.FindPropertyRelative(nameof(CameraZoneData.collider));

            string displayName = string.IsNullOrEmpty(name.stringValue) ? $"카메라 {i + 1}" : name.stringValue;

            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], displayName, true);

            if (foldouts[i])
            {
                EditorGUILayout.BeginVertical("box");

                // 구역 이름 설정
                EditorGUILayout.LabelField(displayName, EditorStyles.boldLabel);
                name.stringValue = EditorGUILayout.TextField("이름", name.stringValue);

                EditorGUILayout.Space(5);

                // 카메라 위치, 사이즈 설정
                var vcamValue = vcam.objectReferenceValue as CinemachineCamera;
                var subVcamValue = subVcam.objectReferenceValue as CinemachineCamera;

                if (vcamValue == null)
                {
                    EditorGUILayout.LabelField("카메라 없음");
                }
                else
                {
                    EditorGUILayout.LabelField("메인 카메라", EditorStyles.boldLabel);
                    // 사이즈
                    float newSize = EditorGUILayout.FloatField("구역 크기", vcamValue.Lens.OrthographicSize);

                    if (!Mathf.Approximately(newSize, vcamValue.Lens.OrthographicSize))
                    {
                        Undo.RecordObject(vcamValue, "Change Camera Size");
                        vcamValue.Lens.OrthographicSize = newSize;
                        EditorUtility.SetDirty(vcamValue);

                        // 콜라이더도 동시에 크기 업데이트
                        var col = collider.objectReferenceValue as PolygonCollider2D;

                        if (col != null)
                        {
                            float height = newSize * 2;
                            float width = height * Camera.main.aspect;

                            Vector2[] point = new Vector2[4];
                            point[0] = new Vector2(-width / 2, height / 2);
                            point[1] = new Vector2(width / 2, height / 2);
                            point[2] = new Vector2(width / 2, -height / 2);
                            point[3] = new Vector2(-width / 2, -height / 2);

                            Undo.RecordObject(col, "Update Collider Size");
                            col.points = point;
                            EditorUtility.SetDirty(col);
                        }
                    }

                    // 위치
                    Vector3 currentPos = vcamValue.transform.position;
                    Vector2 newPos2D = EditorGUILayout.Vector2Field("위치", currentPos);
                    Vector3 newPos = new Vector3(newPos2D.x, newPos2D.y, currentPos.z);

                    if (currentPos != newPos)
                    {
                        Vector3 delta = newPos - currentPos;

                        Undo.RecordObject(vcamValue.transform, "Change Camera Position");
                        vcamValue.transform.position = newPos;

                        var col = collider.objectReferenceValue as PolygonCollider2D;
                        if (col != null)
                        {
                            Undo.RecordObject(col.transform, "Move Collider");
                            col.transform.position += delta;
                        }

                        EditorUtility.SetDirty(vcamValue.transform);
                    }

                    EditorGUILayout.Space(10);

                    if (subVcamValue == null)
                    {
                        EditorGUILayout.LabelField("서브 카메라 없음");
                    }
                    else
                    {
                        //서브 카메라
                        EditorGUILayout.LabelField("서브 카메라", EditorStyles.boldLabel);
                        // 사이즈 설정
                        float newSubSize = EditorGUILayout.FloatField("화면 크기", subVcamValue.Lens.OrthographicSize);

                        if (!Mathf.Approximately(newSubSize, subVcamValue.Lens.OrthographicSize))
                        {
                            Undo.RecordObject(subVcamValue, "Change SubCamera Size");
                            subVcamValue.Lens.OrthographicSize = newSubSize;
                            EditorUtility.SetDirty(subVcamValue);
                        }
                    }
                }

                EditorGUILayout.Space(5);

                bool end = false;
                // 구역 삭제 버튼 - 리스트에서도 삭제
                if (GUILayout.Button("삭제"))
                {
                    end = true;
                }

                EditorGUILayout.EndVertical();


                if (end)
                {
                    var rootProp = element.FindPropertyRelative(nameof(CameraZoneData.camRoot));
                    var rootObj = rootProp.objectReferenceValue as GameObject;

                    if (rootObj != null)
                    {
                        Undo.DestroyObjectImmediate(rootObj);
                    }

                    cameraZones.DeleteArrayElementAtIndex(i);
                    serializedObject.ApplyModifiedProperties();

                    break;
                }

                EditorGUILayout.Space(5);
            }
        }

        EditorGUILayout.Space(10);

        // 구역 추가 버튼
        if (GUILayout.Button("카메라 추가"))
        {
            CreateCameraZone(manager);
        }

        // 수정한 값 오브젝트에 반영 및 저장
        serializedObject.ApplyModifiedProperties();
    }

    private void CreateCameraZone(CameraManager manager)
    {
        // 부모 오브젝트 생성
        GameObject zone = new GameObject("CameraZoneRoot");
        zone.transform.position = Vector3.zero;
        zone.transform.SetParent(manager.transform);

        // 메인 카메라
        GameObject mainCamZone = new GameObject("MainCameraZone");
        mainCamZone.transform.SetParent(zone.transform);

        // NEW
        var vcam = mainCamZone.AddComponent<CinemachineCamera>();

        // 콜라이더 + 트리거
        var collider = mainCamZone.AddComponent<PolygonCollider2D>();
        var trigger = mainCamZone.AddComponent<CameraZoneTrigger>();

        // Orthographic은 이제 Camera가 담당
        if (Camera.main != null) Camera.main.orthographic = true;

        // 사이즈 설정 (여전히 가능)
        vcam.Lens.OrthographicSize = 12;

        // 서브 카메라
        GameObject subCam = new GameObject("SubCamera");
        subCam.transform.SetParent(zone.transform);

        // NEW
        var subVcam = subCam.AddComponent<CinemachineCamera>();
        var follow = subCam.AddComponent<CinemachineFollow>();

        //채널 설정
        subVcam.OutputChannel = OutputChannels.Channel01;

        // Follow
        subVcam.Follow = manager.Player;

        // 사이즈
        subVcam.Lens.OrthographicSize = 2.5f;

        // damping설정
        follow.TrackerSettings.PositionDamping = Vector3.zero;

        // Confiner (Unity 6 방식)
        var confiner = subCam.AddComponent<CinemachineConfiner2D>();
        confiner.BoundingShape2D = collider; // 이름 바뀐 경우 inspector 확인 필요

        // Priority
        if (manager.GetCameraZoneList().Count == 0)
        {
            vcam.Priority = 20;
            subVcam.Priority = 20;
        }
        else
        {
            vcam.Priority = 0;
            subVcam.Priority = 0;
        }

        // Collider 설정
        collider.isTrigger = true;

        float height = vcam.Lens.OrthographicSize * 2;
        float width = height * (Camera.main != null ? Camera.main.aspect : 1.777f);

        Vector2[] point = new Vector2[4];
        point[0] = new Vector2(-width / 2, height / 2);
        point[1] = new Vector2(width / 2, height / 2);
        point[2] = new Vector2(width / 2, -height / 2);
        point[3] = new Vector2(-width / 2, -height / 2);

        collider.points = point;

        //상위 코드에서 에러로 임시 주석
        // 트리거 설정
        trigger.mainVcam = vcam;
        trigger.subVcam = subVcam;

        // cameraManager cameraZones리스트에 추가 하기 위한 데이터 초기화
        CameraZoneData cData = new CameraZoneData
        {
            camRoot = zone,
            mainVCam = vcam,
            subVCam = subVcam,
            collider = collider
        };

        // CameraManager 리스트에 추가
        Undo.RecordObject(manager, "Add Camera Zone");
        manager.AddCamera(cData);
        
        EditorUtility.SetDirty(manager);
        
    }
}