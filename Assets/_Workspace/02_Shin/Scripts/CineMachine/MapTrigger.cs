using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    [SerializeField] private GameObject verCamera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 맵 플레이 중 일때는 콜라이더의 trigger를 false로 두고 클리어하고 다음맵으로 넘어 갈때까지 true로 두고
        // 다음 맵에 입장하면 다시 false로
        if(collision.CompareTag("Player")) verCamera.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) verCamera.SetActive(false);
    }
}
