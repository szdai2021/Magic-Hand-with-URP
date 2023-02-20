using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class directionSphereControl : MonoBehaviour
{
    public GameObject posParent;
    public GameObject arrow;

    public bool hide = true;
    public bool posSyc = true;

    public float magnitude;

    // Update is called once per frame
    void Update()
    {
        if (posSyc)
        {
            this.gameObject.transform.position = posParent.transform.position;
        }
        else
        {
            float d = Vector3.Distance(posParent.transform.position, this.transform.position)*100;

            if (d/2 > 12)
            {
                d = 30;
            }

            arrow.transform.localScale = new Vector3(this.transform.localScale.x, this.transform.localScale.y, d/2);
            arrow.transform.LookAt(posParent.transform);

            magnitude = d / 2;
        }

        hide = posSyc;

        Renderer[] objectR = this.GetComponentsInChildren<Renderer>();

        foreach (Renderer rr in objectR)
        {
            rr.enabled = !hide;
        }
    }
}
