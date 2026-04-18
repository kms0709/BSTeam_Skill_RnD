using UnityEditor;
using UnityEngine;
using Cinemachine;

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
                var vcamValue = vcam.objectReferenceValue as CinemachineVirtualCamera;
                var subVcamValue = subVcam.objectReferenceValue as CinemachineVirtualCamera;

                if (vcamValue == null)
                {
                    EditorGUILayout.LabelField("카메라 없음");
                }
                else
                {
                    EditorGUILayout.LabelField("메인 카메라", EditorStyles.boldLabel);
                    // 사이즈
                    float newSize = EditorGUILayout.FloatField("구역 크기", vcamValue.m_Lens.OrthographicSize);

                    if (!Mathf.Approximately(newSize, vcamValue.m_Lens.OrthographicSize))
                    {
                        Undo.RecordObject(vcamValue, "Change Camera Size");
                        vcamValue.m_Lens.OrthographicSize = newSize;
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
                        float newSubSize = EditorGUILayout.FloatField("화면 크기", subVcamValue.m_Lens.OrthographicSize);

                        if (!Mathf.Approximately(newSubSize, subVcamValue.m_Lens.OrthographicSize))
                        {
                            Undo.RecordObject(subVcamValue, "Change SubCamera Size");
                            subVcamValue.m_Lens.OrthographicSize = newSubSize;
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
        // 구역 오브젝트 생성
        GameObject zone = new GameObject("CameraZoneRoot");

        // 위치 기본값
        zone.transform.position = Vector3.zero;
        zone.transform.SetParent(manager.transform);

        // 부모 오브젝트에 콜라이더 및 트리거 체크 추가
        
        

        //// 메인 카메라
        // 메인 카메라 오브젝트 생성
        GameObject mainCamZone = new GameObject("MainCameraZone");

        // 부모 오브젝트 설정
        mainCamZone.transform.SetParent(zone.transform);

        // 컴포넌트 추가
        var vcam = mainCamZone.AddComponent<CinemachineVirtualCamera>();
        var collider = mainCamZone.AddComponent<PolygonCollider2D>();
        var trigger = mainCamZone.AddComponent<CameraZoneTrigger>();

        //레이어 설정
        mainCamZone.layer = LayerMask.NameToLayer("MainCameraZone");

        // 기본 설정
        // 카메라 설정
        vcam.m_Lens.Orthographic = true;
        vcam.m_Lens.OrthographicSize = 12; // 기본 카메라 사이즈

        //// 서브 카메라
        // 서브 카메라 오브젝트 생성
        GameObject subCam = new GameObject("subCamera");

        // 부모 오브젝트 설정
        subCam.transform.SetParent(zone.transform);

        var subVcam = subCam.AddComponent<CinemachineVirtualCamera>();
        var subVcamBody = subVcam.AddCinemachineComponent<CinemachineFramingTransposer>();
        var subVcamConfiner = subCam.AddComponent<CinemachineConfiner2D>();
        subVcam.m_Lens.Orthographic = true;
        subVcam.m_Lens.OrthographicSize = 2.5f;

        subCam.layer = LayerMask.NameToLayer("GameScreen");

        subVcam.m_StandbyUpdate = CinemachineVirtualCameraBase.StandbyUpdateMode.Never;

        // 부드러운 움직임 삭제
        subVcamBody.m_XDamping = 0;
        subVcamBody.m_YDamping = 0;

        // 서브 카메라 제한 구역 설정
        subVcamConfiner.m_BoundingShape2D = collider;
        subVcam.m_Follow = manager.Player;

        // 카메라 우선순위 설정
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

        // 콜라이더 설정
        collider.isTrigger = true;
        Vector2[] point = new Vector2[4];

        float height = vcam.m_Lens.OrthographicSize * 2;
        float width = height * Camera.main.aspect;

        point[0] = new Vector2(-width / 2, height / 2);
        point[1] = new Vector2(width / 2, height / 2);
        point[2] = new Vector2(width / 2, -height / 2);
        point[3] = new Vector2(-width / 2, -height / 2);

        collider.points = point;

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