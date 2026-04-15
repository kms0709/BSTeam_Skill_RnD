using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CameraZoneTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    public int activePriority = 20;
    public int idlePriority = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("asdf");
        if (collision.CompareTag("Player"))
        {
            Debug.Log("adffg");
            vCam.Priority = activePriority; // 구역 진입 시 우선순위 상승
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            vCam.Priority = idlePriority; // 구역 이탈 시 우선순위 하락
        }
    }
}