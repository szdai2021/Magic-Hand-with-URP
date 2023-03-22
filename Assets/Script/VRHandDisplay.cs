using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHandDisplay : MonoBehaviour
{
    public bool hideVRHand = false;

    // Update is called once per frame
    void Update()
    {
        Renderer[] objectR = this.GetComponentsInChildren<Renderer>();

        foreach (Renderer rr in objectR)
        {
            if (rr.gameObject.tag == "SkipHideScript")
            {

            }
            else
            {
                rr.enabled = !hideVRHand;
            }
        }
    }

}
