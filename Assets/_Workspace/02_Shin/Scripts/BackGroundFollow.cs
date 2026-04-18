using UnityEngine;
using UnityEngine.UI;

public class BackGroundFollow : MonoBehaviour
{
    [SerializeField] private RawImage img;
    [SerializeField] private float x;

    void Update()
    {
        img.uvRect = new Rect(img.uvRect.position + new Vector2(x, 0) * Time.deltaTime, img.uvRect.size);
    }
}