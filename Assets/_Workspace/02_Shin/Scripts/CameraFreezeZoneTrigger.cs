using UnityEngine;

public class CameraFreezeZoneTrigger : MonoBehaviour, ICameraInteractable
{
    public static System.Action<float, Vector3> OnCameraTriggerEnter;

    [field: SerializeField] public float cameraZoneSize { get; set; }
    [field: SerializeField] public Vector3 cameraZonePosition { get; set; }

    //카메라 고정시 벽 오브젝트
    [field: SerializeField] public GameObject cameraBoundary {  get; set; }
    
    public void SetCamera()
    {
        OnCameraTriggerEnter?.Invoke(cameraZoneSize, cameraZonePosition);

        if(cameraBoundary != null)
        {
            cameraBoundary.SetActive(true);
            Debug.Log("벽 활성화 완료");
        }
    }
}
