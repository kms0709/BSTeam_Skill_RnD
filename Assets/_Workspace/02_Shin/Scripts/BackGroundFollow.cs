using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


[System.Serializable]
public class BackGroundData
{
    public RawImage image;
    public float xSpeed;
}

public class BackGroundFollow : MonoBehaviour
{
    [SerializeField] private List<BackGroundData> img;
    [SerializeField] private Rigidbody2D player;

    void Update()
    {
        //img.uvRect = new Rect(img.uvRect.position + new Vector2(x, 0) * Time.deltaTime, img.uvRect.size);

        
        foreach (BackGroundData bgd in img)
        {
            Vector2 add = player.linearVelocity * bgd.xSpeed * Time.deltaTime;
            bgd.image.uvRect = new Rect(bgd.image.uvRect.position + add, bgd.image.uvRect.size);
        }
    }
}