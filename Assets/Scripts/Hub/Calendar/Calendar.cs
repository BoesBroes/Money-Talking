using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Calendar : MonoBehaviour
{
    public static Calendar calendar;

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

    private GameObject buttons;

    private bool inHub;
    private bool ignoreStart;

    public GameObject stats;
    public int income = 100;

    private float happinessPenalty = -0.2f; //maybe public for ez balancing but not for now
    private float energyPenalty = -0.2f;
    private int monthlyIncome;

    private float energySave;
    private float happinesSave;
    private int moneySave;
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
            Calendar.calendar.calendarParent = calendarParent;
            Calendar.calendar.date = date;
            Calendar.calendar.changeEvent = changeEvent;
            Calendar.calendar.stats = stats;
            Calendar.calendar.Reactivate();

            Destroy(gameObject);
        }        
    }

    void Start()
    {
        inHub = true;

        buttons = calendarParent.transform.GetChild(0).gameObject;
        //buttons.SetActive(true);

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

        stats.GetComponent<StatsManager>().ChangeMoney(monthlyIncome);
        monthlyIncome = 0;

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
            buttons.transform.SetParent(transform);

            //here is a for loop that should be used in movemonth lmao (read other comment)
            for (int i = 0; i < storedMonth.Length; i++)
            {
                storedMonth[i].transform.SetParent(transform);
            }

            //save stats
            energySave = stats.GetComponent<StatsManager>().energyBar.value;
            happinesSave = stats.GetComponent<StatsManager>().happinessBar.value;
            moneySave = stats.GetComponent<StatsManager>().money;

            LevelManager.levelManager.ChangeLevel(currentMonth.transform.GetChild(dayCount).GetComponent<CalendarDate>().eventText.text);
        }

        //earn money here
        else if(currentMonth.transform.GetChild(dayCount).GetComponent<CalendarDate>().eventText.text == "work")
        {
            monthlyIncome += income;
            stats.GetComponent<StatsManager>().ChangeHappiness(happinessPenalty);
            stats.GetComponent<StatsManager>().ChangeEnergy(energyPenalty);
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
        stats.GetComponent<StatsManager>().energyBar.value = energySave;
        stats.GetComponent<StatsManager>().happinessBar.value = happinesSave;
        stats.GetComponent<StatsManager>().money = moneySave;

        currentMonth.transform.SetParent(calendarParent.transform, true);
        buttons.transform.SetParent(calendarParent.transform, true);
        buttons.transform.localScale = new Vector3(1, 1, 1);

        buttons.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { calendar.ShowNextMonth(); });
        buttons.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { calendar.ShowMonthBack(); });

        for (int i = 0; i < currentMonth.transform.childCount; i++)
        {
            currentMonth.transform.GetChild(i).GetComponent<CalendarDate>().eventChanger = changeEvent;
            Instantiate(date.transform.GetChild(0), currentMonth.transform.GetChild(i));
            DestroyImmediate(currentMonth.transform.GetChild(i).transform.GetChild(0).gameObject);
        }

        currentMonth.transform.localScale = new Vector3(1, 1, 1);

        //here is a for loop that should be used in movemonth lmao
        for (int i = 0; i < storedMonth.Length; i++)
        {
            storedMonth[i].transform.SetParent(calendarParent.transform, true);
            storedMonth[i].transform.localScale = new Vector3(1, 1, 1);

            //I heard you liked for loops..
            for (int x = 0; x < storedMonth[i].transform.childCount; x++)
            {
                storedMonth[i].transform.GetChild(x).GetComponent<CalendarDate>().eventChanger = changeEvent;
                Instantiate(date.transform.GetChild(0), storedMonth[i].transform.GetChild(x));
                Destroy(storedMonth[i].transform.GetChild(x).transform.GetChild(0).gameObject);
            }
        }

        inHub = true;
    }
}
