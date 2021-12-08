using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeEvent : MonoBehaviour
{
    public GameObject calendar;

    private int currentDate;

    void Start()
    {
        //button = new GameObject[buttonParent.transform.childCount];
        //for (int i = 0; i < buttonParent.transform.childCount; i++)
        //{
        //    button[i] = 
        //}
    }
    public void SetDate(int date)
    {
        currentDate = date;
    }

    public void ChangeDate(string setEvent)
    {
        calendar.SetActive(true);
        calendar.transform.GetChild(currentDate).gameObject.GetComponent<CalendarDate>().eventText.text = setEvent;
    }
}
