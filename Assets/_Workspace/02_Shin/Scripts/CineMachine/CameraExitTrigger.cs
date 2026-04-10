using UnityEngine;

public class CameraExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CameraManager.Instance.SetPlayerCamera();
    }
}
