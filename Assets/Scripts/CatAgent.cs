// Run command for training

// Note: activate the virtual enviromenmt first using "conda activate mlagents"
// Then navigate to the /Users/Callum/DevTools/Git Clones/ml-agents directory and run the command below

// mlagents-learn config/cat_config_ppo.yaml --run-id=DummyRun --force
// tensorboard --logdir results --port 6006

// CAT VERSION 1.1

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

//-----------------------------------------
// CLASS: CatAgent

public class CatAgent : Agent
{
    //-----------------------------------------
    // Simulation parameters
    
    // Movement
    private float moveSpeed = 150f;

    // ???
    private const float maxNeed = 500f; // 100% 
    private const float minNeed = 0f; // 0%
    
    private const float needRegenRate = 10f; // Regeneration rate
    private const float needDegenRate = 1f; // Degeneration rate

    private const float threeQuatersFull = maxNeed / 1.5f; // 75% of the agent's max need
    private const float halfFull = maxNeed / 2f; // 50% of the agent's max need
    private const float quaterFull = maxNeed / 4f; // 25% of the agent's max need

    // Flags
    [Header("Environment")]
    private bool isMoving = false; // Flag to check if the agent is moving
    public bool staticEnvironment = true; // Flag to check if the environment is static

    // Counters
    private int callCount = 0;
    private int stepCount = 0;

    //-----------------------------------------
    // Grid & environment logic
    
    private Vector2 cellSize = new Vector2(1, 1); // The size of each grid cell in the environment
    private Vector3 targetedPosition; // The target grid position for the agent to move to

    public EnvironmentManager environmentManager; // Reference to the environment manager
    public GameObject ScreenFlash; // Screen flash object

    //-----------------------------------------
    // Graphical components

    Animator animator; // Agent animator

    [Header("UI")]
    public StatusBar healthBar; // Agent health bar
    public StatusBar thirstBar; // Agent thirst bar
    public StatusBar hungerBar; // Agent hunger bar
    public StatusBar happinessBar; // Agent fun bar

    //-----------------------------------------
    // Agent needs
    
    [Header("Needs")]
    public float health;
    public float happiness;
    public float hunger;
    public float thirst;

    //-----------------------------------------
    // GameObjects
    
    GameObject happinessObject;
    GameObject thirstObject; 
    GameObject hungerObject; 

    //-----------------------------------------
    // Transforms
    
    Transform happinessTransform; 
    Transform thirstTransform; 
    Transform hungerTransform;

    //-----------------------------------------
    // Positions

    // Object positions
    Vector2 thirstPosition;
    Vector2 hungerPosition;
    Vector2 happinessPosition;

    // World bounds
    private Vector3 minWorldBound;
    private Vector3 maxWorldBound;

    //-----------------------------------------
    // METHOD: Called when the simulation is initialised
    
    public override void Initialize()
    {
        //-----------------------------------------
        // Environment initialisation

        environmentManager.SpawnObjects();

        var cellBounds = environmentManager.tilemap.cellBounds;
        minWorldBound = environmentManager.tilemap.CellToWorld(cellBounds.min) + new Vector3(1, 1, 0);
        maxWorldBound = environmentManager.tilemap.CellToWorld(cellBounds.max) - new Vector3(1, 1, 0);

        //-----------------------------------------
        // Agent component references

        animator = GetComponent<Animator>();
        ScreenFlash.SetActive(false);

        //-----------------------------------------
        // Object component references

        // GameObjects
        happinessObject = GameObject.FindWithTag("Fun Source"); 
        thirstObject = GameObject.FindWithTag("Water Source");
        hungerObject = GameObject.FindWithTag("Food Source");

        // Transforms
        thirstTransform = thirstObject.GetComponent<Transform>();
        hungerTransform = hungerObject.GetComponent<Transform>();
        happinessTransform = happinessObject.GetComponent<Transform>();
    }

    //-----------------------------------------
    // METHOD: Set up an episode
    
    public override void OnEpisodeBegin()
    {
        //-----------------------------------------
        // Agent setup
        callCount = 0;
        stepCount = 0;
        
        transform.position = new Vector3(0, 0, -1); //Reset the agent's position to the centre of the training area
        
        ResetNeeds(); // Reset the agent's needs

        // Determine the agent's lowest need
        float lowestNeed;

        if(thirst < hunger && thirst < happiness)
        {
            lowestNeed = thirst;

            // Set the target position to the respectiove object
            targetedPosition = thirstTransform.position;
        }

        if(hunger < thirst && hunger < happiness)
        {
            lowestNeed = hunger;
            targetedPosition = hungerTransform.position;
        }

        if(happiness < thirst && happiness < hunger)
        {
            lowestNeed = happiness;
            targetedPosition = happinessTransform.position;
        }

        //-----------------------------------------
        // Environment setup
        
        if (staticEnvironment == false)
        {
            environmentManager.RepositionObjects(); // Reposition the interactable objects
        }

        thirstPosition = thirstTransform.position;
        hungerPosition = hungerTransform.position;
        happinessPosition = happinessTransform.position;
    }

