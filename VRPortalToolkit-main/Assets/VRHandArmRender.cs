using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHandArmRender : MonoBehaviour
{
    public GameObject middleJiont;
    public GameObject handJoint;
    public GameObject shoulderJoint;

    public GameObject cylinder1; // shoulder to elbow
    public GameObject cylinder2; // hand to elbow

    public bool enable = false;

    // Update is called once per frame
    void Update()
    {
        if (enable)
        {
            middleJiont.SetActive(true);
            cylinder1.SetActive(true);
            cylinder2.SetActive(true);

            cylinder1.transform.position = shoulderJoint.transform.position;
            cylinder1.transform.LookAt(middleJiont.transform, Vector3.up);

            float d1 = Vector3.Distance(middleJiont.transform.position, shoulderJoint.transform.position);
            cylinder1.transform.localScale = new Vector3(cylinder1.transform.localScale.x, d1 - 0.01f, cylinder1.transform.localScale.z);

            cylinder2.transform.position = handJoint.transform.position;
            cylinder2.transform.LookAt(middleJiont.transform, Vector3.up);

            float d2 = Vector3.Distance(middleJiont.transform.position, handJoint.transform.position);
            cylinder2.transform.localScale = new Vector3(cylinder2.transform.localScale.x, d2 - 0.01f, cylinder2.transform.localScale.z);
        }
        else
        {
            middleJiont.SetActive(false);
            cylinder1.SetActive(false);
            cylinder2.SetActive(false);
        }
    }

}

