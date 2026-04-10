using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 5f;
    float w = 0;
    float h = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        w = Input.GetAxisRaw("Horizontal");
        h = Input.GetAxisRaw("Vertical");

        transform.position += new Vector3(w, h).normalized * Time.deltaTime * speed;
    }
}