    //-----------------------------------------
    // METHOD: Collect the agent's observations, called every step
    //-----------------------------------------
    public override void CollectObservations(VectorSensor sensor)
    {   
        //-----------------------------------------
        // Observations about the agent's position
        sensor.AddObservation(this.transform.position); // Position

        sensor.AddObservation(minWorldBound);
        sensor.AddObservation(maxWorldBound);

        sensor.AddObservation(transform.position.x - minWorldBound.x);
        sensor.AddObservation(transform.position.y - minWorldBound.y);
        sensor.AddObservation(maxWorldBound.x - transform.position.x);
        sensor.AddObservation(maxWorldBound.y - transform.position.y);

        //-----------------------------------------
        // Observations about the agent's needs, normalised to range of -1,1
        sensor.AddObservation(health / maxNeed);
        sensor.AddObservation(thirst / maxNeed); 
        sensor.AddObservation(hunger / maxNeed);
        sensor.AddObservation(happiness / maxNeed); 

        //-----------------------------------------
        // Observations about the environment
        sensor.AddObservation(thirstTransform.position); // Object positions
        sensor.AddObservation(hungerTransform.position);
        sensor.AddObservation(happinessTransform.position); 

        sensor.AddObservation(Vector2.Distance(transform.position, thirstTransform.position)); // Agent distance
        sensor.AddObservation(Vector2.Distance(transform.position, hungerTransform.position)); // to objects
        sensor.AddObservation(Vector2.Distance(transform.position, happinessTransform.position));
    }

    //-----------------------------------------
    // METHOD: Called when the agent takes an action, called every step
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {   
        stepCount++;

        Vector2 currentPosition = transform.position; // The agent's current position

        // Clamp the agent's needs between 0 and the default value
        health = Mathf.Clamp(health, 0, maxNeed);
        thirst = Mathf.Clamp(thirst, 0, maxNeed);
        hunger = Mathf.Clamp(hunger, 0, maxNeed);
        happiness = Mathf.Clamp(happiness, 0, maxNeed);

        // Perform checks on agent's health and needs
        HealthCheck();
        NeedsCheck();

        // Decrement the agent's needs over time
        // NOTE: Minimum step count per episode is 750 steps
        DegenerateNeed(ref thirst, thirstBar);
        DegenerateNeed(ref hunger, hungerBar);
        DegenerateNeed(ref happiness, happinessBar);

        AddReward(-0.01f); // Small negative reward to encourage efficiency

        // Check if the agent is outside of the tilemap bounds
        if (environmentManager.IsOutsideTilemapBounds(transform.position))
        {
            AddReward(-1f);
            EndEpisode();

            Debug.Log("Agent outside of bounds");
            StartCoroutine(ShowAndHide(ScreenFlash, 0.1f));
            return;
        }

        // Get the action from the action buffer
        int movement = actionBuffers.DiscreteActions[0];

        // Agent movement
        if(!isMoving)
        {
            switch(movement)
            {
                case 0: // No movement
                    break;
                case 1: // Move up
                    Move(Vector2.up);
                    break;
                case 2: // Move down
                    Move(Vector2.down);
                    break;
                case 3: // Move left
                    Move(Vector2.left);
                    break;
                case 4: // Move right
                    Move(Vector2.right);
                    break;
                case 5: // Move up-left
                    Move(new Vector2(-1, 1));
                    break;
                case 6: // Move up-right
                    Move(new Vector2(1, 1));
                    break;
                case 7: // Move down-left
                    Move(new Vector2(-1, -1));
                    break;
                case 8: // Move down-right
                    Move(new Vector2(1, -1));
                    break;
                default:
                    break;
            }
        }

        // Check if the agent is using an object
        if(IsAgentUsingObject())
        {
            if (callCount < 1)
            {
                AddReward(1f);
                callCount++;
            }
        }

        if (stepCount >= 10000)
        {
            Debug.Log("Episode complete! Cumulative reward: " + GetCumulativeReward());
            AddReward(1f);
            EndEpisode();
        }
    }

