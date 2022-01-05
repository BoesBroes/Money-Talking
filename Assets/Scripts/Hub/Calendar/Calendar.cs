using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public class Calendar : MonoBehaviour
{
    public static Calendar calendar;

    public GameObject menu;

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

    private float happinessPenalty = -0.05f; //maybe public for ez balancing but not for now (also check if this actually affects stats on a personal level if we would continue this project)
    private float energyPenalty = -0.05f;
    private int monthlyIncome;

    private float energySave;
    private float happinesSave;
    private int moneySave;

    UnityAction action;

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
            Calendar.calendar.menu = menu;
            Calendar.calendar.Reactivate();

            Destroy(gameObject);
        }        
    }

    void Start()
    {
        inHub = true;

        if(buttons == null)
        {
            buttons = calendarParent.transform.GetChild(0).gameObject;
        }
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


            GameObject tempPanel = menu.GetComponent<Menu>().allPanels[0];
            Menu tempMenu = menu.GetComponent<Menu>();

            Destroy(buttons.transform.GetChild(0).GetComponent<Button>());

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

        GameObject tempPanel = menu.GetComponent<Menu>().allPanels[0];
        Menu tempMenu = menu.GetComponent<Menu>();

        Debug.Log(tempPanel.name);


        buttons.transform.GetChild(0).gameObject.AddComponent<Button>();
        buttons.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate { tempMenu.SwitchPanel(tempPanel); });


        buttons.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { calendar.ShowNextMonth(); });
        buttons.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { calendar.ShowMonthBack(); });

        temporaryObject = new GameObject[currentMonth.transform.childCount];

        changeEvent.GetComponent<ChangeEvent>().calendarMonth = new GameObject[3];

        changeEvent.GetComponent<ChangeEvent>().calendarMonth[0] = currentMonth;

        int tempInt = currentMonth.transform.childCount;

        for (int i = 0; i < tempInt; i++)
        {
            temporaryObject[i] = Instantiate(date, currentMonth.transform.GetChild(i));
            temporaryObject[i].SetActive(true);
            temporaryObject[i].GetComponent<CalendarDate>().date = i;
            temporaryObject[i].GetComponent<CalendarDate>().eventChanger = changeEvent;


            temporaryObject[i].GetComponent<CalendarDate>().eventText.text = currentMonth.transform.GetChild(i).GetComponent<CalendarDate>().eventText.text;

            Destroy(currentMonth.transform.GetChild(i).gameObject);

            temporaryObject[i].transform.SetParent(currentMonth.transform, true);
            temporaryObject[i].transform.localScale = new Vector3(1, 1, 1);
            currentMonth.transform.localScale = new Vector3(1, 1, 1);
        }

        //currentMonth.transform.localScale = new Vector3(1, 1, 1);

        //here is a for loop that should be used in movemonth lmao
        for (int i = 0; i < storedMonth.Length; i++)
        {
            storedMonth[i].transform.SetParent(calendarParent.transform, true);
            storedMonth[i].transform.localScale = new Vector3(1, 1, 1);

            changeEvent.GetComponent<ChangeEvent>().calendarMonth[i + 1] = storedMonth[i];

            //I heard you liked for loops..
            for (int x = 0; x < tempInt; x++)
            {
                temporaryObject[x] = Instantiate(date, storedMonth[i].transform.GetChild(x));
                temporaryObject[x].SetActive(true);
                temporaryObject[x].GetComponent<CalendarDate>().date = x;
                temporaryObject[x].GetComponent<CalendarDate>().eventChanger = changeEvent;


                temporaryObject[x].GetComponent<CalendarDate>().eventText.text = storedMonth[i].transform.GetChild(x).GetComponent<CalendarDate>().eventText.text;

                Destroy(storedMonth[i].transform.GetChild(x).gameObject);

                temporaryObject[x].transform.SetParent(storedMonth[i].transform, true);
                temporaryObject[x].transform.localScale = new Vector3(1, 1, 1);
                storedMonth[i].transform.localScale = new Vector3(1, 1, 1);
            }
        }

        inHub = true;
    }
}
