using UnityEngine;

public class CameraUnFreezeZoneTrigger : MonoBehaviour, ICameraInteractable
{
    public static System.Action OnFreezeCameraTriggerEnter;

    public void SetCamera()
    {
        OnFreezeCameraTriggerEnter?.Invoke();
    }
}