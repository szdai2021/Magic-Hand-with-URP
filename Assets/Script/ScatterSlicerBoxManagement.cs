using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterSlicerBoxManagement : MonoBehaviour
{
    public bool enable = false;

    public GameObject DW2_PlaceHolder;
    public VRHandControl VRHandControl;

    public GameObject frontSensor;
    public GameObject backSensor;

    public int activatedLayerIndex = -1;

    [HideInInspector] public List<GameObject> selectedDataPoints = new List<GameObject>();
    public GameObject sphereParent;

    public VRHandControl VR_HandController;
    private bool prevGestureDetection;
    private bool prevRotationDetection;

    private int prevActivatedLayerIndex = -1;

    // Update is called once per frame
    void Update()
    {
        if ((prevGestureDetection & !VR_HandController.gestureDetection) | (prevRotationDetection & !VR_HandController.rotationGesture))
        {
            this.transform.rotation = DW2_PlaceHolder.transform.rotation;
        }

        prevGestureDetection = VR_HandController.gestureDetection;
        prevRotationDetection = VR_HandController.rotationGesture;

        if (enable)
        {
            if (!(VRHandControl.gestureDetection & VRHandControl.rotationGesture)) // start to check the data points in layer
            {
                if (activatedLayerIndex > -1)
                {
                    foreach (Transform t in sphereParent.transform)
                    {
                        if (this.transform.GetChild(activatedLayerIndex).GetComponent<Collider>().bounds.Contains(t.position))
                        {
                            selectedDataPoints.Add(t.gameObject);
                        }
                    }
                }
            }
            else // define the activated layer
            {
                int frontIndex = -1, backIndex = -1;

                foreach (Transform t in sphereParent.transform)
                {
                    if (t.gameObject.GetComponent<Collider>().bounds.Contains(frontSensor.transform.position))
                    {
                        frontIndex = t.GetSiblingIndex();
                        break;
                    }
                }

                foreach (Transform t in sphereParent.transform)
                {
                    if (t.gameObject.GetComponent<Collider>().bounds.Contains(backSensor.transform.position))
                    {
                        backIndex = t.GetSiblingIndex();
                        break;
                    }
                }

                if (frontIndex < 1)
                {
                    activatedLayerIndex = -1;
                }
                else if (backIndex > 31)
                {
                    activatedLayerIndex = -1;
                }
                else
                {
                    activatedLayerIndex = (frontIndex + backIndex) / 2;

                    if (prevActivatedLayerIndex != activatedLayerIndex)
                    {
                        sphereParent.transform.GetChild(prevActivatedLayerIndex).gameObject.GetComponent<MeshRenderer>().enabled = false;
                    }
                    else
                    {
                        sphereParent.transform.GetChild(activatedLayerIndex).gameObject.GetComponent<MeshRenderer>().enabled = true;
                    }
                }

                prevActivatedLayerIndex = activatedLayerIndex;
            }
        }
    }
}

