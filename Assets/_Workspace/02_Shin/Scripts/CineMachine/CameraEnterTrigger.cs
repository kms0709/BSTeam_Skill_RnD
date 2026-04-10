using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CameraEnterTrigger : MonoBehaviour
{
    [SerializeField] private CameraPoint targetPoint;

    //private void Reset()
    //{
    //    Collider2D col = GetComponent<Collider2D>();
    //    col.isTrigger = true;
    //}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CameraManager.Instance.ActivatePoint(targetPoint);
    }
}