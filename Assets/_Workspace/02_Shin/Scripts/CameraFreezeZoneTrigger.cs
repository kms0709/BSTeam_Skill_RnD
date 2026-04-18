using UnityEngine;

public class CameraFreezeZoneTrigger : MonoBehaviour
{
    public static System.Action<float, Vector3> OnCameraTriggerEnter;

    [field : SerializeField] public float cameraZoneSize { get; set; }
    [field : SerializeField] public Vector3 cameraZonePosition { get; set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            OnCameraTriggerEnter?.Invoke(cameraZoneSize, cameraZonePosition);
        }
    }
}
