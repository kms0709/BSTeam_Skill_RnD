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
        cameraZones = serializedObject.FindProperty("cameraZones");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

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
            SerializedProperty vcam = element.FindPropertyRelative(nameof(CameraZoneData.vcam));
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

                if (vcamValue == null)
                {
                    EditorGUILayout.LabelField("카메라 없음");
                }
                else
                {
                    // 사이즈
                    float newSize = EditorGUILayout.FloatField("구역 크기", vcamValue.m_Lens.OrthographicSize);

                    if (!Mathf.Approximately(newSize, vcamValue.m_Lens.OrthographicSize))
                    {
                        Undo.RecordObject(vcamValue, "Change Camera Size");
                        vcamValue.m_Lens.OrthographicSize = newSize;
                        EditorUtility.SetDirty(vcamValue);
                    }

                    // 위치
                    Vector3 currentPos = vcamValue.transform.position;
                    Vector2 newPos2D = EditorGUILayout.Vector2Field("위치", currentPos);
                    Vector3 newPos = new Vector3(newPos2D.x, newPos2D.y, currentPos.z);

                    if (currentPos != newPos)
                    {
                        Undo.RecordObject(vcamValue.transform, "Change Camera Position");
                        vcamValue.transform.position = newPos;
                        EditorUtility.SetDirty(vcamValue.transform);
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

                if(end)
                {
                    // GameObject까지 같이 삭제
                    var vcamObj = vcam.objectReferenceValue as CinemachineVirtualCamera;

                    if (vcamObj != null)
                    {
                        Undo.DestroyObjectImmediate(vcamObj.gameObject);
                    }

                    cameraZones.DeleteArrayElementAtIndex(i);
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
        GameObject zone = new GameObject("CameraZone");

        //레이어 설정
        zone.layer = LayerMask.NameToLayer("MainCameraZone");

        // 위치 기본값
        zone.transform.position = Vector3.zero;

        // 컴포넌트 추가
        var vcam = zone.AddComponent<CinemachineVirtualCamera>();
        var collider = zone.AddComponent<PolygonCollider2D>();
        var trigger = zone.AddComponent<CameraZoneTrigger>();

        // 기본 설정
        // 카메라 설정
        vcam.m_Lens.Orthographic = true;
        vcam.m_Lens.OrthographicSize = 12; // 기본 카메라 사이즈
        float height = vcam.m_Lens.OrthographicSize * 2;
        float width = height * Camera.main.aspect;
        
        if (manager.GetCameraZoneList().Count == 0) vcam.Priority = 10;
        else vcam.Priority = 0;

        // 콜라이더 설정
        collider.isTrigger = true;
        Vector2[] point = new Vector2[4];

        point[0] = new Vector2(-width / 2, height / 2);
        point[1] = new Vector2(width / 2, height / 2);
        point[2] = new Vector2(width / 2, -height / 2);
        point[3] = new Vector2(-width / 2, -height / 2);

        collider.points = point;

        //트리거 설정
        // cameraManager cameraZones리스트에 추가 하기 위한 데이터 초기화
        CameraZoneData cData = new CameraZoneData
        {
            vcam = vcam,
            collider = collider
        };

        // CameraManager 리스트에 추가
        Undo.RecordObject(manager, "Add Camera Zone");
        manager.AddCamera(cData);
        
        zone.transform.SetParent(manager.transform);

        EditorUtility.SetDirty(manager);
    }
}