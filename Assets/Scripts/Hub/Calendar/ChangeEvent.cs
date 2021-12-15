using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeEvent : MonoBehaviour
{
    public GameObject[] calendarMonth;

    private int currentDate;
    private int currentMonth; //not 'current' month but the one currently showing on screen

    void Awake()
    {
        calendarMonth = new GameObject[3]; //hardcoded for now 
        currentMonth = 0;
    }
    public void SetDate(int date)
    {
        currentDate = date;
    }

    public void SetMonth(int month)
    {
        currentMonth = month;
    }

    public void ChangeDate(string setEvent)
    {
        calendarMonth[currentMonth].transform.GetChild(currentDate).GetComponent<CalendarDate>().eventText.text = setEvent;
    }
}
