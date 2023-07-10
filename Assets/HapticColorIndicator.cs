using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticColorIndicator : MonoBehaviour
{
    public GameObject VRHand;
    public GameObject VRHandTwin;
    public GameObject portal1PlaceHolder;
    public GameObject portal2PlaceHolder;
    public GameObject RobotEndeffectorCollider;

    public GameObject TempSliderReference;
    public GameObject TempEndEffector;
    public GameObject sphereParent;
    public GameObject sphereParentTwinTemp;

    public Material indicator;

    public bool enable = false;

    public bool InRangeFlag = false;

    // Update is called once per frame
    void Update()
    {
        if (enable)
        {
            (sphereParentTwinTemp.transform.position, sphereParentTwinTemp.transform.rotation) = MagicHandControl.getNewPosRotAfterRotation(portal2PlaceHolder.transform, portal1PlaceHolder.transform, sphereParent.transform);
        
            sphereParentTwinTemp.transform.GetChild(0).localPosition = sphereParent.transform.GetChild(0).localPosition;
            TempSliderReference.transform.position = sphereParentTwinTemp.transform.GetChild(0).position;

            if (RobotEndeffectorCollider.GetComponent<Collider>().bounds.Contains(TempEndEffector.transform.position))
            {
                indicator.color = Color.red;

                InRangeFlag = true;
            }
            else
            {
                indicator.color = Color.white;

                InRangeFlag = false;
            }

        }
    }
}

