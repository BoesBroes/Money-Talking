using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FamilyMember : MonoBehaviour
{
    public float energy;
    public float happiness;

    public enum Mood
    {
        Happy,
        Unhappy,
        Tired,
        Rested,
        Entertained,
        Bored
    }

    public enum Action
    {
        Walking,
        WatchTV,
        Resting,
        Idle,
        Talk,
        Cook,
        ToTheLoo,
        Nothing
    }

    //hide in inspector in the future
    public Mood currentMood = Mood.Happy;
    public Action currentAction = Action.Idle;

    public bool takingAction = false;

    [Header("Resting Settings")]
    public float maxActivityTime = 20f;
    public float minActivityTime = 2f;
    [HideInInspector]
    public float randomizedActivityTime = 5f;
    [HideInInspector]
    public float currentActivityTime = 0f;

    [Header("Destination Accuracy")]
    public float destinationOffset = 0.1f;

    public bool reachedDestination = false;
    public bool moving = false; 

    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;
    private Animator anim;
    private CharacterController controller;

    private Transform currentDestination;
    private GameObject currentDestinationObject;

    [HideInInspector]
    public GameObject lastDestination;

    private bool avoid = false;
    private Vector3 lastStoredDestination;
    private Vector3 storedDestination; // for avoidance
    private bool hasStarted = false; //avoid stupid errors

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        obstacle = this.GetComponent<NavMeshObstacle>();
        controller = GetComponent<CharacterController>();

        agent.enabled = false;

        anim = gameObject.GetComponentInChildren<Animator>();
        StartCoroutine(WaitForFirstCollision());
    }

    public void MoveToAction(GameObject destination)
    {
        moving = true;
        reachedDestination = false;

        if (lastDestination)
        {
            lastDestination.GetComponent<Occupation>().occupied = false;
        }

        obstacle.enabled = false;
        agent.enabled = true;

        currentDestination = destination.transform;

        currentDestinationObject = destination;

        agent.destination = currentDestination.position;

        StartCoroutine(StoreLastDestination());
        StartCoroutine(WaitUntilReached());
    }

    IEnumerator WaitUntilReached()
    {
        while (Mathf.Abs((currentDestination.position.z - this.transform.position.z)) > destinationOffset || Mathf.Abs((currentDestination.position.x - this.transform.position.x)) > destinationOffset)
        {
            anim.SetInteger("AnimationPar", 1);
            yield return null;
        }

        anim.SetInteger("AnimationPar", 0);

        lastDestination = currentDestinationObject;

        reachedDestination = true;
        moving = false;

        agent.enabled = false;
        obstacle.enabled = true;
    }

    IEnumerator StoreLastDestination()
    {
        if(!avoid)
        {
            if(storedDestination == null)
            {
                lastStoredDestination = transform.position;
                storedDestination = lastStoredDestination;
            }
            else
            {
                storedDestination = lastStoredDestination;
                lastStoredDestination = transform.position;
            }
        }
        if(!reachedDestination && !avoid)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            StartCoroutine(StoreLastDestination());
        }
    }

    IEnumerator WaitUntilAvoidanceReached()
    {
        while (Mathf.Abs((storedDestination.z - this.transform.position.z)) > destinationOffset || Mathf.Abs((storedDestination.x - this.transform.position.x)) > destinationOffset)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 5f));
        }

        avoid = false;

        agent.destination = currentDestination.position;

        yield return new WaitForSeconds(4f);

        StartCoroutine(StoreLastDestination());
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Member"  && !reachedDestination && hasStarted && !avoid)
        {
            avoid = true;
            agent.destination = storedDestination;
            StartCoroutine(WaitUntilAvoidanceReached());
        }
    }

    IEnumerator WaitForFirstCollision()
    { 
        yield return new WaitForSeconds(1f);
        hasStarted = true;
    }

}
