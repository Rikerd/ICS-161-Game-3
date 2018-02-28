using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestroyTile : MonoBehaviour {
    public GridLayout grid;
    public Tilemap tilemap;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            Vector3Int position = grid.WorldToCell(collision.transform.position);

            foreach (var p in new BoundsInt(-1, -1, 0, 3, 3, 1).allPositionsWithin)
            {
                tilemap.SetTile(position + p, null);
            }
        }
    }
}
