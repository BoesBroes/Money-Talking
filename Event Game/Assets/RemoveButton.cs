using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveButton : MonoBehaviour
{
    public UnityEngine.UI.Button Button;


    public void onClickEvent()
    {
        //Hide button
        Button.gameObject.SetActive(false);
    }
}
