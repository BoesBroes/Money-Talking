using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Calendar : MonoBehaviour
{
    public static Calendar calendar;
    private Calendar _calendar;

    public GameObject calendarParent;
    public GameObject calendarObject;
    public GameObject date;

    public GameObject changeEvent;

    private GameObject currentCalendar; //current month used to create month
    private GameObject[] temporaryObject;
    private Vector3 temporaryPosition;

    public GameObject currentMonth; //current month ingame

    public GameObject[] storedMonth;
    private int storeCount;

    [HideInInspector]
    public int showCount;

    public float dayTime;
    public float timeModifier; //modifier that should affect speed of family(stats/movement)
    private float currentTime;
    private int dayCount;

    private bool inHub;
    private bool ignoreStart;
    void Awake()
    {
        if(calendar == null)
        {
            ignoreStart = false;
            calendar = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            ignoreStart = true;
            //_calendar = gameObject.AddComponent<Calendar>();
            Reactivate();

            Destroy(calendar.gameObject);
            calendar = this;
            DontDestroyOnLoad(gameObject);
        }        
    }

    void Start()
    {
        if(!ignoreStart)
        {
            inHub = true;

            storedMonth = new GameObject[2];
            //no one looks further ahead than 3 months
            CreateMonth(true);
            CreateMonth(false);
            CreateMonth(false);
            //make more months to plan in advance?
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(inHub)
        {
            DateTimer();
        }
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
                CheckDay();
            }
        }
    }    

    private void CreateMonth(bool firstMonth)
    {
        dayCount = 0;

        currentCalendar = Instantiate(calendarObject, calendarParent.transform);

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
            changeEvent.GetComponent<ChangeEvent>().calendarMonth[0] = currentMonth;
        }
        else
        {
            currentCalendar.SetActive(false);
            storedMonth[storeCount] = currentCalendar;
            changeEvent.GetComponent<ChangeEvent>().calendarMonth[storeCount +1] = storedMonth[storeCount];
            if (storeCount < 1)
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

        //do the same for ChangeEvent (make this a for loop)
        changeEvent.GetComponent<ChangeEvent>().calendarMonth[0] = currentMonth;
        changeEvent.GetComponent<ChangeEvent>().calendarMonth[1] = storedMonth[0];
        changeEvent.GetComponent<ChangeEvent>().calendarMonth[2] = storedMonth[1];

        //create new month
        CreateMonth(false);

        //add new month (and deleted one) into account 
        if(showCount >= 1)
        {
            showCount--;
            changeEvent.GetComponent<ChangeEvent>().SetMonth(showCount);
        }
    }

    private void CheckDay()
    {
        //set nice colors
        currentMonth.transform.GetChild(dayCount).GetComponent<CalendarDate>().eventImage.color = Color.red;
        currentMonth.transform.GetChild(dayCount - 1).GetComponent<CalendarDate>().eventImage.color = Color.white;
        if(LevelManager.levelManager.scenes.Contains(currentMonth.transform.GetChild(dayCount).GetComponent<CalendarDate>().eventText.text))
        {
            inHub = false;
            currentMonth.transform.SetParent(transform);

            //here is a for loop that should be used in movemonth lmao
            for(int i = 0; i < storedMonth.Length; i++)
            {
                storedMonth[i].transform.SetParent(transform);
            }
            LevelManager.levelManager.ChangeLevel(currentMonth.transform.GetChild(dayCount).GetComponent<CalendarDate>().eventText.text);
        }
    }

    public void ShowNextMonth()
    {
        //yes there are better ways to do this lmao
        showCount++;
        if(showCount == 1)
        {
            changeEvent.GetComponent<ChangeEvent>().SetMonth(showCount);
            currentMonth.SetActive(false);
            storedMonth[1].SetActive(false);
            storedMonth[0].SetActive(true);
        }

        else if (showCount == 2)
        {
            changeEvent.GetComponent<ChangeEvent>().SetMonth(showCount);
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
            changeEvent.GetComponent<ChangeEvent>().SetMonth(showCount);
            storedMonth[0].SetActive(false);
            currentMonth.SetActive(true);
        }

        else if (showCount == 1)
        {
            changeEvent.GetComponent<ChangeEvent>().SetMonth(showCount);
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

    public void Reactivate()
    {
        currentMonth.transform.SetParent(currentCalendar.transform, true);

        //here is a for loop that should be used in movemonth lmao
        for (int i = 0; i < storedMonth.Length; i++)
        {
            storedMonth[i].transform.SetParent(currentCalendar.transform, true);
        }
    }
}
