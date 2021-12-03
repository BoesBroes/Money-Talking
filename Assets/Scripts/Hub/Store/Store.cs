using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Store : MonoBehaviour
{
    public Menu menu;
    public GameObject hubPanel;

    [Header("Customization Objects")]
    public GameObject changeTV;
    //add more

    public GameObject storeOptions;
    private GameObject[] storeType;

    public GameObject temporaryObject;
    private Vector3 temporaryVector;
    public Camera storeCamera;

    public Text[] costs;
    public GameObject costsParent;
    void Start()
    {
        storeType = new GameObject[this.transform.childCount];
        costs = new Text[costsParent.transform.childCount];
        Debug.Log(costsParent.transform.childCount);

        for (int i = 0; i < storeOptions.transform.childCount; i++)
        {
            storeType[i] = storeOptions.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < costsParent.transform.childCount; i++)
        {
            costs[i] = costsParent.transform.GetChild(i).GetComponent<Text>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //raycast from camera but at other cameras position (real funky but it works)
            RaycastHit hit;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Debug.DrawRay(storeCamera.transform.position, ray.direction * 6, Color.yellow);
            
            if (Physics.Raycast(storeCamera.transform.position, ray.direction * 6, out hit))
            {
                //subtract costs
                StatsManager.statsManager.ChangeMoney(hit.collider.GetComponentInParent<Cost>().cost);

                //place new object
                temporaryObject = hit.collider.gameObject;
                temporaryVector = new Vector3(changeTV.gameObject.transform.GetChild(0).position.x + hit.collider.GetComponentInParent<Cost>().positionOffset, changeTV.gameObject.transform.GetChild(0).position.y, changeTV.gameObject.transform.GetChild(0).position.z);
                temporaryObject = Instantiate(temporaryObject, temporaryVector, changeTV.gameObject.transform.GetChild(0).rotation, changeTV.transform);

                Destroy(changeTV.transform.GetChild(0).gameObject);

                for (int i = 0; i < storeOptions.transform.childCount; i++)
                {
                    storeType[i].SetActive(false);
                }

                menu.SwitchPanel(hubPanel);
            }
        }
    }

    public void ChangeCosts(GameObject currentObject)
    {
        //run this only once
        costs = new Text[costsParent.transform.childCount];

        for (int i = 0; i < costsParent.transform.childCount; i++)
        {
            costs[i] = costsParent.transform.GetChild(i).GetComponent<Text>();
        }

        for (int i = 0; i < currentObject.transform.childCount; i++)
        {
            string temp = currentObject.transform.GetChild(i).GetComponent<Cost>().cost.ToString();
            costs[i].text = temp;
        }
    }
}
