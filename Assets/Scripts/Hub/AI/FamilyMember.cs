using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FamilyMember : MonoBehaviour
{

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

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        obstacle = this.GetComponent<NavMeshObstacle>();
        controller = GetComponent<CharacterController>();

        agent.enabled = false;

        anim = gameObject.GetComponentInChildren<Animator>();
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
}
