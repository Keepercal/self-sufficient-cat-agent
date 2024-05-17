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
    public TileBase[] targetTiles; // Tiles to spawn objects on

    public void SpawnObjects()
    {
        List<Vector3> spawnPositions = new List<Vector3>();
        Vector3 minWorldPos, maxWorldPos;
        GetWorldBounds(out minWorldPos, out maxWorldPos);

        foreach(GameObject prefab in prefabsToSpawn)
        {
            Vector3 spawnPosition = FindValidSpawnPosition(minWorldPos, maxWorldPos, spawnPositions);
            spawnPositions.Add(spawnPosition);
            GameObject newObject = Instantiate(prefab, spawnPosition, Quaternion.identity, objectParent);            
        }
        
    }

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

    void GetWorldBounds(out Vector3 minWorldPos, out Vector3 maxWorldPos)
    {
        minWorldPos = tilemap.CellToWorld(tilemap.cellBounds.min) + new Vector3(borderSize, borderSize, 0);
        maxWorldPos = tilemap.CellToWorld(tilemap.cellBounds.max) - new Vector3(borderSize, borderSize, 0);
    }

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
}
