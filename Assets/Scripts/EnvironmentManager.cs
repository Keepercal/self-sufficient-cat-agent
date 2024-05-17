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

        // Get the size of the tile grid in tile units
        Vector3Int gridSize = tilemap.cellBounds.size;

        // Convert tile corrdinates to world coordinates
        Vector3 minWorldPos = tilemap.CellToWorld(tilemap.cellBounds.min) + new Vector3(borderSize, borderSize, 0);
        Vector3 maxWorldPos = tilemap.CellToWorld(tilemap.cellBounds.max) - new Vector3(borderSize, borderSize, 0);

        foreach(GameObject prefab in prefabsToSpawn)
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

            spawnPositions.Add(spawnPosition);

            GameObject newObject = Instantiate(prefab, spawnPosition, Quaternion.identity, objectParent);

            //Debug.Log("Spawned object at: " + spawnPosition);
            
        }
        
    }

    public Vector3 RepositionObjects()
    {
        List<Vector3> spawnPositions = new List<Vector3>();

        if (staticEnvironment == false)
        {
            if (objectParent.childCount > 0)
            {
                foreach (Transform child in objectParent)
                {
                    bool validSpawn = false;
                    Vector3 spawnPosition = Vector3.zero;
                
                    while(!validSpawn)
                    {
                        // Get the size of the tile grid in tile units
                        Vector3Int gridSize = tilemap.cellBounds.size;

                        // Convert tile corrdinates to world coordinates
                        Vector3 minWorldPos = tilemap.CellToWorld(tilemap.cellBounds.min) + new Vector3(borderSize, borderSize, 0);
                        Vector3 maxWorldPos = tilemap.CellToWorld(tilemap.cellBounds.max) - new Vector3(borderSize, borderSize, 0);

                        float randomX = Mathf.Round(Random.Range(minWorldPos.x, maxWorldPos.x));
                        float randomY = Mathf.Round(Random.Range(minWorldPos.y, maxWorldPos.y));

                        spawnPosition = new Vector3(randomX, randomY, -0.5f);

                        validSpawn = IsPositionValid(spawnPosition, minDistanceBetweenObjects, spawnPositions); 
                    }

                    spawnPositions.Add(spawnPosition);
                    child.position = spawnPosition;
                }
            }
        }
        
        // Return a default value if there are no children
        return Vector3.zero;
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
