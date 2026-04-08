public class CameraStateMachine
{
    private ICameraState currentState;
    private CameraController cameraController;

    public CameraStateMachine(CameraController cameraController)
    {
        this.cameraController = cameraController;
    }

    public void ChangeCameraState(ICameraState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(cameraController);
    }

    public void Update()
    {
        currentState?.Update();
    }
}
