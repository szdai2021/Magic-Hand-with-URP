using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class directionWheelControl : MonoBehaviour
{
    public GameObject posParent;
    public GameObject DataStreamObject;

    public bool hide = true;
    public bool posSyc = true;
    public bool autoAssignParent = false;
    public int assignIndex = 0;


    // Update is called once per frame
    void Update()
    {
        if (autoAssignParent)
        {
            posParent = DataStreamObject.transform.GetChild(assignIndex).gameObject;
        }

        if (posSyc)
        {
            this.gameObject.transform.position = posParent.transform.position;
        }

        Renderer[] objectR = this.GetComponentsInChildren<Renderer>();

        foreach (Renderer rr in objectR)
        {
            rr.enabled = !hide;
        }
    }
}
