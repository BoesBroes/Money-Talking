using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyAI : MonoBehaviour
{
    public GameObject[] familyMember;

    [Header("Activity spots")]
    public GameObject[] couch;
    public GameObject[] bed; //maybe assign beds to specific people so they dont mix and incest stuff happens lol
    public GameObject[] toilet;
    public GameObject[] kitchen;

    //stuff to check how many spots are occupied
    private int couchCount;
    private int bedCount;
    private int toiletCount;
    private int kitchenCount;

    private float happiness;
    private float energy;




    // Start is called before the first frame update
    public void SpawnFamily()
    {
        //probably _actually_ spawn family here?
        happiness = StatsManager.statsManager.happinessBar.value;
        energy = StatsManager.statsManager.energyBar.value;

        //set size and assign each family member
        familyMember = new GameObject[this.transform.childCount];

        for (int i = 0; i < this.transform.childCount; i++)
        {
            familyMember[i] = this.transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //add foreach family member

            switch (familyMember[0].GetComponent<FamilyMember>().currentAction)
            {
                case FamilyMember.Action.Idle:
                    familyMember[0].GetComponent<FamilyMember>().currentAction = ChooseAction(familyMember[0]);
                    break;

                case FamilyMember.Action.Resting:
                    familyMember[0].GetComponent<FamilyMember>().takingAction = true;
                    RestMember(familyMember[0]);
                    break;

                case FamilyMember.Action.WatchTV:
                    familyMember[0].GetComponent<FamilyMember>().takingAction = true;
                    WatchTV(familyMember[0]);
                    break;
            }
    }

    //more will be added
    private FamilyMember.Action ChooseAction(GameObject member)
    {
        //Random number generated for use later for the AI random decision making
        int randomNumber = Random.Range(0, 100);
        Debug.Log(randomNumber);
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
        if (happiness * 100 > 50)
        {
            if (randomNumber < 50)
            {
                return FamilyMember.Action.WatchTV;
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
        //Debug.Log("Resting");

        member.GetComponent<FamilyMember>().currentRestingTime += Time.deltaTime;


        if (member.GetComponent<FamilyMember>().currentRestingTime > member.GetComponent<FamilyMember>().randomizedRestingTime)
        {
            member.GetComponent<FamilyMember>().currentRestingTime = 0;

            StatsManager.statsManager.ChangeEnergy(.2f); //magic number

            //Resting time is randomly set for the next resting cycle
            member.GetComponent<FamilyMember>().randomizedRestingTime = Random.Range(member.GetComponent<FamilyMember>().minRestingTime, member.GetComponent<FamilyMember>().maxRestingTime);

            //Set the characters current action to idle so it can chose a new action
            member.GetComponent<FamilyMember>().currentAction = FamilyMember.Action.Idle;

            familyMember[0].GetComponent<FamilyMember>().takingAction = false;
            Debug.Log("Rested!");

        }
    }
    private void WatchTV(GameObject member)
    {
        //move to TV and stay there for a while
        Debug.Log("TV");

        StatsManager.statsManager.ChangeHappiness(.2f); //magic number

        member.GetComponent<FamilyMember>().currentAction = FamilyMember.Action.Idle;

        familyMember[0].GetComponent<FamilyMember>().takingAction = false;

    }
}
