using UnityEngine;

public class CameraFreezeZoneTrigger : MonoBehaviour, ICameraInteractable
{
    public static System.Action<float, Vector3> OnCameraTriggerEnter;

    [field: SerializeField] public float cameraZoneSize { get; set; }
    [field: SerializeField] public Vector3 cameraZonePosition { get; set; }

    public void SetCamera()
    {
        OnCameraTriggerEnter?.Invoke(cameraZoneSize, cameraZonePosition);
    }
}
