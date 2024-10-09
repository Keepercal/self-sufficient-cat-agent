using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnvironmentManager : MonoBehaviour
{
    public bool staticEnvironment = true; // If true, objects will be spawned once. If false, objects will be repositioned every time the function is called
    public float minDistanceBetweenObjects = 8f; // Minimum distance between objects
    public int borderSize = 1; // Border size around the tilemap where objects will not be spawned

    public Transform objectParent; // objectParent object to hold all the spawned objects
    public GameObject[] prefabsToSpawn; // Array of object prefabs to spawn (cat tower, bowl, food)

    public Tilemap tilemap; // Tilemap to spawn objects on
    private TileBase[] targetTiles; // Tiles to spawn objects on

    /// <summary>
    /// Spawns all prefabs in the prefabsToSpawn array onto the tilemap
    /// </summary>
    /// <returns>
    /// void
    /// </returns>
    ///<remarks>
    /// If staticEnvironment is true, objects will be spawned once in a set position. If false, objects will be repositioned every time the function is called
    ///</remarks>
    public void SpawnObjects()
    {
        List<Vector3> spawnPositions = new List<Vector3>();
        Vector3 minWorldPos, maxWorldPos;
        GetWorldBounds(out minWorldPos, out maxWorldPos);

        if (staticEnvironment == true)
        {
            GameObject newObject1 = Instantiate(prefabsToSpawn[0], new Vector3(-6, 3, -1), Quaternion.identity, objectParent);
            GameObject newObject2 = Instantiate(prefabsToSpawn[1], new Vector3(-6, -3, -1), Quaternion.identity, objectParent);
            GameObject newObject3 = Instantiate(prefabsToSpawn[2], new Vector3(8, 3, -1), Quaternion.identity, objectParent);
        }
        
        else{
            foreach(GameObject prefab in prefabsToSpawn)
            {
                Vector3 spawnPosition = FindValidSpawnPosition(minWorldPos, maxWorldPos, spawnPositions);
                spawnPositions.Add(spawnPosition);
                GameObject newObject = Instantiate(prefab, spawnPosition, Quaternion.identity, objectParent);            
            }    
        }
    }

    /// <summary>
    /// Repositions all objects in the objectParent object
    /// </summary>
    /// <returns>
    /// Vector3 : A Vector3 position of the first object in the objectParent object
    /// </returns>
    ///<remarks>
    /// If staticEnvironment is true, objects will be spawned once in a set position. If false, objects will be repositioned every time the function is called
    ///</remarks>
    public Vector3 RepositionObjects()
    {
        if (staticEnvironment == false && objectParent.childCount > 0)
        {
            List<Vector3> spawnPositions = new List<Vector3>();
            Vector3 minWorldPos, maxWorldPos;
            GetWorldBounds(out minWorldPos, out maxWorldPos);

            foreach (Transform child in objectParent)
            {
                Vector3 spawnPosition = FindValidSpawnPosition(minWorldPos, maxWorldPos, spawnPositions);
                spawnPositions.Add(spawnPosition);
                child.position = spawnPosition;
            }
            return spawnPositions[0];
        }
        
        // Return a default value if there are no children
        return Vector3.zero;
    }

    /// <summary>
    /// Gets the world bounds of the tilemap
    /// </summary>
    /// <param name="minWorldPos">The minimum world position of the tilemap</param>
    /// <param name="maxWorldPos">The maximum world position of the tilemap</param>
    /// <returns>
    /// void
    /// </returns>
    public void GetWorldBounds(out Vector3 minWorldPos, out Vector3 maxWorldPos)
    {
        minWorldPos = tilemap.CellToWorld(tilemap.cellBounds.min) + new Vector3(borderSize, borderSize, 0);
        maxWorldPos = tilemap.CellToWorld(tilemap.cellBounds.max) - new Vector3(borderSize, borderSize, 0);
    }

    /// <summary>
    /// Finds a valid spawn position for an object
    /// </summary>
    /// <param name="minWorldPos">The minimum world position of the tilemap</param>
    /// <param name="maxWorldPos">The maximum world position of the tilemap</param>
    /// <param name="spawnPositions">A list of all the spawn positions of the objects</param>
    /// <returns>
    /// Vector3 : A Vector3 position of the spawn position
    /// </returns>
    Vector3 FindValidSpawnPosition(Vector3 minWorldPos, Vector3 maxWorldPos, List<Vector3> spawnPositions)
    {
        bool validSpawn = false;
        Vector3 spawnPosition = Vector3.zero;

        while(!validSpawn)
        {
            float randomX = Mathf.Round(Random.Range(minWorldPos.x, maxWorldPos.x));
            float randomY = Mathf.Round(Random.Range(minWorldPos.y, maxWorldPos.y));

            spawnPosition = new Vector3(randomX, randomY, -0.5f);

            validSpawn = IsPositionValid(spawnPosition, minDistanceBetweenObjects, spawnPositions); 
        }

        return spawnPosition;
    }

    /// <summary>
    /// Checks if a position is valid to spawn an object
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <param name="minDistance">The minimum distance between objects</param>
    /// <param name="positions">A list of all the spawn positions of the objects</param>
    /// <returns>
    /// bool : True if the position is valid, false if it is not
    /// </returns>
    bool IsPositionValid(Vector3 position, float minDistance, List<Vector3> positions)
    {
        foreach(Vector3 existingPosition in positions)
        {
            if(Vector3.Distance(position, existingPosition) < minDistance)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if a position is outside the tilemap bounds
    /// </summary>
    /// <param name="position">The position to check</param>
    /// <returns>
    /// bool : True if the position is outside the tilemap bounds, false if it is not
    /// </returns>
    public bool IsOutsideTilemapBounds(Vector3 position)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(position);

        BoundsInt tilemapBounds = tilemap.cellBounds;

        if (cellPosition.x < tilemapBounds.min.x || cellPosition.x > tilemapBounds.max.x ||
        cellPosition.y < tilemapBounds.min.y || cellPosition.y > tilemapBounds.max.y)
        {
            return true;
        }

        if (!tilemap.HasTile(cellPosition))
        {
            return true;
        }

        return false;
    }
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 minWorldPos, maxWorldPos;
        GetWorldBounds(out minWorldPos, out maxWorldPos);
        
        Gizmos.DrawLine(minWorldPos, new Vector3(minWorldPos.x, maxWorldPos.y, 0));
        Gizmos.DrawLine(minWorldPos, new Vector3(maxWorldPos.x, minWorldPos.y, 0));
        Gizmos.DrawLine(maxWorldPos, new Vector3(minWorldPos.x, maxWorldPos.y, 0));
        Gizmos.DrawLine(maxWorldPos, new Vector3(maxWorldPos.x, minWorldPos.y, 0));
    }
}
