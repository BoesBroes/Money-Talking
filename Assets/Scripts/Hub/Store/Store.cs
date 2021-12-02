using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Store : MonoBehaviour
{
    [Header("Customization Objects")]
    public GameObject changeTV;
    //add more

    public GameObject storeOptions;
    private GameObject[] storeType;


    public void Start()
    {
        storeType = new GameObject[this.transform.childCount];

        for (int i = 0; i < this.transform.childCount; i++)
        {
            storeType[i] = this.transform.GetChild(i).gameObject;
            storeType[i].SetActive(false);
        }
    }

    public void ActivateCurrentStore(GameObject currentStore)
    {
        if(storeType.Contains(currentStore))
        {
            currentStore.SetActive(true);
        }
    }

    public void DeactivateStores()
    {
        for (int i = 0; i < storeType.Length; i++)
        {
            storeType[i].SetActive(false);
        }
    }
}
