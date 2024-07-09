using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[CustomEditor(typeof(Tilemap))]
public class TilemapBoundsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Recalculate Bounds"))
        {
            RecalculateBounds();
        }
    }

    private void RecalculateBounds()
    {
        Tilemap tilemap = (Tilemap)target;
        BoundsInt newBounds = CalculateBounds(tilemap);
        tilemap.CompressBounds();
        Debug.Log("New Tilemap Bounds: " + newBounds);
    }

    private BoundsInt CalculateBounds(Tilemap tilemap)
    {
        BoundsInt bounds = new BoundsInt();
        bool boundsInitialized = false;

        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                if (!boundsInitialized)
                {
                    bounds = new BoundsInt(pos, Vector3Int.one);
                    boundsInitialized = true;
                }
                else
                {
                    // Expand the bounds to include the new position
                    bounds.xMin = Mathf.Min(bounds.xMin, pos.x);
                    bounds.yMin = Mathf.Min(bounds.yMin, pos.y);
                    bounds.zMin = Mathf.Min(bounds.zMin, pos.z);
                    bounds.xMax = Mathf.Max(bounds.xMax, pos.x + 1);
                    bounds.yMax = Mathf.Max(bounds.yMax, pos.y + 1);
                    bounds.zMax = Mathf.Max(bounds.zMax, pos.z + 1);
                }
            }
        }

        return bounds;
    }
}
