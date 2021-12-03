using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cost : MonoBehaviour
{
    public int cost;
    [HideInInspector]
    public float positionOffset;

    void start()
    {
        positionOffset = this.transform.position.x;
    }
}
