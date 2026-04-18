using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionWreck_Detecter : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Test Player")
        {
            ExplosionWreck parent = gameObject.GetComponentInParent<ExplosionWreck>();
            parent.ActiveFuse();
        }
    }
}
