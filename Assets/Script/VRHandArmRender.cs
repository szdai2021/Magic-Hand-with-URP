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
                    cylinder1.transform.localScale = new Vector3(cylinder1.transform.localScale.x, cylinder1.transform.localScale.y, d1 / 2.75f - 0.03f);

                    cylinder2.transform.position = handJoint.transform.position;
                    cylinder2.transform.LookAt(middleJiont.transform, Vector3.left);

                    d2 = Vector3.Distance(middleJiont.transform.position, handJoint.transform.position);
                    cylinder2.transform.localScale = new Vector3(cylinder2.transform.localScale.x, cylinder2.transform.localScale.y, d2 / 2.75f - 0.03f);
                    break;
                case 1:
                    Vector3 elbowProjection = Vector3.Project(elbowVICON.transform.position - shoulderJoint.transform.position, (handJoint.transform.position - shoulderJoint.transform.position).normalized) + shoulderJoint.transform.position;
                    Vector3 middlePoint = (handJoint.transform.position + shoulderJoint.transform.position) / 2;

                    Vector3 offset = middlePoint - elbowProjection;
                    middleJiont.transform.position = elbowVICON.transform.position + offset;

                    cylinder1.transform.position = shoulderJoint.transform.position;
                    cylinder1.transform.LookAt(middleJiont.transform, Vector3.left);

                    d1 = Vector3.Distance(middleJiont.transform.position, shoulderJoint.transform.position);
                    cylinder1.transform.localScale = new Vector3(cylinder1.transform.localScale.x, cylinder1.transform.localScale.y, d1 / 2.75f - 0.03f);

                    cylinder2.transform.position = handJoint.transform.position;
                    cylinder2.transform.LookAt(middleJiont.transform, Vector3.left);

                    d2 = Vector3.Distance(middleJiont.transform.position, handJoint.transform.position);
                    cylinder2.transform.localScale = new Vector3(cylinder2.transform.localScale.x, cylinder2.transform.localScale.y, d2 / 2.75f - 0.03f);
                    break;
                case 2: // to do
                    Vector3 VRHandActualDirection = (VRHand.transform.position - shoulderJoint.transform.position).normalized;
                    Vector3 VRHandVirtualDirection = (VRHandTwin.transform.position - shoulderJoint.transform.position).normalized;

                    Quaternion desiredRotation = Quaternion.FromToRotation(VRHandActualDirection, VRHandVirtualDirection);
                    Quaternion rotationDifference = Quaternion.FromToRotation(VRHandActualDirection, VRHandVirtualDirection);
                    Quaternion inverseRotationDifference = Quaternion.Inverse(rotationDifference);
                    Quaternion finalRotation = inverseRotationDifference * desiredRotation;

                    Vector3 shoulderToElbowDirection = (elbowVICON.transform.position - shoulderJoint.transform.position).normalized;

                    float ratio = Vector3.Distance(VRHandTwin.transform.position, shoulderJoint.transform.position) / Vector3.Distance(VRHand.transform.position, shoulderJoint.transform.position);

                    middleJiont.transform.position = shoulderJoint.transform.position + (finalRotation * shoulderToElbowDirection)*ratio;

                    cylinder1.transform.position = shoulderJoint.transform.position;
                    cylinder1.transform.LookAt(middleJiont.transform, Vector3.left);

                    d1 = Vector3.Distance(middleJiont.transform.position, shoulderJoint.transform.position);
                    cylinder1.transform.localScale = new Vector3(cylinder1.transform.localScale.x, cylinder1.transform.localScale.y, d1 / 2.75f - 0.03f);

                    cylinder2.transform.position = handJoint.transform.position;
                    cylinder2.transform.LookAt(middleJiont.transform, Vector3.left);

                    d2 = Vector3.Distance(middleJiont.transform.position, handJoint.transform.position);
                    cylinder2.transform.localScale = new Vector3(cylinder2.transform.localScale.x, cylinder2.transform.localScale.y, d2 / 2.75f - 0.03f);
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

