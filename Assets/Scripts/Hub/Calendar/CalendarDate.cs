using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarDate : MonoBehaviour
{
    public Text eventText;
    public Image eventImage;
    public int date;

    public GameObject eventChanger;
    // Start is called before the first frame update
    void Start()
    {
         
    }

    public void SetDate()
    {
        eventChanger.GetComponent<ChangeEvent>().SetDate(date);
    }

    public void AddEvent()
    {
        //add event
    }
}
