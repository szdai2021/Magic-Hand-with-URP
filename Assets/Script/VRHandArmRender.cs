using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHandArmRender : MonoBehaviour
{
    public GameObject VRHand;
    public GameObject VRHandTwin;

    public GameObject middleJiont;
    public GameObject handJoint;
    public GameObject shoulderJoint;

    public GameObject cylinder1; // shoulder to elbow
    public GameObject cylinder2; // hand to elbow

    public GameObject elbowVICON;

    public bool enable = false;

    public ArmRender methodSwitch = new ArmRender();

    private float d1, d2;

    public float offset = 0.09f;

    // Update is called once per frame
    void Update()
    {
        if (enable)
        {
            middleJiont.SetActive(true);
            cylinder1.SetActive(true);
            cylinder2.SetActive(true);

            switch ((int)methodSwitch)
            {
                case 0:
                    middleJiont.transform.position = elbowVICON.transform.position;

                    cylinder1.transform.position = shoulderJoint.transform.position;
                    cylinder1.transform.LookAt(middleJiont.transform, Vector3.left);

                    d1 = Vector3.Distance(middleJiont.transform.position, shoulderJoint.transform.position);
                    cylinder1.transform.localScale = new Vector3(cylinder1.transform.localScale.x, cylinder1.transform.localScale.y, (d1 - 0.09f) / 2.75f );

                    cylinder2.transform.position = handJoint.transform.position;
                    cylinder2.transform.LookAt(middleJiont.transform, Vector3.left);

                    d2 = Vector3.Distance(middleJiont.transform.position, handJoint.transform.position);
                    cylinder2.transform.localScale = new Vector3(cylinder2.transform.localScale.x, cylinder2.transform.localScale.y, (d2 - 0.09f) / 2.75f);
                    break;
                case 1:
                    Vector3 elbowProjection = Vector3.Project(elbowVICON.transform.position - shoulderJoint.transform.position, (handJoint.transform.position - shoulderJoint.transform.position).normalized) + shoulderJoint.transform.position;
                    Vector3 middlePoint = (handJoint.transform.position + shoulderJoint.transform.position) / 2;

                    Vector3 offset = middlePoint - elbowProjection;
                    middleJiont.transform.position = elbowVICON.transform.position + offset;

                    cylinder1.transform.position = shoulderJoint.transform.position;
                    cylinder1.transform.LookAt(middleJiont.transform, Vector3.left);

                    d1 = Vector3.Distance(middleJiont.transform.position, shoulderJoint.transform.position);
                    cylinder1.transform.localScale = new Vector3(cylinder1.transform.localScale.x, cylinder1.transform.localScale.y, (d1 - 0.09f) / 2.75f);

                    cylinder2.transform.position = handJoint.transform.position;
                    cylinder2.transform.LookAt(middleJiont.transform, Vector3.left);

                    d2 = Vector3.Distance(middleJiont.transform.position, handJoint.transform.position);
                    cylinder2.transform.localScale = new Vector3(cylinder2.transform.localScale.x, cylinder2.transform.localScale.y, (d2 - 0.09f) / 2.75f);
                    break;
                case 2: 
                    // Calculate the direction from pointC to pointA
                    Vector3 dirAC = handJoint.transform.position - shoulderJoint.transform.position;

                    // Calculate the distance from pointC to pointA
                    float distAC = dirAC.magnitude;

                    // Calculate the direction from pointC to pointB
                    float angle = Vector3.Angle(-shoulderJoint.transform.position + elbowVICON.transform.position, VRHand.transform.position - elbowVICON.transform.position);
                    Vector3 dirCB = elbowVICON.transform.position - shoulderJoint.transform.position;
                    dirCB = Quaternion.AngleAxis(angle, dirCB) * -dirCB; // to do

                    // Calculate the distance from pointC to pointB using the length ratios
                    float distAB = 0.5f * distAC;
                    float distCB = distAC - distAB * 1;

                    // Calculate the position of pointB based on the direction and distances
                    Vector3 posB = shoulderJoint.transform.position + -dirCB.normalized * distCB;

                    // Update the position of pointB
                    middleJiont.transform.position = posB;

                    cylinder1.transform.position = shoulderJoint.transform.position;
                    cylinder1.transform.LookAt(middleJiont.transform, Vector3.left);

                    d1 = Vector3.Distance(middleJiont.transform.position, shoulderJoint.transform.position);
                    cylinder1.transform.localScale = new Vector3(cylinder1.transform.localScale.x, cylinder1.transform.localScale.y, (d1 - 0.09f) / 2.75f);

                    cylinder2.transform.position = handJoint.transform.position;
                    cylinder2.transform.LookAt(middleJiont.transform, Vector3.left);

                    d2 = Vector3.Distance(middleJiont.transform.position, handJoint.transform.position);
                    cylinder2.transform.localScale = new Vector3(cylinder2.transform.localScale.x, cylinder2.transform.localScale.y, (d2 - 0.09f) / 2.75f);
                    break;
                default:
                    break;
            }
        }
        else
        {
            middleJiont.SetActive(false);
            cylinder1.SetActive(false);
            cylinder2.SetActive(false);
        }
    }

    public enum ArmRender
    {
        ForeArm,
        Center,
        Scale
    }

}

