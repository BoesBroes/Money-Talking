using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    Text dollarAmount;

    void Start()
    {
        dollarAmount = GetComponent<Text>();
    }

    public void valueUpdate(float value)
    {
        dollarAmount.text = Mathf.RoundToInt(value) + "$";
    }
}
