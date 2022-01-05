using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HappinessButtonFather : MonoBehaviour
{
    public Text valueText;
    int progress = 0;
    public Slider slider;

    public void OnSliderChanged(float value)
    {
        valueText.text = value.ToString();
    }

    public void UpdateProgress()
    {
        progress++;
        slider.value = progress;        
    }
}
