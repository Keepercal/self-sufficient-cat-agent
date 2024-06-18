// Run command for training
// Note: activate the virtual enviromenmt first using "conda activate mlagents"

// Then navigate to the /Users/Callum/DevTools/Git Clones/ml-agents directory and run the command below

// mlagents-learn config/dsp_config.yaml --run-id=RunTest --force

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

//-----------------------------------------
// CLASS: CatAgent
//-----------------------------------------
public class CatAgent : Agent
{
    //-----------------------------------------
    // Environment class members
    //-----------------------------------------

    private Vector2 gridSize = new Vector2(1, 1); // The size of each grid cell in the environment
    private Vector3 targetedPosition; // The target position for the agent to move to

    // Reference to the environment manager
    public EnvironmentManager environmentManager;

    // Flags
    private bool isMoving = false; // Flag to check if the agent is moving

    //-----------------------------------------
    // Agent component class members
    //-----------------------------------------

    Rigidbody2D rBody; // Agent rigidbody
    Collider2D agentCollider; // Agent collider
    Animator animator; // Agent animator

    public StatusBar healthBar; // Agent health bar
    public StatusBar thirstBar; // Agent thirst bar
    public StatusBar hungerBar; // Agent hunger bar
    public StatusBar funBar; // Agent fun bar

    //-----------------------------------------
    // Agent needs class members
    //-----------------------------------------
    // Default need value
    private const int maxNeed = 500;
    private const int minNeed = 0;
    
    private const int needDecayRate = 1;
    private const int needRegenRate = 5; // The rate at which the agent's needs regenerate
    private const int needThreshold = maxNeed / 2;


    public int agentHealth;
    public int agentFun;
    public int agentHunger;
    public int agentThirst;

    //-----------------------------------------
    // Object game object class members
    //-----------------------------------------
    GameObject funSource;
    GameObject waterSource; 
    GameObject foodSource; 

    //-----------------------------------------
    // Object transform class members
    //-----------------------------------------
    Transform funTransform; 
    Transform waterTransform; 
    Transform foodTransform;

    //-----------------------------------------
    // Object position class members
    //-----------------------------------------
    private Vector2 waterPosition;
    private Vector2 foodPosition;
    private Vector2 funPosition;

    //-----------------------------------------
    // Simulation parameters
    //-----------------------------------------
    private float moveSpeed = 150f; // The force multiplier for the agent's movement

    private Vector2 targetPosition; // The target position for the agent to move to
    private Vector2 previousAgentPosition; // The agent's previous position

    private Vector3 minWorldBound;
    private Vector3 maxWorldBound;

    //-----------------------------------------
    // METHOD: Called when the simulation is initialised
    //-----------------------------------------
    public override void Initialize()
    {
        //-----------------------------------------
        // Environment initialisation
        //-----------------------------------------
        environmentManager.SpawnObjects();

        //-----------------------------------------
        // References to the agent's components
        //-----------------------------------------
        rBody = GetComponent<Rigidbody2D>(); 
        animator = GetComponent<Animator>();
        agentCollider = GetComponent<Collider2D>();

        //-----------------------------------------
        // References to the environment objects
        //-----------------------------------------
        funSource = GameObject.FindWithTag("Fun Source"); 
        waterSource = GameObject.FindWithTag("Water Source");
        foodSource = GameObject.FindWithTag("Food Source");

        //-----------------------------------------
        // Environment transforms
        //-----------------------------------------
        waterTransform = waterSource.GetComponent<Transform>();
        foodTransform = foodSource.GetComponent<Transform>();
        funTransform = funSource.GetComponent<Transform>();

        var cellBounds = environmentManager.tilemap.cellBounds;
        minWorldBound = environmentManager.tilemap.CellToWorld(cellBounds.min) + new Vector3(1, 1, 0);
        maxWorldBound = environmentManager.tilemap.CellToWorld(cellBounds.max) - new Vector3(1, 1, 0);
    }

