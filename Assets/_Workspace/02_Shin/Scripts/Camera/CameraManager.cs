using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera cam;
    [SerializeField] private float cameraSize = 5;
    [SerializeField] private Transform cameraTarget;

    private void Start()
    {
        cam = Camera.main;
        cam.transform.position = new Vector3(10, 10, -10);
        ChangennnCameraSize();
    }

    private void Update()
    {
        //transform.position = cameraTarget.transform.position;    
    }

    private void ChangennnCameraSize()
    {
        cam.orthographicSize = cameraSize;
    }


}
