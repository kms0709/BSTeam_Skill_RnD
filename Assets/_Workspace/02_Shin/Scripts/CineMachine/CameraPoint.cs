using UnityEditor;
using UnityEngine;

public enum CameraPointType
{
    Hold,
    Boss,
    Event,
    MakeObeject
}

public class CameraPoint : MonoBehaviour
{
    [Header("카메라 종류")]
    public CameraPointType cameraType = CameraPointType.Hold;

    [Header("이 카메라가 활성화될 때 우선순위")]
    public int priority = 20;

    [Header("카메라 Follow 위치")] // 필요 생각
    [SerializeField] private Transform FollowTarget;
    public Transform followTarget => FollowTarget;

    [Header("카메라 크기")] // 필요 생각
    [SerializeField] private float CameraSize;
    public float cameraSize => CameraSize;

    [Header("카메라 흔들림")] // 필요 생각
    [SerializeField] private float CameraNoise;
    public float cameraNoise => CameraNoise;

    //[Header("카메라가 플레이어를 앞질러서 움직이는 속도")]
    //[SerializeField] private float LookaheadTime;
    //public float lookahead => LookaheadTime;

    //[Header("카메라가 앞질러 가는 부드러움")]
    //[SerializeField] private float LookaheadSmooth;
    //public float lookaheadSmooth => LookaheadSmooth;

    //[Header("플레이어가 x축으로 먼저가고 카메라가 따라오는 속도")]
    //[SerializeField] private float X_Damping;
    //public float x_Damping => X_Damping;

    //[Header("플레이어가 y축으로 먼저가고 카메라가 따라오는 속도")]
    //[SerializeField] private float Y_Damping;
    //public float y_Damping => Y_Damping;


    [Header("트리거에 들어왔을 때만 활성화")]
    public bool useTrigger = true;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.3f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(5, 3, 0.1f));
    }
#endif
}