    //-----------------------------------------
    // HELPER FUNCTION: Agent interaction with objects
    //-----------------------------------------
    private bool IsAgentNearObject(Vector2 agentPosition, Vector2 objectPosition)
    {
        float distance = Vector2.Distance(agentPosition, objectPosition);
        return distance <= Mathf.Sqrt(2) * cellSize.x;
    }

    // Check if the agent is using an object
    private bool IsAgentUsingObject()
    {
        Vector2 currentObjectUsed = new Vector2(0, 0);
        Vector2 previousObjectUsed = new Vector2(0, 0);
        Vector2 agentPosition = transform.position;

        if (IsAgentNearObject(agentPosition, thirstPosition))
        {
            RegenerateNeed(ref thirst, thirstBar);
            callCount++;
            return true;
        }
        else if (IsAgentNearObject(agentPosition, hungerPosition))
        {
            RegenerateNeed(ref hunger, hungerBar);
            callCount++;
            return true;
        }
        else if (IsAgentNearObject(agentPosition, happinessPosition))
        {
            RegenerateNeed(ref happiness, happinessBar);
            callCount++;
            return true;
        }

        else
        {
            return false;
        }
    }

    //-----------------------------------------
    // HELPER FUNCTION: Increase the agent's health if all needs are above the threshold
    //-----------------------------------------
    private void HealthCheck()
    {
        // If all needs are above the threshold, increase the agent's health
        if (thirst >= threeQuatersFull && hunger >= threeQuatersFull && happiness >= threeQuatersFull)
        {
            RegenerateNeed(ref health, healthBar);
            AddReward(0.01f);
        }


        // If any need is below the threshold, decrement the agent's health
        if (thirst <= 0 || hunger <= 0 || happiness <= 0)
        {
            AddReward(-0.01f);

            if (health < quaterFull)
            {
                AddReward(-0.01f);
            }

            if (health > 0) // If the agent's health is above 0, decrement it
            {
                DegenerateNeed(ref health, healthBar);
                AddReward(-0.01f);
            }
            
            // If the agent's health reaches 0, end the episode
            else
            {   
                AddReward(-1f);
                EndEpisode();

                Debug.Log("Agent died");
                StartCoroutine(ShowAndHide(ScreenFlash, 0.1f));
                return;
            }
        }
    }

    private void NeedsCheck()
    {   
        if (thirst <= quaterFull)
        {
            AddReward(-0.01f);
        }

        if (hunger <= quaterFull)
        {
            AddReward(-0.01f);
        }

        if (happiness <= quaterFull)
        {
            AddReward(-0.01f);
        }

        if (thirst <= 0)
        {
            AddReward(-0.01f);
        }

        if (hunger <= 0)
        {
            AddReward(-0.01f);
        }

        if (happiness <= 0)
        {
            AddReward(-0.01f);
        }
    }

    //-----------------------------------------
    // HELPER FUNCTION: Randomise the agent's needs
    //-----------------------------------------
    private void ResetNeeds()
    {
        health = maxNeed;
        thirst = Random.Range(maxNeed / 1.5f, maxNeed);
        hunger = Random.Range(maxNeed / 1.5f, maxNeed);
        happiness = Random.Range(maxNeed / 1.5f, maxNeed);
    }

    //-----------------------------------------
    // HELPER FUNCTION: Regenerate a given need
    //-----------------------------------------
    public void RegenerateNeed(ref float need, StatusBar statusBar)
    {
        need += needRegenRate;
        statusBar.SetValue(need);
    }

    //-----------------------------------------
    // HELPER FUNCTION: Degenerate a given need
    //-----------------------------------------
    public void DegenerateNeed(ref float need, StatusBar statusBar)
    {
        need--;
        statusBar.SetValue(need);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if(!isMoving)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                Move(Vector2.up);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                Move(Vector2.down);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                Move(Vector2.left);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                Move(Vector2.right);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                EndEpisode();
                StartCoroutine(ShowAndHide(ScreenFlash, 0.1f));
            }
        }
    }

    void Move(Vector2 direction)
    {
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(direction.x * cellSize.x, direction.y * cellSize.y, 0);

        if (IsWithinBounds(end))
        {
            targetedPosition = end;
            isMoving = true;
        }
    }

    bool IsWithinBounds(Vector3 position)
    {
        return true;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetedPosition, moveSpeed * Time.deltaTime);

            if ((Vector3)transform.position == targetedPosition)
            {
                isMoving = false;
            }
        }
    }

    IEnumerator ShowAndHide(GameObject ScreenFlash, float delay)
    {
        ScreenFlash.SetActive(true);
        yield return new WaitForSeconds(delay);
        ScreenFlash.SetActive(false);
    }

}
