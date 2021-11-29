using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    public Slider energyBar;
    public Slider happinessBar;

    public Image energyFill;
    public Image happinessFill;

    //TODO: probably store the stats when moving to other scene

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

    private void ChangeColor(Image sliderFill, float value)
    {
        Color statsColor = new Color(1 - value, value, 0, 1);

        sliderFill.color = statsColor;
    }
}
