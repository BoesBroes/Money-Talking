using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    public static StatsManager statsManager;

    [Header("stats")]
    public Slider energyBar;
    public Slider happinessBar;

    [Header("sliders")]
    public Image energyFill;
    public Image happinessFill;

    [Header("FamilyStuff")]
    public GameObject family;

    public int money;

    public Text moneyText;

    private float storeHappiness;
    private float storeEnergy;
    private bool stored;

    private GameObject lastCollided;
    private string moneyString;

    [Header("CalendarStuff")]
    public GameObject calendar;
    //TODO: probably store the stats when moving to other scene (most stats that need to be stored are probably here?)

    void Start()
    {
        if (statsManager == null)
        {
            statsManager = this;
        }
        family.GetComponent<FamilyAI>().SpawnFamily();

        moneyString = money.ToString();
        moneyText.text = ("$" + moneyString);

        stored = false;
    }

    public void ChangeEnergy(float change)
    {
        Debug.Log(change);
        if (!stored)
        {
            energyBar.value += change;
            ChangeColor(energyFill, energyBar.value);
        }
        else
        {
            energyBar.value = lastCollided.GetComponent<FamilyMember>().energy;
            ChangeColor(energyFill, energyBar.value);
            storeEnergy += change;
        }
    }

    public void ChangeHappiness(float change)
    {
        if (!stored)
        {
            happinessBar.value += change;
            ChangeColor(happinessFill, happinessBar.value);
        }
        else
        {
            happinessBar.value = lastCollided.GetComponent<FamilyMember>().happiness;
            ChangeColor(happinessFill, happinessBar.value);
            storeHappiness += change;
        }
    }

    public void ChangeMoney(int valueChange)
    {
        money += valueChange;
        moneyString = money.ToString();
        moneyText.text = ("$" + moneyString);
    }

    private void ChangeColor(Image sliderFill, float value)
    {
        Color statsColor = new Color(1 - value, value, 0, 1);

        sliderFill.color = statsColor;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //raycast from camera but at other cameras position (real funky but it works)
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(Camera.main.transform.position, ray.direction * 10, out hit))
            {
                if(hit.collider.GetComponent<FamilyMember>())
                {
                    lastCollided = hit.collider.gameObject;
                    if(!stored)
                    {
                        stored = true;
                        storeEnergy = energyBar.value;
                        storeHappiness = happinessBar.value;
                    }
                    energyBar.value = hit.collider.GetComponent<FamilyMember>().energy;
                    happinessBar.value = hit.collider.GetComponent<FamilyMember>().happiness;

                    ChangeColor(energyFill, energyBar.value);
                    ChangeColor(happinessFill, happinessBar.value);
                }
                else
                {
                    if(stored)
                    {
                        stored = false;
                        energyBar.value = storeEnergy;
                        happinessBar.value = storeHappiness;

                        ChangeColor(energyFill, energyBar.value);
                        ChangeColor(happinessFill, happinessBar.value);
                    }
                }
            }
        }
    }

}
