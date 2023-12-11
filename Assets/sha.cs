using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manus.Hermes.Glove;
using System;
using Manus.Hand;

public class sha : MonoBehaviour
{
    public GameObject posParent;
    public GameObject DataStreamObject;

    public bool posSyc = true;
    public bool autoAssignParent = false;
    public int assignIndex = 0;
    public bool isHand = false;
    public Vector3 rotationOffset;
    public Vector3 positionOffset;
    public Vector3 reverseDirectionA;
    public Vector3 reverseDirectionB;
    public Vector3 reverseDirectionC;
    public bool positionFlag;
    public bool rotationFlag;


    public Quaternion wristRotation;
    public Dictionary<int, Tuple<Vector3, Quaternion>> rawData = new Dictionary<int, Tuple<Vector3, Quaternion>>();
    public Tuple<Vector3, Quaternion> thumb;
    public Tuple<Vector3, Quaternion> index;
    public Tuple<Vector3, Quaternion> middle;
    public Tuple<Vector3, Quaternion> ring;
    public Tuple<Vector3, Quaternion> pinky;

    public Quaternion startRot;
    public bool getStartRot = false;

    public int i;

    // Update is called once per frame
    void Update()
    {
        if (getStartRot)
        {
            startRot = posParent.transform.rotation;
            getStartRot = false;
        }

        if (autoAssignParent)
        {
            posParent = DataStreamObject.transform.GetChild(assignIndex).gameObject;
        }

        if (posSyc & !isHand)
        {
            this.gameObject.transform.position = posParent.transform.position;
            this.gameObject.transform.rotation = posParent.transform.rotation;
        }

        if (posSyc & isHand)
        {
            if (positionFlag)
            {
                this.gameObject.transform.position = posParent.transform.position + positionOffset;
            }


            if (rotationFlag)
            {
                //this.gameObject.transform.localRotation = Quaternion.Euler(rotationOffset) * ReverseQuaternionInOneDirection(ReverseQuaternionInOneDirection(ReverseQuaternionInOneDirection(Data.m_WristRotation, reverseDirectionA), reverseDirectionB), reverseDirectionC);
                //this.gameObject.transform.rotation = Quaternion.Euler(rotationOffset) * ReverseQuaternionInOneDirection(ReverseQuaternionInOneDirection(ReverseQuaternionInOneDirection(posParent.transform.rotation, reverseDirectionA), reverseDirectionB), reverseDirectionC);
                this.gameObject.transform.rotation = Quaternion.Euler(posParent.transform.rotation.eulerAngles.x - startRot.eulerAngles.x, 0, 0);
            }
            //print(Data.m_WristRotation);

            wristRotation = Data.m_WristRotation;
        }

    }

    public Quaternion ReverseQuaternionInOneDirection(Quaternion originalQuaternion, Vector3 reverseDirection)
    {
        // Calculate the angle between the original quaternion's forward direction and the reverse direction
        float angle = Vector3.Angle(originalQuaternion * Vector3.forward, reverseDirection);

        // Create a rotation quaternion around the reverse direction
        Quaternion reverseRotation = Quaternion.AngleAxis(180f - angle, reverseDirection);

        // Apply the reverse rotation to the original quaternion
        Quaternion reversedQuaternion = reverseRotation * originalQuaternion;

        return reversedQuaternion;
    }
}
