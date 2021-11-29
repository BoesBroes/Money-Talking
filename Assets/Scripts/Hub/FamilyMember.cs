using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public float maxRestingTime = 20f;
    public float minRestingTime = 2f;
    [HideInInspector]
    public float randomizedRestingTime = 5f;
    [HideInInspector]
    public float currentRestingTime = 0f;
}
