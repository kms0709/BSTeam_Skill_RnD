using UnityEngine;

public class CameraHoldState : ICameraState
{
    CameraController cameraController;
    
    public void Enter(CameraController cameraController)
    {
        this.cameraController = cameraController;
    }

    public void Update()
    {
        // 움직임을 설정한 위치에 고정하고 사이즈를 변경한다.

    }

    public void Exit()
    {

        Debug.Log("hold out");
    }
}
