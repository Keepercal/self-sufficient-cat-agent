// Run command for training
// Note: activate the virtual enviromenmt first using "conda activate mlagents"

// Then navigate to the /Users/Callum/DevTools/Git Clones/ml-agents directory and run the command below

// mlagents-learn config/cat_config.yaml --run-id=CatAgent --force

// NOTE: The ML-Agents github repo is a dependancy and instructions will be needed for markers to run the simulation themselves

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

    public Vector2 gridSize = new Vector2(1, 1); // The size of each grid cell in the environment
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
    private static int defaultValue = 500;

    private static int maxValue = defaultValue;
    private static int minValue = defaultValue / 2;

    private static float highValue = defaultValue / 1.5f;
    private static float midValue = defaultValue / 2f;
    private static float lowValue = defaultValue / 4f;
    private static float criticalValue = defaultValue / 8f;

    private static float highValueNormalised = highValue / defaultValue;
    private static float midValueNormalised = midValue / defaultValue;
    private static float lowValueNormalised = lowValue / defaultValue;
    private static float criticalValueNormalised = criticalValue / defaultValue;

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
    private int regenRate = 5; // The rate at which the agent's needs regenerate

    private Vector2 targetPosition; // The target position for the agent to move to
    private Vector2 previousAgentPosition; // The agent's previous position

    //-----------------------------------------
    // METHOD: Called when the simulation is initialised
    //-----------------------------------------
    void Start()
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

        Debug.Log("Water position: " + waterPosition);
        Debug.Log("Food position: " + foodPosition);
        Debug.Log("Fun position: " + funPosition);
    }

    //-----------------------------------------
    // METHOD: Collect the agent's observations, called every step
    //-----------------------------------------
    public override void CollectObservations(VectorSensor sensor)
    {   
        //-----------------------------------------
        // Observations about the agent's state
        //-----------------------------------------
        sensor.AddObservation(this.transform.position); // Po sition

        //-----------------------------------------
        // Observations about the agent's needs, normalised to range of -1,1
        //-----------------------------------------
        sensor.AddObservation(agentHealth / 1000f);
        sensor.AddObservation(agentThirst / 1000f); 
        sensor.AddObservation(agentHunger / 1000f);
        sensor.AddObservation(agentFun / 1000f); 

        //-----------------------------------------
        // Observations about the environment
        //-----------------------------------------
        sensor.AddObservation(waterTransform.position); 
        sensor.AddObservation(foodTransform.position);
        sensor.AddObservation(funTransform.position); 

        sensor.AddObservation(Vector2.Distance(transform.position, waterTransform.position));
        sensor.AddObservation(Vector2.Distance(transform.position, foodTransform.position));
        sensor.AddObservation(Vector2.Distance(transform.position, funTransform.position));
    }

    //-----------------------------------------
    // METHOD: Called when the agent takes an action, called every step
    //-----------------------------------------
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {   
        rBody.velocity = Vector2.zero; // Reset the agent's velocity

        //-----------------------------------------
        // Logic for the agent's needs
        //-----------------------------------------
        // Clamp the agent's needs between 0 and the default value
        agentHealth = Mathf.Clamp(agentHealth, 0, defaultValue);
        agentThirst = Mathf.Clamp(agentThirst, 0, defaultValue);
        agentHunger = Mathf.Clamp(agentHunger, 0, defaultValue);
        agentFun = Mathf.Clamp(agentFun, 0, defaultValue);

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

        //-----------------------------------------
        // Reward or punish the agent based on it's distance to the action target 
        //-----------------------------------------
        float guideReward = 0;
        
        guideReward += GuideAgent(transform.position, waterTransform.position, previousAgentPosition);
        guideReward += GuideAgent(transform.position, foodTransform.position, previousAgentPosition);
        guideReward += GuideAgent(transform.position, funTransform.position, previousAgentPosition);

        AddReward(guideReward);

        //-----------------------------------------
        // Calculate the reward for the agent in the current step
        //-----------------------------------------
        float stepReward = CalculateStepReward();
        AddReward(stepReward);

        //-----------------------------------------
        // Set the agent's current position as the previous position before the next step
        //-----------------------------------------
        previousAgentPosition = transform.position;
    }

    //-----------------------------------------
    // Calulcate the reward for each step
    // NOTE: The agent's needs are normalised to the range of -1,1
    //-----------------------------------------
    private float CalculateStepReward()
    {
        float totalReward = 0f;

        //-----------------------------------------
        // Normalise the agent's needs
        //-----------------------------------------
        float normalisedHealth = Normalise(ref agentHealth);
        float normalisedThirst = Normalise(ref agentThirst);
        float normalisedHunger = Normalise(ref agentHunger);
        float normalisedFun = Normalise(ref agentFun);

        float minNeed = Mathf.Min(normalisedThirst, normalisedHunger, normalisedFun);

        //-----------------------------------------
        // Health rewards
        //-----------------------------------------
        totalReward += HealthReward(normalisedHealth);

        //-----------------------------------------
        // Needs rewards
        //-----------------------------------------
        totalReward += NeedsReward(minNeed);

        return totalReward;
    }

    //-----------------------------------------
    // Reward the agent based on its distance to the target
    //-----------------------------------------
    private float GuideAgent(Vector2 currentAgentPosition, Vector2 targetPosition, Vector2 previousAgentPosition)
    {
        float currentDistanceToTarget = Vector2.Distance(currentAgentPosition, targetPosition);
        float previousDistanceToTarget = Vector2.Distance(previousAgentPosition, targetPosition);

        float distanceChange = previousDistanceToTarget - currentDistanceToTarget;

        if (currentDistanceToTarget < previousDistanceToTarget)
        {
            return 0.2f;
        }
        else
        {
            return -0.1f;
        }
    }

    //-----------------------------------------
    // HELPER FUNCTIONS: Calculate the rewards
    // NOTE: Parameters are normalised
    //-----------------------------------------

    private float NeedsReward(float minNeed)
    {
        float reward = 0f;
        // Reward the agent for its minimum need begin high
        if (minNeed >= midValueNormalised)
        {
            reward += 0.1f;
        }
        // Punish the agent if any need is critically low
        else if (minNeed <= criticalValueNormalised)
        {
            reward += -0.2f;
        }

        return reward;
    }

    private float HealthReward(float health)
    {
        float reward = 0f;
        // Punish the agent for having low health
        if (agentThirst <= 0 || agentHunger <= 0 || agentFun <= 0)
        {
            if (agentHealth > 0) 
            {
                AddReward(-0.2f);
            }
        }
        // Reward the agent for having full health
        if (health >= maxValue) 
        {
            reward += 0.1f;
        }
        // Punish the agent if health is critically low
        if (health <= criticalValueNormalised)
        {
            reward += -0.2f;
        }
        
        return reward;
    }

    private float DecisionReward(float need)
    {
        float reward = 0f;

        if (need <= criticalValueNormalised)
            reward += 0.1f;
        else
            reward += -0.2f;

        return reward;
    }

    //-----------------------------------------
    // HELPER FUNCTION: Increase the agent's health if all needs are above the threshold
    //-----------------------------------------
    private void AgentHealthCheck()
    {
        // If all needs are above the threshold, increase the agent's health
        if (agentThirst >= midValue && agentHunger >= midValue && agentFun >= midValue)
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
        agentHealth = defaultValue;
        agentThirst = Random.Range(minValue, maxValue);
        agentHunger = Random.Range(minValue, maxValue);
        agentFun = Random.Range(minValue, maxValue);
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
        need += regenRate;
        statusBar.SetValue(need);
    }

    //-----------------------------------------
    // HELPER FUNCTION: Normalise the agent's needs to the range of -1,1
    //-----------------------------------------
    private float Normalise(ref int value)
    {
        return value / defaultValue;
    }

    //-----------------------------------------
    // HELPER FUNCTION: Agent interaction with objects
    //-----------------------------------------
    // Check if the agent is using an object
    private void IsAgentUsingObject()
    {
        Vector2 agentPosition = transform.position;

        if (agentPosition == waterPosition)
        {
            RegenerateNeed(ref agentThirst, thirstBar);
            AddReward(0.1f);
        }
        else if (agentPosition == foodPosition)
        {
            RegenerateNeed(ref agentHunger, hungerBar);
            AddReward(0.1f);
        }
        else if (agentPosition == funPosition)
        {
            RegenerateNeed(ref agentFun, funBar);
            AddReward(0.1f);
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