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
    private Animator anim;
    private CharacterController controller;

    private Transform currentDestination;
    private GameObject currentDestinationObject;

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        controller = GetComponent<CharacterController>();
        anim = gameObject.GetComponentInChildren<Animator>();
    }

    public void MoveToAction(GameObject destination)
    {
        moving = true;
        reachedDestination = false;
        currentDestination = destination.transform;

        currentDestinationObject = destination;

        agent.destination = currentDestination.position;

        StartCoroutine(WaitUntilReached());
    }

    IEnumerator WaitUntilReached()
    {
        while ((Mathf.Abs((currentDestination.position.y - this.transform.position.y)) > destinationOffset && Mathf.Abs((currentDestination.position.x - this.transform.position.x)) > destinationOffset))
        {
            anim.SetInteger("AnimationPar", 1);
            yield return null;
        }
        anim.SetInteger("AnimationPar", 0);

        currentDestinationObject.GetComponent<Occupation>().occupied = false;

        reachedDestination = true;
        moving = false;
    }
}
