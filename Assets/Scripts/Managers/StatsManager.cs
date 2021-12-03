using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    public static StatsManager statsManager;

    public Slider energyBar;
    public Slider happinessBar;

    public Image energyFill;
    public Image happinessFill;

    public GameObject family;

    public int money;

    public Text moneyText;

    private string moneyString;
    //TODO: probably store the stats when moving to other scene

    void Start()
    {
        if(statsManager == null)
        {
            statsManager = this; 
        }
        family.GetComponent<FamilyAI>().SpawnFamily();

        moneyString = money.ToString();
        moneyText.text = ("$" + moneyString);
    }

    public void ChangeEnergy(float change)
    {
        energyBar.value += change;
        ChangeColor(energyFill, energyBar.value);
    }

    public void ChangeHappiness(float change)
    {
        happinessBar.value += change;
        ChangeColor(happinessFill, happinessBar.value);
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
}
