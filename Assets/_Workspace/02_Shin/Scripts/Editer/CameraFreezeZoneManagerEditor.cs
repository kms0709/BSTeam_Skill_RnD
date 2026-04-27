using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

[CustomEditor(typeof(CameraFreezeZoneManager))]
public class CameraFreezeZoneManagerEditor : Editor
{
    private SerializedProperty freezeZones;
    private bool[] foldouts;

    private void OnEnable()
    {
        freezeZones = serializedObject.FindProperty("freezeZonelist");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        if (foldouts == null || foldouts.Length != freezeZones.arraySize)
        {
            foldouts = new bool[freezeZones.arraySize];
        }

        CameraFreezeZoneManager manager = (CameraFreezeZoneManager)target;

        for (int i = 0; i < freezeZones.arraySize; i++)
        {
            var element = freezeZones.GetArrayElementAtIndex(i);

            // 박스 구역 생성
            EditorGUILayout.BeginVertical("box");

            SerializedProperty name = element.FindPropertyRelative(nameof(FreezeZoneData.name));
            string displayName = string.IsNullOrEmpty(name.stringValue) ? $"구역 {i + 1}" : name.stringValue;
            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], displayName, true);

            bool end = false;
            if (foldouts[i])
            {
                name.stringValue = EditorGUILayout.TextField("이름", name.stringValue);

                EditorGUILayout.Space();

                //stop존 필드
                EditorGUILayout.LabelField("Stop Zone", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(element.FindPropertyRelative(nameof(FreezeZoneData.stopData)), true);

                EditorGUILayout.Space();

                //move존 필드
                EditorGUILayout.LabelField("Move Zone", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(element.FindPropertyRelative(nameof(FreezeZoneData.moveData)), true);

                EditorGUILayout.Space();

                if (GUILayout.Button("삭제"))
                {
                    end = true;
                }

            }

            // 박스 구역 닫기
            EditorGUILayout.EndVertical();

            if (end)
            {
                var zoneData = manager.GetFreezeZoneList()[i];

                Undo.RecordObject(manager, "Delete Freeze Zone");

                // 오브젝트 삭제
                if (zoneData.rootObject != null)
                {
                    // ctrl + z전용 삭제
                    Undo.DestroyObjectImmediate(zoneData.rootObject);
                }

                freezeZones.DeleteArrayElementAtIndex(i);
                break;
            }
        }

        if (GUILayout.Button("구역 추가"))
        {
            CreateFreezeZone(manager);
        }

        EditorGUILayout.Space(20);

        if(GUILayout.Button("설정 적용"))
        {
            manager.ApplyToScene();
        }

        // 수정한 값 오브젝트에 반영 및 저장
        serializedObject.ApplyModifiedProperties();
    }

    // stop, move존 부모 오브젝트 생성
    private void CreateFreezeZone(CameraFreezeZoneManager manager)
    {
        // 구역 오브젝트 생성
        GameObject zone = new GameObject("freezeZone");
        // 위치 기본값
        zone.transform.position = Vector3.zero;

        // 부모 오브젝트 설정
        zone.transform.SetParent(manager.transform);

        FreezeZoneData zoneData = new FreezeZoneData
        {
            rootObject = zone,
            stopData = CreateStopZone(zone, manager),
            moveData = CreateMoveZone(zone, manager)
        };

        // 리스트 추가
        manager.AddPoint(zoneData);

        EditorUtility.SetDirty(zone);
    }

    // stop존 생성
    private StopData CreateStopZone(GameObject zone, CameraFreezeZoneManager manager)
    {
        // Stop 구역 오브젝트 생성
        GameObject stop = new GameObject("stopPoint");
        // 위치 기본값
        stop.transform.position = Vector3.zero;
        // 부모 오브젝트 설정
        stop.transform.SetParent(zone.transform);

        // 컴포넌트 추가
        var trigger = stop.AddComponent<CameraFreezeZoneTrigger>();

        //프리즈 존 프리팹 추가
        var prefab = manager.FreezeZonePrefab;
        var freezeZone = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        // 부모 설정
        freezeZone.transform.SetParent(stop.transform);

        return new StopData { pointSize = Vector3.zero, cameraPosition = Vector3.zero, cameraSize = 0, pointPosition = Vector3.zero};
    }

    // move존 생성
    private MoveData CreateMoveZone(GameObject zone, CameraFreezeZoneManager manager)
    {
        // Move 구역 오브젝트 생성
        GameObject move = new GameObject("movePoint");
        // 위치 기본값
        move.transform.position = Vector3.zero;
        // 부모 오브젝트 설정
        move.transform.SetParent(zone.transform);

        // 컴포넌트 추가
        var trigger = move.AddComponent<CameraUnFreezeZoneTrigger>();

        //프리즈 존 프리팹 추가
        var prefab = manager.FreezeZonePrefab;
        var freezeZone = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

        // 부모 설정
        freezeZone.transform.SetParent(move.transform);

        return new MoveData { size = Vector3.zero, position = Vector3.zero};
    }
}
