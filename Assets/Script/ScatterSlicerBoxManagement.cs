using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterSlicerBoxManagement : MonoBehaviour
{
    public bool enable = false;

    public GameObject DW2_PlaceHolder;
    public VRHandControl VRHandControl;

    public GameObject middleTouchPoint;

    public int activatedLayerIndex = -1;

    [HideInInspector] public List<GameObject> selectedDataPoints = new List<GameObject>();
    public GameObject sphereParent;
    public GameObject slicingBoxParent;

    public VRHandControl VR_HandController;

    public GameObject p1;
    public GameObject p2;

    private bool prevGestureDetection;
    private bool prevRotationDetection;

    private int prevActivatedLayerIndex = -1;

    // Update is called once per frame
    void Update()
    {
        if ((prevGestureDetection & !VR_HandController.gestureDetection) | (prevRotationDetection & !VR_HandController.rotationGesture))
        {
            //this.transform.rotation = DW2_PlaceHolder.transform.rotation;

            Vector3 customNormal = p2.transform.position - p1.transform.position;
            this.transform.up = customNormal.normalized;
        }

        prevGestureDetection = VR_HandController.gestureDetection;
        prevRotationDetection = VR_HandController.rotationGesture;

        if (enable)
        {
            if (activatedLayerIndex > -1 & prevActivatedLayerIndex != activatedLayerIndex)
            {
                foreach (GameObject g in selectedDataPoints)
                {
                     g.gameObject.GetComponent<MeshRenderer>().enabled = true; // debug
                }

                selectedDataPoints = new List<GameObject>();

                foreach (Transform t in sphereParent.transform)
                {
                    if (this.transform.GetChild(activatedLayerIndex).GetComponent<Collider>().bounds.Contains(t.position))
                    {
                        selectedDataPoints.Add(t.gameObject);

                        t.gameObject.GetComponent<MeshRenderer>().enabled = false; // debug
                    }
                }
            }

            int index = -1;

            foreach (Transform t in slicingBoxParent.transform)
            {
                if (t.gameObject.GetComponent<Collider>().bounds.Contains(middleTouchPoint.transform.position))
                {
                    index = t.GetSiblingIndex();
                    break;
                }
            }

            if (index < 0 | index > 96)
            {
                activatedLayerIndex = -1;
            }
            else
            {
                activatedLayerIndex = index;

                slicingBoxParent.transform.GetChild(activatedLayerIndex).gameObject.GetComponent<MeshRenderer>().enabled = true;

                if (prevActivatedLayerIndex != activatedLayerIndex & prevActivatedLayerIndex > -1)
                {
                    slicingBoxParent.transform.GetChild(prevActivatedLayerIndex).gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
            }

            prevActivatedLayerIndex = activatedLayerIndex;
        }
        
    }
}

