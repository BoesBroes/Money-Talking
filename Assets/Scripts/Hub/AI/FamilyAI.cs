using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FamilyAI : MonoBehaviour
{
    public GameObject[] familyMember;

    [Header("parent activity spots")]
    public GameObject couch;
    public GameObject bed; //maybe assign beds to specific people so they dont mix and incest stuff happens lol
    public GameObject toilet;
    public GameObject kitchen;

    private GameObject[] couches;
    private GameObject[] beds; //maybe assign beds to specific people so they dont mix and incest stuff happens lol
    private GameObject[] toilets;
    private GameObject[] kitchens;

    //stuff to check how many spots are occupied
    private int couchCount;
    private int bedCount;
    private int toiletCount;
    private int kitchenCount;

    private float happiness;
    private float energy;

    //could make these public for ez changes perhaps
    private float activityGain = .2f;
    private float activityDrain = -.2f; //drain should not be affected by money?

    private float memberCountModifier;
    private float currencyModifier = 1;

    private bool activityDone = false;


    // Start is called before the first frame update
    public void SpawnFamily()
    {
        //probably _actually_ spawn family here?
        GetStats();

        //set size and assign each family member and modifiers
        memberCountModifier = 0;
        SetModifiers();

        //do the same but for each spot
        couches = new GameObject[couch.transform.childCount];
        beds = new GameObject[bed.transform.childCount];
        toilets = new GameObject[toilet.transform.childCount];
        kitchens = new GameObject[kitchen.transform.childCount];

        for (int i = 0; i < couch.transform.childCount; i++)
        {
            couches[i] = couch.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < bed.transform.childCount; i++)
        {
            beds[i] = bed.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < toilet.transform.childCount; i++)
        {
            toilets[i] = toilet.transform.GetChild(i).gameObject;
        }
        for (int i = 0; i < kitchen.transform.childCount; i++)
        {
            kitchens[i] = kitchen.transform.GetChild(i).gameObject;
        }

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < familyMember.Length; i++)
        {
            switch (familyMember[i].GetComponent<FamilyMember>().currentAction)
            {
                case FamilyMember.Action.Idle:
                    activityDone = false;
                    familyMember[i].GetComponent<FamilyMember>().currentAction = ChooseAction(familyMember[i]);
                    break;

                case FamilyMember.Action.Resting:
                    familyMember[i].GetComponent<FamilyMember>().takingAction = true;
                    RestMember(familyMember[i]);
                    break;

                case FamilyMember.Action.WatchTV:
                    familyMember[i].GetComponent<FamilyMember>().takingAction = true;
                    WatchTV(familyMember[i]);
                    break;

                case FamilyMember.Action.Cook:
                    familyMember[i].GetComponent<FamilyMember>().takingAction = true;
                    Cook(familyMember[i]);
                    break;
            }
        }
    }

    //more will be added
    private FamilyMember.Action ChooseAction(GameObject member)
    {
        //Random number generated for use later for the AI random decision making and get current family stats
        int randomNumber = Random.Range(0, 100);
        GetStats();

        if(happiness * 100 < 50)
        {
            if(randomNumber < 50)
            {
                return FamilyMember.Action.WatchTV;
            }

            if(randomNumber > 50)
            {
                return FamilyMember.Action.Resting;
            }
        }

        //just for testings sake
        if (happiness * 100 > 50 && energy * 100 > 50)
        {
            if (randomNumber < 75)
            {
                return FamilyMember.Action.Cook;
            }

            if (randomNumber > 50)
            {
                return FamilyMember.Action.Resting;
            }
        }

        //if none are true
        return FamilyMember.Action.Idle;
    }

    private void RestMember(GameObject member)
    {

        if (!member.GetComponent<FamilyMember>().moving && !member.GetComponent<FamilyMember>().reachedDestination)
        {
            Debug.Log("Bed start");

            //if already at location

            if (beds.Contains(member.GetComponent<FamilyMember>().lastDestination))
            {
                Debug.Log("Already there");
                member.GetComponent<FamilyMember>().reachedDestination = true;
            }
            
            
            else if (bedCount < beds.Length)
            {
                if (!beds[bedCount].GetComponent<Occupation>().occupied)
                {
                    member.GetComponent<FamilyMember>().MoveToAction(beds[bedCount]);
                    beds[bedCount].GetComponent<Occupation>().occupied = true;
                    bedCount = 0;
                }
                else
                {
                    bedCount++;
                }
            }
            else
            {
                bedCount = 0;

                //Set the characters current action to idle so it can chose a new action
                member.GetComponent<FamilyMember>().currentAction = FamilyMember.Action.Idle;

                familyMember[0].GetComponent<FamilyMember>().takingAction = false;
                Debug.Log("all occupied:(");
            }
        }

        else if (member.GetComponent<FamilyMember>().reachedDestination)
        {
            ActivityTimer(member);

            if (activityDone)
            {
                SetModifiers();

                StatsManager.statsManager.ChangeEnergy(activityGain * (memberCountModifier * currencyModifier)); 

                //Set the characters current action to idle so it can chose a new action
                member.GetComponent<FamilyMember>().currentAction = FamilyMember.Action.Idle;

                familyMember[0].GetComponent<FamilyMember>().takingAction = false;
                Debug.Log("Rested!");
            }
        }
    }
    private void WatchTV(GameObject member)
    {
        //move to TV and stay there for a while

        if(!member.GetComponent<FamilyMember>().moving && !member.GetComponent<FamilyMember>().reachedDestination)
        {
            Debug.Log("TV Start");

            //if already at location

            if (couches.Contains(member.GetComponent<FamilyMember>().lastDestination))
            {
                Debug.Log("Already there");
                member.GetComponent<FamilyMember>().reachedDestination = true;
            }
            

            else if (couchCount < couches.Length)
            {
                if (!couches[couchCount].GetComponent<Occupation>().occupied)
                {
                    member.GetComponent<FamilyMember>().MoveToAction(couches[couchCount]);
                    couches[couchCount].GetComponent<Occupation>().occupied = true;
                    couchCount = 0;
                }
                else
                {
                    couchCount++;
                }
            }

            else
            {
                couchCount = 0;

                //Set the characters current action to idle so it can chose a new action
                member.GetComponent<FamilyMember>().currentAction = FamilyMember.Action.Idle;

                familyMember[0].GetComponent<FamilyMember>().takingAction = false;
                Debug.Log("all occupied:(");
            }
        }

        else if(member.GetComponent<FamilyMember>().reachedDestination)
        {
            ActivityTimer(member);

            if(activityDone)
            {
                Debug.Log("TV Finish");

                SetModifiers();

                StatsManager.statsManager.ChangeHappiness(activityGain * (memberCountModifier * currencyModifier)); 

                member.GetComponent<FamilyMember>().currentAction = FamilyMember.Action.Idle;

                member.GetComponent<FamilyMember>().reachedDestination = false;

                familyMember[0].GetComponent<FamilyMember>().takingAction = false;
            }
        }
    }

    private void Cook(GameObject member)
    {
        if (!member.GetComponent<FamilyMember>().moving && !member.GetComponent<FamilyMember>().reachedDestination)
        {
            Debug.Log("Kitchen");

            //if already at location

            if (kitchens.Contains(member.GetComponent<FamilyMember>().lastDestination))
            {
                Debug.Log("Already there");
                member.GetComponent<FamilyMember>().reachedDestination = true;
            }


            else if (kitchenCount < kitchens.Length)
            {
                if (!kitchens[kitchenCount].GetComponent<Occupation>().occupied)
                {
                    member.GetComponent<FamilyMember>().MoveToAction(kitchens[kitchenCount]);
                    kitchens[kitchenCount].GetComponent<Occupation>().occupied = true;
                    couchCount = 0;
                }
                else
                {
                    kitchenCount++;
                }
            }

            else
            {
                kitchenCount = 0;

                //Set the characters current action to idle so it can chose a new action
                member.GetComponent<FamilyMember>().currentAction = FamilyMember.Action.Idle;

                familyMember[0].GetComponent<FamilyMember>().takingAction = false;
                Debug.Log("all occupied:(");
            }
        }

        else if (member.GetComponent<FamilyMember>().reachedDestination)
        {
            ActivityTimer(member);

            if (activityDone)
            {
                Debug.Log("TV Finish");

                SetModifiers();

                StatsManager.statsManager.ChangeHappiness(activityDrain * memberCountModifier); 

                member.GetComponent<FamilyMember>().currentAction = FamilyMember.Action.Idle;

                member.GetComponent<FamilyMember>().reachedDestination = false;

                familyMember[0].GetComponent<FamilyMember>().takingAction = false;
            }
        }
    }

    private void ActivityTimer(GameObject member)
    {
        member.GetComponent<FamilyMember>().currentActivityTime += Time.deltaTime;
        if (member.GetComponent<FamilyMember>().currentActivityTime > member.GetComponent<FamilyMember>().randomizedActivityTime)
        {
            //reset current activitytime
            member.GetComponent<FamilyMember>().currentActivityTime = 0;

            //Resting time is randomly set for the next resting cycle
            member.GetComponent<FamilyMember>().randomizedActivityTime = Random.Range(member.GetComponent<FamilyMember>().minActivityTime, member.GetComponent<FamilyMember>().maxActivityTime);

            activityDone = true;
        }
    }

    public void GetStats()
    {
        happiness = StatsManager.statsManager.happinessBar.value;
        energy = StatsManager.statsManager.energyBar.value;
    }
    public void SetModifiers()
    {
        if(memberCountModifier == 0)
        {
            familyMember = new GameObject[this.transform.childCount];
            memberCountModifier = memberCountModifier / this.transform.childCount;

            for (int i = 0; i < this.transform.childCount; i++)
            {
                familyMember[i] = this.transform.GetChild(i).gameObject;
            }
        }
        
        currencyModifier = StatsManager.statsManager.money / 500;
        if (currencyModifier < 0.25f)
        {
            currencyModifier = 0.25f;
        }
        else if (currencyModifier > 2)
        {
            currencyModifier = 2;
        }
    }
}
