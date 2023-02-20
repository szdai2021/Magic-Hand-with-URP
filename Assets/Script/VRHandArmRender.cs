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

    public GameObject elbowVICON;

    public bool enable = false;

    // Update is called once per frame
    void Update()
    {
        if (enable)
        {
            middleJiont.SetActive(true);
            cylinder1.SetActive(true);
            cylinder2.SetActive(true);

            Vector3 elbowProjection = Vector3.Project(elbowVICON.transform.position - shoulderJoint.transform.position, (handJoint.transform.position - shoulderJoint.transform.position).normalized) + shoulderJoint.transform.position;
            Vector3 middlePoint = (handJoint.transform.position + shoulderJoint.transform.position) / 2;

            Vector3 offset = middlePoint - elbowProjection;
            middleJiont.transform.position = elbowVICON.transform.position + offset;

            cylinder1.transform.position = shoulderJoint.transform.position;
            cylinder1.transform.LookAt(middleJiont.transform, Vector3.left);

            float d1 = Vector3.Distance(middleJiont.transform.position, shoulderJoint.transform.position);
            cylinder1.transform.localScale = new Vector3(cylinder1.transform.localScale.x, cylinder1.transform.localScale.y, d1/2 - 0.03f);

            cylinder2.transform.position = handJoint.transform.position;
            cylinder2.transform.LookAt(middleJiont.transform, Vector3.left);

            float d2 = Vector3.Distance(middleJiont.transform.position, handJoint.transform.position);
            cylinder2.transform.localScale = new Vector3(cylinder2.transform.localScale.x, cylinder2.transform.localScale.y, d2/2 - 0.03f);
        }
        else
        {
            middleJiont.SetActive(false);
            cylinder1.SetActive(false);
            cylinder2.SetActive(false);
        }
    }

}

