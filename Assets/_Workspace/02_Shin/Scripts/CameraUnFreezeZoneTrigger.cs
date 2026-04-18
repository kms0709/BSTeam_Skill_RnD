using UnityEngine;

public class CameraUnFreezeZoneTrigger : MonoBehaviour
{
    public static System.Action OnFreezeCameraTriggerEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnFreezeCameraTriggerEnter?.Invoke();
        }
    }
}