    //-----------------------------------------
    // METHOD: Called when the agent is reset
    //----------------------------------------- 
    public override void OnEpisodeBegin()
    {
        //-----------------------------------------
        // Agent logic
        //-----------------------------------------
        transform.position = new Vector3(0, 0, -1); //Reset the agent's position to the centre of the training area
        
        ResetNeeds(); // Reset the agent's needs
        
        //-----------------------------------------
        // Environment logic
        //-----------------------------------------
        environmentManager.RepositionObjects(); // Reposition the interactable objects

        waterPosition = waterTransform.position;
        foodPosition = foodTransform.position;
        funPosition = funTransform.position;
    }

    //-----------------------------------------
    // METHOD: Collect the agent's observations, called every step
    //-----------------------------------------
    public override void CollectObservations(VectorSensor sensor)
    {   
        //-----------------------------------------
        // Observations about the agent's state
        //-----------------------------------------
        sensor.AddObservation(this.transform.position); // Position

        sensor.AddObservation(minWorldBound.x);
        sensor.AddObservation(minWorldBound.y);
        sensor.AddObservation(maxWorldBound.x);
        sensor.AddObservation(maxWorldBound.y);

        //-----------------------------------------
        // Observations about the agent's needs, normalised to range of -1,1
        //-----------------------------------------
        sensor.AddObservation(agentHealth / maxNeed);
        sensor.AddObservation(agentThirst / maxNeed); 
        sensor.AddObservation(agentHunger / maxNeed);
        sensor.AddObservation(agentFun / maxNeed); 

        //-----------------------------------------
        // Observations about the environment
        //-----------------------------------------
        sensor.AddObservation(waterTransform.position); 
        sensor.AddObservation(foodTransform.position);
        sensor.AddObservation(funTransform.position); 

        sensor.AddObservation(Vector2.Distance(transform.position, waterTransform.localPosition));
        sensor.AddObservation(Vector2.Distance(transform.position, foodTransform.localPosition));
        sensor.AddObservation(Vector2.Distance(transform.position, funTransform.localPosition));
    }

    //-----------------------------------------
    // METHOD: Called when the agent takes an action, called every step
    //-----------------------------------------
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {   
        rBody.velocity = Vector2.zero; // Reset the agent's velocity

        if (environmentManager.IsOutsideTilemapBounds(transform.position))
        {
            AddReward(-1f);
            EndEpisode();
            return;
        }

        AddReward(-1f);

        //-----------------------------------------
        // Logic for the agent's needs
        //-----------------------------------------
        // Clamp the agent's needs between 0 and the default value
        agentHealth = Mathf.Clamp(agentHealth, minNeed, maxNeed);
        agentThirst = Mathf.Clamp(agentThirst, minNeed, maxNeed);
        agentHunger = Mathf.Clamp(agentHunger, minNeed, maxNeed);
        agentFun = Mathf.Clamp(agentFun, minNeed, maxNeed);

        // Decrement the agent's needs over time
        DegenerateNeed(ref agentThirst, thirstBar);
        DegenerateNeed(ref agentHunger, hungerBar);
        DegenerateNeed(ref agentFun, funBar);

        // Check if the agent is using an object
        // (Actually checks if the agent is within the bounds of an interactable object)
        IsAgentUsingObject();

        // Increase agent's health if every need is above the threshold
        AgentHealthCheck();

        //-----------------------------------------
        // Discrete actions
        //-----------------------------------------
        int movement = actionBuffers.DiscreteActions[0];
        //int interact = actionBuffers.DiscreteActions[1];

        if(!isMoving)
        {
            switch(movement)
            {
                case 0:
                    Debug.Log("No movement");
                    break;
                case 1:
                    Move(Vector2.up);
                    Debug.Log("Moving up");
                    break;
                case 2:
                    Move(Vector2.down);
                    Debug.Log("Moving down");
                    break;
                case 3:
                    Move(Vector2.left);
                    Debug.Log("Moving left");
                    break;
                case 4:
                    Move(Vector2.right);
                    Debug.Log("Moving right");
                    break;
                default:
                    break;
            }
        }

        if (agentHunger < needThreshold)
        {
            float distanceToFood = Vector2.Distance(transform.position, foodTransform.position);
            AddReward(-distanceToFood * 0.01f);
        }

        if (agentThirst < needThreshold)
        {
            float distanceToWater = Vector2.Distance(transform.position, waterTransform.position);
            AddReward(-distanceToWater * 0.01f);
        }

        if (agentFun < needThreshold)
        {
            float distanceToFun = Vector2.Distance(transform.position, funTransform.position);
            AddReward(-distanceToFun * 0.01f);
        }

        // switch(interact)
        // {
        //     case 0:
        //         if (IsAgentOnObject() == true)
        //         {
        //             AgentOnObject();
        //             break;
        //         }
        //         else {
        //             AddReward(-1f);
        //             break;
        //         }
        // }
    }

