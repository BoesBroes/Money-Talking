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

    [HideInInspector]
    public float randomWaitTime;

    private GameObject collidedObject;
    private Vector3 startPos;
    public float waitTime;
    public int attempts;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        obstacle = this.GetComponent<NavMeshObstacle>();
        controller = GetComponent<CharacterController>();

        agent.enabled = false;

        startPos = transform.position;

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
            if(!avoid)
            {
                anim.SetInteger("AnimationPar", 1);
            }
            yield return null;
        }

        anim.SetInteger("AnimationPar", 0);

        lastDestination = currentDestinationObject;

        reachedDestination = true;
        moving = false;

        agent.enabled = false;
        obstacle.enabled = true;

        attempts = 0;
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
            yield return new WaitForSeconds(Random.Range(3f, 4f));
            StartCoroutine(StoreLastDestination());
        }
    }

    IEnumerator WaitUntilAvoidanceReached(Vector3 pos)
    {
        while (Mathf.Abs((pos.z - this.transform.position.z)) > destinationOffset || Mathf.Abs((pos.x - this.transform.position.x)) > destinationOffset)
        {
            yield return null;
        }

        agent.enabled = false;
        obstacle.enabled = true;

        anim.SetInteger("AnimationPar", 0);
        yield return new WaitForSeconds(randomWaitTime);

        obstacle.enabled = false;
        agent.enabled = true;

        if (randomWaitTime > 1)
        {
            while (!collidedObject.GetComponent<FamilyMember>().reachedDestination && waitTime < 10f)
            {
                waitTime += Time.deltaTime;
                yield return null;
            }
        }

        if(waitTime >= 10 || attempts == 2)
        {
            attempts = 0;
            avoid = true;
            anim.SetInteger("AnimationPar", 1);
            agent.destination = startPos;
            StartCoroutine(WaitUntilAvoidanceReached(startPos));
        }
        else
        {
            avoid = false;
            anim.SetInteger("AnimationPar", 1);
            agent.destination = currentDestination.position;
            attempts++;
        }

        waitTime = 0;


        yield return new WaitForSeconds(4f);

        StartCoroutine(StoreLastDestination());
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "Member"  && !reachedDestination && hasStarted && !avoid)
        {
            avoid = true;
            randomWaitTime = Random.Range(1f, 5f);
            agent.destination = storedDestination;

            collidedObject = collision.gameObject;

            if(collision.GetComponent<FamilyMember>().randomWaitTime > randomWaitTime)
            {
                randomWaitTime = 0.5f;
            }
            else
            {
                randomWaitTime = 5f;
            }

            Debug.Log(agent.destination);

            StartCoroutine(WaitUntilAvoidanceReached(agent.destination));
        }
    }

    IEnumerator WaitForFirstCollision()
    { 
        yield return new WaitForSeconds(1f);
        hasStarted = true;
    }

}
