using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class _TileChecker : MonoBehaviour
{
    Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D hit in collision.contacts)
        {
            Vector3Int cellPos = tilemap.WorldToCell(hit.point);
            Debug.Log(string.Format("Cell Position : {0}", cellPos));

            TileBase tile = tilemap.GetTile(cellPos + Vector3Int.down);

            if (tile != null)
            {
                Debug.Log("Hit Tile: " + tile.name + "at " + cellPos);
            }
            else
            {
                Debug.Log("What?");
            }
        }
    }
}
