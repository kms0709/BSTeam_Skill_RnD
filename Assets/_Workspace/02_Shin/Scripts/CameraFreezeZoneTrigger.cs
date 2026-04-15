using UnityEngine;

public class CameraFreezeZoneTrigger : MonoBehaviour
{
    public static System.Action<float, Vector3> OnCameraTriggerEnter;

    [SerializeField] private float cameraZoneSize;
    [SerializeField] private Vector3 cameraZonePosition;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            OnCameraTriggerEnter?.Invoke(cameraZoneSize, cameraZonePosition);
        }
    }
}
