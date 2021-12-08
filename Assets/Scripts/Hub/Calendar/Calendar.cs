using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar : MonoBehaviour
{
    public GameObject date;
    private GameObject[] temporaryObject;
    private Vector3 temporaryPosition;
    // Start is called before the first frame update
    void Start()
    {
        temporaryObject = new GameObject[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++) 
        {
            temporaryPosition = new Vector3(this.transform.GetChild(i).transform.position.x, this.transform.GetChild(i).transform.position.y, 0);
            Destroy(this.transform.GetChild(i).gameObject);
            temporaryObject[i] = Instantiate(date, temporaryPosition, transform.rotation);
            temporaryObject[i].SetActive(true);
            temporaryObject[i].GetComponent<CalendarDate>().date = i;
        }
        for (int i = 0; i < temporaryObject.Length; i++)
        {
            temporaryObject[i].transform.SetParent(this.transform, true);
            temporaryObject[i].transform.localScale = new Vector3(1, 1, 1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
