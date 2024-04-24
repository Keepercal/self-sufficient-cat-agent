using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{

    //-----------------------------------------
    // GameObject Parents
    //-----------------------------------------
    public Transform obstacleParent;
    public Transform interactableParent;

    //-----------------------------------------
    // Training Area
    //-----------------------------------------
    public Collider2D trainingArea;

    //-----------------------------------------
    // GameObject lists
    //-----------------------------------------
    public GameObject[] obstaclePrefabArray; // Prefabs of obstacles
    public GameObject[] sourcesList; // Prefabs of interactables

    //-----------------------------------------
    // METHOD: Called when the script is loaded
    //-----------------------------------------
    void Start()
    {
        GameObject waterSource = GameObject.FindGameObjectWithTag("Water Source");
        GameObject foodSource = GameObject.FindGameObjectWithTag("Food Source");
        GameObject funSource = GameObject.FindGameObjectWithTag("Fun Source");

        sourcesList = new GameObject[] { waterSource, foodSource, funSource };
    }

    //-----------------------------------------
    // METHOD: Randomly spawns obstacles within the environment
    //-----------------------------------------
    public void SpawnObstacles() // Spawn a number of obstacles
    {
        DestoryObstacles(); // Destory all exisitng obstacles

        int numObstacles;
        numObstacles = Random.Range(5, 20); // The number of obstacles to spawn in the training area

        int randomIndex = Random.Range(0, obstaclePrefabArray.Length); // Randomly select an obstacle prefab

        // Spawn new obstacles
        for (int i = 0; i < numObstacles; i++)
        {
            // Set the obstacle's position to a random position within the training area
            Vector2 position = RandomisePosition(collider: obstaclePrefabArray[randomIndex].GetComponent<Collider2D>());
            Instantiate(obstaclePrefabArray[randomIndex], position, Quaternion.identity, obstacleParent); // Spawn the obstacle
        }
    }

    //-----------------------------------------
    // HELPER METHOD: Randomly positions the GameObject called on
    //-----------------------------------------
    public Vector2 RandomisePosition(Collider2D collider)
    {
        // Randomly generate a position within the training area
        Vector2 newPosition = new Vector2(Random.Range(trainingArea.bounds.min.x, trainingArea.bounds.max.x), Random.Range(trainingArea.bounds.min.y, trainingArea.bounds.max.y));

        float minDistance = 4.0f; // Minimum distance between GameObjects
        
        foreach(GameObject source in sourcesList)
        {
            // Calculate the distance between the new position and the source
            float distance = Vector2.Distance(newPosition, source.transform.position);

            // If the distance is less than the minimum distance, re-randomise the position
            if (distance < minDistance)
            {
                newPosition = new Vector2(Random.Range(trainingArea.bounds.min.x, trainingArea.bounds.max.x), Random.Range(trainingArea.bounds.min.y, trainingArea.bounds.max.y));
            }

            // If the GameObject is touching any other objects in the env, re-randomise the position
            if(collider.IsTouching(source.GetComponent<Collider2D>()))
            {
                // Randomise the position of the obstacle within the training area
                newPosition = new Vector2(Random.Range(trainingArea.bounds.min.x, trainingArea.bounds.max.x), Random.Range(trainingArea.bounds.min.y, trainingArea.bounds.max.y));
            }
        }

        return newPosition;
    }

    //-----------------------------------------
    // HELPER METHOD: Destorys all existing obstacles
    //-----------------------------------------
    void DestoryObstacles()
    {
        GameObject[] existingObstacles = GameObject.FindGameObjectsWithTag("Obstacle");

        // Destory previous obstacles
        foreach (GameObject obstacle in existingObstacles)
        {
            Destroy(obstacle);
        }
    }
}
