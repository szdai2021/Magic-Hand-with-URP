using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHandControl : MonoBehaviour
{
    public GameObject palmCenter;

    public GameObject index;
    public GameObject ring;
    public GameObject pinky;
    public GameObject middle;
    public GameObject thumb;

    public GameObject rotationMarker;
    public BoxCollider leftBox;
    public BoxCollider rightBox;

    public float gestureThreshold = 0;

    public bool gestureDetection = false;

    public directionWheelControl DWC1;
    public directionWheelControl DWC2;

    public List<GameObject> DW_List;

    public float DW_Threshold = 0;

    public GameObject VRHandTwin;
    public GameObject handShadow;
    public Vector3 VRHandTwinPosOffset_Local = new Vector3(0,0,0);
    public Vector3 VRHandTwinPosOffset_Public = new Vector3(0, 0, 0);
    public Quaternion VRHandTwinRotOffset = Quaternion.identity;
    public bool turnOnHandShadow = true;

    public GameObject portal1;
    public GameObject portal2;

    public HandControl methodSwitch = new HandControl();

    //public int methodSwitch = 2;

    private Vector3 prevHandPos = new Vector3(0, 0, 0);

    // Update is called once per frame
    void Update()
    {
        /*
        VRHandTwin.transform.localPosition = this.gameObject.transform.localPosition + VRHandTwinPosOffset_Local;

        if (VRHandTwinPosOffset_Local.sqrMagnitude != 0)
        {
            print(VRHandTwin.transform.localPosition);
            print(VRHandTwin.transform.position);
            VRHandTwinPosOffset_Public = VRHandTwin.transform.position - this.gameObject.transform.position;
            VRHandTwin.transform.localPosition = new Vector3(0, 0, 0);
            VRHandTwinPosOffset_Local = new Vector3(0, 0, 0);
            VRHandTwin.transform.parent.position = this.gameObject.transform.position + VRHandTwinPosOffset_Public;
        }
        
        VRHandTwin.transform.parent.rotation = this.gameObject.transform.rotation * VRHandTwinRotOffset;
        */

        DWC2.transform.position = DWC1.transform.position + VRHandTwinPosOffset_Local;
        DWC2.transform.rotation = DWC1.transform.rotation * VRHandTwinRotOffset;

        Vector3 currentHandPos = this.gameObject.transform.position;

        if (((int)methodSwitch) != 2)
        {
            (VRHandTwin.transform.position, VRHandTwin.transform.rotation) = MagicHandControl.getTargetPosRot(DWC1.transform, DWC2.transform, this.transform);
        }
        else
        {
            (VRHandTwin.transform.position, VRHandTwin.transform.rotation) = MagicHandControl.getTargetPosRot(portal1.transform, portal2.transform, this.transform);
        }


        if (Vector3.Distance(palmCenter.transform.position, index.transform.position) < gestureThreshold &
            Vector3.Distance(palmCenter.transform.position, ring.transform.position) < gestureThreshold &
            Vector3.Distance(palmCenter.transform.position, pinky.transform.position) < gestureThreshold &
            Vector3.Distance(palmCenter.transform.position, middle.transform.position) < gestureThreshold &
            ((int)methodSwitch) != 2)
        {
            gestureDetection = true;
            DWC2.hide = false;
        }
        else
        {
            gestureDetection = false;
            DWC2.hide = true;
        }

        switch (((int)methodSwitch))
        {
            case 0:
                thumbDirecting();
                break;
            case 1:
                centerDirecting();
                break;
            case 2:
                break;
            default:
                centerDirecting();
                break;
        }

        if (turnOnHandShadow)
        {
            if (gestureDetection)
            {
                handShadow.SetActive(false);
            }
            else
            {
                handShadow.SetActive(true);
            }
        }

        //rotating();

        prevHandPos = this.transform.position;
    }

    private void centerDirecting()
    {
        if (gestureDetection)
        {
            DWC1.posSyc = false;

            //find the closest direction
            float minDistance = 100;
            GameObject closestDirection = null;
            foreach (GameObject g in DW_List)
            {
                if (Vector3.Distance(palmCenter.transform.position, g.transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(palmCenter.transform.position, g.transform.position);
                    closestDirection = g;
                }
            }

            if (minDistance < DW_Threshold & closestDirection != null)
            {
                //change highlight color
                closestDirection.GetComponent<DW_IndivisualControl>().highlightFlag = true;

                //move vr hand
                VRHandTwinPosOffset_Local += closestDirection.GetComponent<DW_IndivisualControl>().offset;
            }
            else
            {
                foreach (GameObject g in DW_List)
                {
                    //change highlight color
                    g.GetComponent<DW_IndivisualControl>().highlightFlag = false;
                }
            }
        }
        else
        {
            DWC1.posSyc = true;
        }
    }

    private void thumbDirecting()
    {
        if (gestureDetection)
        {
            //find the closest direction
            float minDistance = 100;
            GameObject closestDirection = null;
            foreach (GameObject g in DW_List)
            {
                if (Vector3.Distance(thumb.transform.position, g.transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(thumb.transform.position, g.transform.position);
                    closestDirection = g;
                }
            }

            if (minDistance < DW_Threshold & closestDirection != null)
            {
                //change highlight color
                closestDirection.GetComponent<DW_IndivisualControl>().highlightFlag = true;

                //move vr hand
                VRHandTwinPosOffset_Local += closestDirection.GetComponent<DW_IndivisualControl>().offset;
            }
            else
            {
                foreach (GameObject g in DW_List)
                {
                    //change highlight color
                    g.GetComponent<DW_IndivisualControl>().highlightFlag = false;
                }
            }
        }
    }

    private void rotating()
    {
        if (gestureDetection)
        {
            if (leftBox.bounds.Contains(rotationMarker.transform.position))
            {
                VRHandTwinRotOffset *= Quaternion.AngleAxis(0.1f, DW_List[0].transform.up);
            }

            if (rightBox.bounds.Contains(rotationMarker.transform.position))
            {
                VRHandTwinRotOffset *= Quaternion.AngleAxis(-0.1f, DW_List[0].transform.up);
            }
        }
    }
}

public enum HandControl
{
    Thumb,
    Center,
    Portal
} 
