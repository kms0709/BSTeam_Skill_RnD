using UnityEngine;

public class CameraUnFreezeZoneTrigger : MonoBehaviour
{
    public static System.Action<float> OnFreezeCameraTriggerEnter;

    [SerializeField] private float cameraZoneSize;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnFreezeCameraTriggerEnter?.Invoke(cameraZoneSize);
        }
    }
}
