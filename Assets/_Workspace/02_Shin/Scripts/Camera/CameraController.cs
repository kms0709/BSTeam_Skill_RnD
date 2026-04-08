using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //[SerializeField] private Transform Target;
    //public Transform target => Target;


    [field : SerializeField]
    public Transform target { get; private set; }

    private CameraStateMachine cameraState;

    private void Start()
    {
        cameraState = new CameraStateMachine(this);
        cameraState.ChangeCameraState(new CameraFollowState());
    }

    private void Update()
    {
        if (cameraState == null) return;

        cameraState.Update();
    }

    //todo : 이벤트 구독 및 해제 코드
    //private void OnEnable()
    //{
           
    //}

    //private void OnDisable()
    //{
        
    //}

    private void EnterHold() => cameraState.ChangeCameraState(new CameraHoldState());

    private void ExitHold() => cameraState.ChangeCameraState(new CameraFollowState());

}
