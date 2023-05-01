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

    public float resizeScale = 1;

    private bool throughPortal = false;

    // Update is called once per frame
    void Update()
    {
        if (enable)
        {
            if (checkBox.bounds.Contains(VRHand.transform.position))
            {
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

                portalHandPosControl();

                //GameObject.FindGameObjectsWithTag("Ray")[0].GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = false;
                VRHandTwin.GetComponent<VRHandDisplay>().hideVRHand = true;

                //GameObject.FindGameObjectsWithTag("Ray")[0].GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }


    private void portalHandPosControl()
    {
        if (upperHandHull != null | lowerHandHull != null | upperHandHullClone != null)
        {
            Object.Destroy(upperHandHull);
            Object.Destroy(lowerHandHull);
            Object.Destroy(upperHandHullClone);
        }

        GameObject[] handHulls = PUE.sliceObject();

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

            //upperHandHull.SetActive(false);
            lowerHandHull.GetComponent<Renderer>().material = VRHandMaterial;
            upperHandHullClone.GetComponent<Renderer>().material = VRHandMaterial;
        }
    }
}

