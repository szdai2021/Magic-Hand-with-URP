using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class directionWheelControl : MonoBehaviour
{
    public GameObject posParent;

    public bool hide = true;
    public bool posSyc = true;

    // Update is called once per frame
    void Update()
    {
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
