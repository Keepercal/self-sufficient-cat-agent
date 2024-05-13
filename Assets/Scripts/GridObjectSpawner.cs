using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridObjectSpawner : MonoBehaviour
{
    public GameObject[] prefabsToSpawn;
    public Transform parent; // Parent object to hold all the spawned objects
    public Tilemap tilemap;
    public TileBase[] targetTiles;

    public float minDistanceBetweenObjects = 10f; // Minimum distance between objects
    public int borderSize = 1; // Border size around the tilemap where objects will not be spawned

    // Start is called before the first frame update
    public void SpawnObjects()
    {
        List<Vector3> spawnPositions = new List<Vector3>();

        foreach(GameObject prefab in prefabsToSpawn)
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

                float randomX = Random.Range(minWorldPos.x, maxWorldPos.x);
                float randomY = Random.Range(minWorldPos.y, maxWorldPos.y);

                spawnPosition = new Vector3(randomX, randomY, 0);

                validSpawn = IsPositionValid(spawnPosition, minDistanceBetweenObjects, spawnPositions); 
            }

            spawnPositions.Add(spawnPosition);

            GameObject newObject = Instantiate(prefab, spawnPosition, Quaternion.identity, parent);
            
        }
        
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

    public void RepositionObjects()
    {
        foreach(GameObject prefab in prefabsToSpawn)
        {
            // Get the size of the tile grid in tile units
            Vector3Int gridSize = tilemap.cellBounds.size;

            // Convert tile corrdinates to world coordinates
            Vector3 minWorldPos = tilemap.CellToWorld(tilemap.cellBounds.min);
            Vector3 maxWorldPos = tilemap.CellToWorld(tilemap.cellBounds.max);

            float randomX = Random.Range(minWorldPos.x, maxWorldPos.x);
            float randomY = Random.Range(minWorldPos.y, maxWorldPos.y);

            prefab.transform.position = new Vector3(randomX, randomY, 0);
        }
    }
}
