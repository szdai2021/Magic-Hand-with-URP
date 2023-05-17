using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCutOff : MonoBehaviour
{
    public static bool enable = false;

    public Collider checkBox;
    public GameObject VRHand;

    public GameObject portal1;
    public GameObject portal2;

    public GameObject portal1PlaceHolder;
    public GameObject portal2PlaceHolder;

    public GameObject HandHullClone;
    public GameObject VRHandTwin;

    public PlaneUsageExample PUE;

    public GameObject VRHandHull;
    public Material FullyTransparent;
    public Material VRHandMaterial;
    public Collider throughPortalCheckBox;

    private GameObject upperHandHull;
    private GameObject lowerHandHull;

    private GameObject upperHandHullClone;

    public GameObject VRArm;
    public GameObject VRArmClone; // whole arm render

    public GameObject ArmHullClone;
    public GameObject VRArmHull;

    private GameObject upperArmHull;
    private GameObject lowerArmHull;

    private GameObject upperArmHullClone;

    public float resizeScale = 1;

    private bool throughPortal = false;

    // Update is called once per frame
    void Update()
    {
        if (enable)
        {
            if (VRHand.GetComponent<VRHandControlGoGo>().gestureDetection)
            {
                cleanUnusedHull();

                VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;
                VRHandTwin.GetComponent<VRHandDisplay>().hideVRHand = false;

                VRArm.GetComponent<VRHandDisplay>().hideVRHand = false;
                VRArmClone.GetComponent<VRHandDisplay>().hideVRHand = true;
            }
            else
            {
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = false;
                VRHandTwin.GetComponent<VRHandDisplay>().hideVRHand = true;

                VRArm.GetComponent<VRHandDisplay>().hideVRHand = true;
                VRArmClone.GetComponent<VRHandDisplay>().hideVRHand = false;
            }

            if (checkBox.bounds.Contains(VRHand.transform.position))
            {
                // hand portal cut off
                (VRHandTwin.transform.position, VRHandTwin.transform.rotation) = MagicHandControl.getNewPosRotAfterRotation(portal1PlaceHolder.transform, portal2PlaceHolder.transform, VRHand.transform);
                (HandHullClone.transform.position, HandHullClone.transform.rotation) = MagicHandControl.getNewPosRotAfterRotation(portal1PlaceHolder.transform, portal2PlaceHolder.transform, VRHandHull.transform);

                HandHullClone.transform.localScale = new Vector3(resizeScale, resizeScale, resizeScale);

                if (throughPortal)
                {
                    VRHandTwin.transform.localScale = new Vector3(resizeScale, resizeScale, resizeScale);
                }
                else
                {
                    VRHandTwin.transform.localScale = Vector3.one;
                }

                // arm portal cut off
                (VRArmClone.transform.position, VRArmClone.transform.rotation) = MagicHandControl.getNewPosRotAfterRotation(portal1PlaceHolder.transform, portal2PlaceHolder.transform, VRArm.transform);
                (ArmHullClone.transform.position, ArmHullClone.transform.rotation) = MagicHandControl.getNewPosRotAfterRotation(portal1PlaceHolder.transform, portal2PlaceHolder.transform, VRArmHull.transform);

                portalHandPosControl();
            }
            else
            {
                //VRArm.GetComponent<VRHandDisplay>().hideVRHand = false;
                //VRArmClone.GetComponent<VRHandDisplay>().hideVRHand = true;
            }
        }
    }


    private void portalHandPosControl()
    {
        cleanUnusedHull();

        GameObject[] handHulls = PUE.sliceObject(VRHandHull);

        if (handHulls == null) // no intersaction between the hand and the cutting plane
        {
            if (throughPortalCheckBox.bounds.Contains(VRHand.transform.position)) // hand passed through portal completely
            {
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;
                VRHandTwin.GetComponent<VRHandDisplay>().hideVRHand = false;

                throughPortal = true;
            }
            else
            {
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = false;
                VRHandTwin.GetComponent<VRHandDisplay>().hideVRHand = true;

                throughPortal = false;
            }

        }
        else // Upper handhull and lower handhull generated
        {
            VRHandTwin.GetComponent<VRHandDisplay>().hideVRHand = true;
            VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;

            upperHandHull = handHulls[0];
            lowerHandHull = handHulls[1];

            upperHandHull.transform.SetParent(VRHandHull.transform);
            lowerHandHull.transform.SetParent(VRHandHull.transform);

            upperHandHullClone = Instantiate(upperHandHull);
            upperHandHullClone.transform.SetParent(HandHullClone.transform);
            upperHandHullClone.transform.localPosition = upperHandHull.transform.localPosition;
            upperHandHullClone.transform.localRotation = upperHandHull.transform.localRotation;
            upperHandHullClone.transform.localScale = new Vector3(1, 1, 1);

            lowerHandHull.GetComponent<Renderer>().material = VRHandMaterial;
            upperHandHullClone.GetComponent<Renderer>().material = VRHandMaterial;
        }

        // arm
        GameObject[] armHulls = PUE.sliceObject(VRArmHull);

        if (armHulls == null) // no intersaction between the hand and the cutting plane
        {
            VRArm.GetComponent<VRHandDisplay>().hideVRHand = false;
            VRArmClone.GetComponent<VRHandDisplay>().hideVRHand = true;
        }
        else // Upper handhull and lower handhull generated
        {
            VRArmClone.GetComponent<VRHandDisplay>().hideVRHand = true;
            VRArm.GetComponent<VRHandDisplay>().hideVRHand = true;

            upperArmHull = armHulls[0];
            lowerArmHull = armHulls[1];

            upperArmHull.transform.SetParent(VRArmHull.transform);
            lowerArmHull.transform.SetParent(VRArmHull.transform);

            upperArmHullClone = Instantiate(upperArmHull);
            upperArmHullClone.transform.SetParent(ArmHullClone.transform);
            upperArmHullClone.transform.localPosition = upperArmHull.transform.localPosition;
            upperArmHullClone.transform.localRotation = upperArmHull.transform.localRotation;
            upperArmHullClone.transform.localScale = new Vector3(1, 1, 1);

            lowerArmHull.GetComponent<Renderer>().material = VRHandMaterial;
            upperArmHullClone.GetComponent<Renderer>().material = VRHandMaterial;
        }
    }

    private void cleanUnusedHull()
    {
        if (upperHandHull != null | lowerHandHull != null | upperHandHullClone != null)
        {
            Object.Destroy(upperHandHull);
            Object.Destroy(lowerHandHull);
            Object.Destroy(upperHandHullClone);
        }

        if (upperArmHull != null | lowerArmHull != null | upperArmHullClone != null)
        {
            Object.Destroy(upperArmHull);
            Object.Destroy(lowerArmHull);
            Object.Destroy(upperArmHullClone);
        }
    }
}

