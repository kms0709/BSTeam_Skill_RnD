using UnityEngine;

public class CameraFollowState : ICameraState
{
    CameraController cameraController;
    private Vector3 targetPos;

    public void Enter(CameraController cameraController)
    {
        this.cameraController = cameraController;
    }

    public void Update()
    {
        if (cameraController.target == null) return;
        targetPos = cameraController.target.transform.position;
        targetPos.z = -10;

        cameraController.transform.position = targetPos; //Vector3.Lerp(cameraController.transform.position, targetPos, 1);
    }

    public void Exit()
    {
        Debug.Log("follow Out");
    }
}
