using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public ViconMixedRealityCalibration VMRC;
    public UnityClient UC;

    public float gripperDiameter = 40f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            VMRC.ApplyOffset();
        }

        if (Input.GetKeyDown("u"))
        {
            UC.changeGripperDiameter(gripperDiameter);
        }
    }
}