    //-----------------------------------------
    // HELPER FUNCTION: Increase the agent's health if all needs are above the threshold
    //-----------------------------------------
    private void AgentHealthCheck()
    {
        // If all needs are above the threshold, increase the agent's health
        if (agentThirst >= needThreshold && agentHunger >= needThreshold && agentFun >= needThreshold)
        {
            RegenerateNeed(ref agentHealth, healthBar);
        }

        // If any need is below the threshold, decrement the agent's health
        if (agentThirst <= 0 || agentHunger <= 0 || agentFun <= 0)
        {
            if (agentHealth > 0) // If the agent's health is above 0, decrement it
            {
                DegenerateNeed(ref agentHealth, healthBar);
            }
            // If the agent's health reaches 0, end the episode
            else
            {   
                AddReward(-5f);
                EndEpisode();

                Debug.Log("Agent died");
                return;
            }
        }
    }

    //-----------------------------------------
    // HELPER FUNCTION: Randomise the agent's needs
    //-----------------------------------------
    private void ResetNeeds()
    {
        agentHealth = maxNeed;
        agentThirst = Random.Range(maxNeed / 2, maxNeed);
        agentHunger = Random.Range(maxNeed / 2, maxNeed);
        agentFun = Random.Range(maxNeed / 2, maxNeed);
    }

    //-----------------------------------------
    // HELPER FUNCTION: Degenerate a given need
    //-----------------------------------------
    public void DegenerateNeed(ref int need, StatusBar statusBar)
    {
        need--;
        statusBar.SetValue(need);
    }

    //-----------------------------------------
    // HELPER FUNCTION: Regenerate a given need
    //-----------------------------------------
    public void RegenerateNeed(ref int need, StatusBar statusBar)
    {
        need += needRegenRate;
        statusBar.SetValue(need);
    }

    //-----------------------------------------
    // HELPER FUNCTION: Agent interaction with objects
    //-----------------------------------------
    // private bool IsAgentOnObject()
    // {
    //     Vector2 agentPosition = funTransform.position;

    //     if (agentPosition == waterPosition || agentPosition == foodPosition || agentPosition == funPosition)
    //     {
    //         return true;
    //     }
    //     else
    //     {
    //         return false;
    //     }
    // }

    // Check if the agent is using an object
    private bool IsAgentUsingObject()
    {
        Vector2 agentPosition = transform.position;

        if (agentPosition == waterPosition)
        {
            RegenerateNeed(ref agentThirst, thirstBar);
            AddReward(1f);
            return true;
        }
        else if (agentPosition == foodPosition)
        {
            RegenerateNeed(ref agentHunger, hungerBar);
            AddReward(1f);
            return true;
        }
        else if (agentPosition == funPosition)
        {
            RegenerateNeed(ref agentFun, funBar);
            AddReward(1f);
            return true;
        }
        else{
            return false;
        }
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
            }
        }
    }

    void Move(Vector2 direction)
    {
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(direction.x * gridSize.x, direction.y * gridSize.y, 0);

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

}