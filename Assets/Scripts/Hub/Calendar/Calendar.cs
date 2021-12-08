using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Calendar : MonoBehaviour
{
    public GameObject calendarParent;
    public GameObject calendar;
    public GameObject date;

    public GameObject changeEvent;

    private GameObject currentCalendar; //current month used to create month
    private GameObject[] temporaryObject;
    private Vector3 temporaryPosition;

    private GameObject currentMonth; //current month ingame
    public GameObject[] storedMonth;
    private int storeCount;
    private int showCount;

    public float dayTime;
    public float timeModifier; //modifier that should affect speed of family(stats/movement)
    private float currentTime;
    private int dayCount;
    // Start is called before the first frame update
    void Start()
    {
        storedMonth = new GameObject[2];
        //no one looks further ahead than 3 months
        CreateMonth(true);
        CreateMonth(false);
        CreateMonth(false);
        //make more months to plan in advance?
    }

    // Update is called once per frame
    void Update()
    {
        DateTimer();
    }

    private void DateTimer()
    {
        currentTime += (Time.deltaTime * timeModifier);
        if(currentTime > dayTime)
        {
            currentTime = 0;
            dayCount++;
            if(dayCount > 19) //yes on this planet we only have 20 days, deal with it
            {
                MoveMonth();
                //add and substract income and expenses?
            }
            else
            {
                currentMonth.transform.GetChild(dayCount).GetComponent<CalendarDate>().eventImage.color = Color.red;
                currentMonth.transform.GetChild(dayCount - 1).GetComponent<CalendarDate>().eventImage.color = Color.white;
            }
        }
    }    

    private void CreateMonth(bool firstMonth)
    {
        dayCount = 0;

        currentCalendar = Instantiate(calendar, calendarParent.transform);

        changeEvent.GetComponent<ChangeEvent>().calendar = currentCalendar;

        temporaryObject = new GameObject[currentCalendar.transform.childCount];
        for (int i = 0; i < currentCalendar.transform.childCount; i++)
        {
            temporaryPosition = new Vector3(currentCalendar.transform.GetChild(i).transform.position.x, currentCalendar.transform.GetChild(i).transform.position.y, 0);
            Destroy(currentCalendar.transform.GetChild(i).gameObject);
            temporaryObject[i] = Instantiate(date, temporaryPosition, transform.rotation);
            temporaryObject[i].SetActive(true);
            temporaryObject[i].GetComponent<CalendarDate>().date = i;
            temporaryObject[i].GetComponent<CalendarDate>().eventChanger = changeEvent;
        }
        for (int i = 0; i < temporaryObject.Length; i++)
        {
            temporaryObject[i].transform.SetParent(currentCalendar.transform, true);
            temporaryObject[i].transform.localScale = new Vector3(1, 1, 1);
        }

        if(firstMonth)
        {
            temporaryObject[0].GetComponent<CalendarDate>().eventImage.color = Color.red;
            currentMonth = currentCalendar;
        }
        else
        {
            currentCalendar.SetActive(false);
            storedMonth[storeCount] = currentCalendar;
            if(storeCount < 1)
            {
                storeCount++;
            }
        }
    }

    private void MoveMonth()
    {
        Destroy(currentMonth.gameObject);

        //set new month 
        currentMonth = storedMonth[0];
        currentMonth.transform.GetChild(0).GetComponent<CalendarDate>().eventImage.color = Color.red;
        currentMonth.SetActive(true);
        storedMonth[0] = storedMonth[1];

        //create new month
        CreateMonth(false);

        //add new month (and deleted one) into account 
        if(showCount >= 1)
        {
            showCount--;
        }
    }

    public void ShowNextMonth()
    {
        showCount++;
        if(showCount == 1)
        {
            currentMonth.SetActive(false);
            storedMonth[1].SetActive(false);
            storedMonth[0].SetActive(true);
        }

        else if (showCount == 2)
        {
            storedMonth[0].SetActive(false);
            storedMonth[1].SetActive(true);
        }

        else if(showCount > 2)
        {
            showCount = 2;
            Debug.Log("out of months");
        }
    }

    public void ShowMonthBack()
    {
        showCount--;
        if (showCount == 0)
        {
            storedMonth[0].SetActive(false);
            currentMonth.SetActive(true);
        }

        else if (showCount == 1)
        {
            currentMonth.SetActive(false);
            storedMonth[1].SetActive(false);
            storedMonth[0].SetActive(true);
        }

        else if(showCount < 0)
        {
            showCount = 0;
            Debug.Log("out of months");
        }
    }
}
