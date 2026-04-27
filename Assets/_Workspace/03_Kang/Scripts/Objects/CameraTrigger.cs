using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    /// <summary>
    /// 부모의 클래스를 인터페이스로 가져옴.
    /// </summary>
    ICameraInteractable _cameraInteract;

    private void Awake()
    {
        _cameraInteract = GetComponentInParent<ICameraInteractable>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // 트리거 충돌 시, 부모 오브젝트의 스크립트 호출.
            _cameraInteract.SetCamera();
        }
    }
}