//using Cinemachine;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(CameraManager))]
//public class CameraManagerEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        CameraManager manager = (CameraManager)target;

//        GUILayout.Space(10);

//        if (GUILayout.Button("카메라 구역 추가"))
//        {
//            AddZone(manager);
//        }

//        GUILayout.Space(10);

//        var zones = manager.GetComponentsInChildren<CameraZone>();

//        foreach (var zone in zones)
//        {
//            EditorGUILayout.BeginVertical("box");

//            EditorGUILayout.LabelField(zone.zoneName, EditorStyles.boldLabel);
//            zone.zoneName = EditorGUILayout.TextField(zone.zoneName);

//            if (zone.vcam != null)
//            {
//                float size = zone.vcam.m_Lens.OrthographicSize;
//                float newSize = EditorGUILayout.FloatField("카메라 사이즈", size);

//                if (newSize != size)
//                {
//                    zone.vcam.m_Lens.OrthographicSize = newSize;
//                }

//                int order = zone.vcam.m_Priority;
//                int newOrder = EditorGUILayout.IntField("카메라 우선순위", order);

//                if (newOrder != order)
//                {
//                    zone.vcam.m_Priority = newOrder;
//                    EditorUtility.SetDirty(zone.vcam);
//                }
//            }

//            // Polygon Transform 이동
//            if (zone.confiner != null)
//            {
//                zone.confiner.transform.position =
//                    EditorGUILayout.Vector3Field("위치", zone.confiner.transform.position);
//            }

//            // PolygonCollider2D Inspector 그대로 출력
//            if (zone.confiner != null)
//            {
//                if (!Application.isPlaying)
//                {
//                    SerializedObject so = new SerializedObject(zone.confiner);

//                    SerializedProperty pointsProp = so.FindProperty("m_Points");

//                    EditorGUILayout.PropertyField(pointsProp, true);

//                    so.ApplyModifiedProperties();
//                }
//            }

//            if (GUILayout.Button("삭제"))
//            {
//                DestroyImmediate(zone.gameObject);
//                break;
//            }

//            EditorGUILayout.EndVertical();
//        }
//    }

//    void AddZone(CameraManager manager)
//    {
//        GameObject zoneObj = new GameObject("CameraZone");
//        zoneObj.transform.parent = manager.transform;

//        var zone = zoneObj.AddComponent<CameraZone>();

//        // Polygon
//        var polygon = zoneObj.AddComponent<PolygonCollider2D>();
//        polygon.isTrigger = true;

//        var triggerScript = zoneObj.AddComponent<CameraZoneTrigger>();

//        // Camera
//        GameObject camObj = new GameObject("VirtualCamera");
//        camObj.transform.parent = zoneObj.transform;

//        var vcam = camObj.AddComponent<CinemachineVirtualCamera>();
//        vcam.transform.position = new Vector3(0, 0, -10);
//        vcam.m_Priority = 0;

//        var body = vcam.AddCinemachineComponent<CinemachineFramingTransposer>();

//        var confiner = vcam.gameObject.AddComponent<CinemachineConfiner2D>();
//        confiner.m_BoundingShape2D = polygon;
//        confiner.InvalidateCache();

//        // 연결
//        zone.vcam = vcam;
//        zone.confiner = polygon;

//        triggerScript.zone = zone;
//        triggerScript.manager = manager;

//        if (manager.currentCamera == null)
//        {
//            manager.currentCamera = vcam;
//            vcam.Follow = manager.player.transform;
//            vcam.m_Priority = 100;
//        }
//    }
//}