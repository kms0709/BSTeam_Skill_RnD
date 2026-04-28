using UnityEngine;

public class CameraUnFreezeZoneTrigger : MonoBehaviour, ICameraInteractable
{
    public static System.Action OnFreezeCameraTriggerEnter;

    //카메라 고정시 벽 오브젝트
    [field: SerializeField] public GameObject cameraBoundary { get; set; }

    public void SetCamera()
    {
        OnFreezeCameraTriggerEnter?.Invoke();

        if (cameraBoundary != null)
        {
            cameraBoundary.SetActive(false);
            Debug.Log("벽 비활성화 완료");
        }
    }
}