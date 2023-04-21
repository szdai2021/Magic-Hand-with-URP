using System;
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

    public float gestureThreshold = 0;
    public int gestureDetectingDelay = 100;

    public directionWheelControl DWC1;
    public directionWheelControl DWC2;
    public directionSphereControl DSC;

    public GameObject directionArrow;
    public GameObject directionOrientation;
    public GameObject directionSphere;

    public GameObject DWC1PlaceHolder;
    public GameObject DWC2PlaceHolder;
    public List<GameObject> DW_List;
    public float DW_Threshold = 0;

    public GameObject VRHandTwin;
    public GameObject handShadow;
    public bool turnOnHandShadow = true;

    public HandControl methodSwitch = new HandControl();
    private Vector3 prevHandPos = new Vector3(0, 0, 0);

    public PlaneUsageExample PUE;
    public GameObject VRHandHull;
    public Material VRHandMaterial;
    public GameObject HandHullClone;

    public GameObject frontSensor;
    public GameObject backSensor;

    [HideInInspector] public bool gestureDetection = false;
    [HideInInspector] public bool rotationGesture = false;
    [HideInInspector] public Vector3 VRHandTwinPosOffset_Local = new Vector3(0, 0, 0);
    [HideInInspector] public Vector3 VRHandTwinPosOffset_Public = new Vector3(0, 0, 0);
    [HideInInspector] public Quaternion VRHandTwinRotOffset = Quaternion.identity;

    private Vector3 positionReference = Vector3.zero;
    private Quaternion RotationReference = Quaternion.identity;
    private Quaternion TempRotationReference = Quaternion.identity;
    private int gestureCounter = 0;
    private int rotationGestureCounter = 0;
    private bool prev_rotationGesture = false;
    private bool prev_DWC1_posSyc = true;

    private Ray ray;
    private RaycastHit hit;
    private int layerMask = 1 << 12; // only hit to layer 12
    private LineRenderer lineRenderer;
    private Vector3 directionVector;

    //private GameObject selectedPortal = null;
    public GameObject selectedPortal;

    private int prevScenario = -1;
    private GameObject DirectionArrow_Parent = null;
    private GameObject DWC2_Parent = null;

    public bool posDetectionLock = false;
    public bool rotDetectionLock = false;

    public bool fixedPosDetectionMode = false;
    public Collider targetAreaCollider;
    public GameObject grapTarget;
    public GameObject grapTargetOnHand;

    public UnityClient unity_client;
    public GameObject stopPlane;
    public bool resetPosFlag = false;
    public GameObject grappingDetectionArea;

    public Material arrowIndicatorSlow;
    public Material arrowIndicatorMiddle;
    public Material arrowIndicatorFast;

    private float C_min = 11.448f;
    private float C_max = 21;

    private void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        DirectionArrow_Parent = directionArrow.transform.parent.gameObject.GetComponent<directionWheelControl>().posParent;
        DWC2_Parent = DWC2.GetComponent<directionWheelControl>().posParent;
    }

    // Update is called once per frame
    void Update()
    {
        DWC2.transform.position = DWC1.transform.position + VRHandTwinPosOffset_Local;
        DWC2.transform.rotation = DWC1.transform.rotation * VRHandTwinRotOffset * TempRotationReference;

        if (((int)methodSwitch) == 3)
        {
            PortalCutOff.enable = true; // enable slicing plane on portals

            // disable portal selection on ray hit
            //if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            //{
            //    if (selectedPortal == null)
            //    {
            //        GameObject g = hit.transform.gameObject;

            //        selectedPortal = g;

            //        directionArrow.transform.parent.gameObject.GetComponent<directionWheelControl>().posParent = selectedPortal;
            //        DWC2.GetComponent<directionWheelControl>().posParent = selectedPortal;
            //        VRHandTwinPosOffset_Local = selectedPortal.transform.position - this.transform.position;
            //    }
            //}

            directionArrow.transform.parent.gameObject.GetComponent<directionWheelControl>().posParent = selectedPortal;
            DWC2.GetComponent<directionWheelControl>().posParent = selectedPortal;
            VRHandTwinPosOffset_Local = selectedPortal.transform.position - this.transform.position;

            //// raycast renderring
            //directionVector = frontSensor.transform.position - backSensor.transform.position;
            //ray = new Ray(backSensor.transform.position, directionVector.normalized);

            //if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            //{
            //    Material m = lineRenderer.material;

            //    if (rotationGesture | gestureDetection) // render for either translation or the first time in rotation gesture
            //    {
            //        lineRenderer.positionCount = 2;
            //        lineRenderer.SetPosition(0, ray.origin);
            //        lineRenderer.SetPosition(1, selectedPortal.transform.position);
            //    }

            //    if (gestureDetection)
            //    {
            //        m.color = Color.red;
            //    }
            //    else if (rotationGesture)
            //    {
            //        m.color = Color.blue;
            //    }
            //}
            //else
            //{
            //    lineRenderer.positionCount = 0;
            //}

            sphereDirecting();

            if (selectedPortal != null)
            {
                if (gestureDetection) // position change
                {
                    selectedPortal.transform.position = positionReference + VRHandTwinPosOffset_Local;
                }
                else if (rotationGesture) // rotation change
                {
                    Quaternion relativeRotation = this.transform.rotation * Quaternion.Inverse(RotationReference);
                    selectedPortal.transform.rotation = relativeRotation * selectedPortal.transform.rotation;
                }
                //else
                //{
                //    selectedPortal = null;
                //}
            }
            

            positionReference = this.transform.position;
            RotationReference = this.transform.rotation;
        }
        else
        {
            PortalCutOff.enable = false;
            lineRenderer.positionCount = 0;

            directionArrow.transform.parent.gameObject.GetComponent<directionWheelControl>().posParent = DirectionArrow_Parent;
            DWC2.GetComponent<directionWheelControl>().posParent = DWC2_Parent;


            (VRHandTwin.transform.position, VRHandTwin.transform.rotation) = MagicHandControl.getNewPosRotAfterRotation(DWC1PlaceHolder.transform, DWC2PlaceHolder.transform, this.transform);

            adjustTranslationOffsetByRotation();
        }

        checkTranslationGesture();
        checkRotationGesture();

        switch ((int)methodSwitch)
        {
            case 0:
                thumbDirecting();
                break;
            case 1:
                centerDirecting();
                break;
            case 2:
                if (prevScenario != 2)
                {
                    VRHandTwinPosOffset_Local = Vector3.zero;
                    VRHandTwinRotOffset = Quaternion.identity;
                    TempRotationReference = Quaternion.identity;
                }
                sphereDirecting();
                break;
            default:
                
                break;
        }

        if (prevScenario != (int)methodSwitch | resetPosFlag)
        {
            resetConfig();

            resetPosFlag = false;
        }

        //if ()
        //{

        //}

        if (turnOnHandShadow)
        {
            if (gestureDetection | rotationGesture)
            {
                handShadow.SetActive(false);
            }
            else
            {
                handShadow.SetActive(true);
            }
        }

        rotating();

        // stopping plane method - inactive
        //if (stopPlane.transform.GetChild(1).GetComponent<BoxCollider>().bounds.Contains(VRHandTwin.transform.position) & gestureDetection)
        //{
        //    stopPlane.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        //}
        //else
        //{
        //    stopPlane.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        //}

        prevHandPos = this.transform.position;
        prevScenario = (int)methodSwitch;
    }

    private void resetConfig()
    {
        positionReference = Vector3.zero;
        RotationReference = Quaternion.identity;
        TempRotationReference = Quaternion.identity;
        gestureCounter = 0;
        rotationGestureCounter = 0;
        prev_rotationGesture = false;
        prev_DWC1_posSyc = true;

        VRHandTwinPosOffset_Local = Vector3.zero;
        VRHandTwinRotOffset = Quaternion.identity;

        VRHandTwin.transform.position = this.transform.position;
        VRHandTwin.transform.rotation = this.transform.rotation;
        VRHandTwin.transform.localScale = this.transform.localScale;
    }

    private void checkTranslationGesture()
    {
        if (!posDetectionLock)
        {
            if (fixedPosDetectionMode)
            {
                targetAreaCollider.gameObject.SetActive(true);
            }
            else
            {
                targetAreaCollider.gameObject.SetActive(false);
            }

            if (!fixedPosDetectionMode | targetAreaCollider.bounds.Contains(this.transform.position) | gestureDetection)
            {
                if (Vector3.Distance(palmCenter.transform.position, index.transform.position) < gestureThreshold &
                        Vector3.Distance(palmCenter.transform.position, ring.transform.position) < gestureThreshold &
                        Vector3.Distance(palmCenter.transform.position, pinky.transform.position) < gestureThreshold &
                        Vector3.Distance(palmCenter.transform.position, middle.transform.position) < gestureThreshold)
                {
                    gestureCounter++;

                    if (gestureCounter == gestureDetectingDelay)
                    {
                        positionReference = this.transform.position;
                    }

                    if (gestureCounter > gestureDetectingDelay)
                    {
                        gestureDetection = true;

                        grapTarget.SetActive(false);
                        grapTargetOnHand.SetActive(true);

                        //if (fixedPosDetectionMode & !unity_client.homePosition)
                        //{
                        //    unity_client.initialPos();
                        //}
                    }
                }
                else
                {
                    gestureDetection = false;
                    grapTarget.SetActive(true);
                    grapTargetOnHand.SetActive(false);
                    DWC2.hide = true;
                    gestureCounter = 0;
                }
            }
        }
    }

    private void checkRotationGesture()
    {
        if (!rotDetectionLock)
        {
            if (Vector3.Distance(palmCenter.transform.position, index.transform.position) > gestureThreshold &
                        Vector3.Distance(palmCenter.transform.position, ring.transform.position) < gestureThreshold &
                        Vector3.Distance(palmCenter.transform.position, pinky.transform.position) < gestureThreshold &
                        Vector3.Distance(palmCenter.transform.position, middle.transform.position) > gestureThreshold)
            {
                rotationGestureCounter++;

                if (rotationGestureCounter == gestureDetectingDelay)
                {
                    RotationReference = this.transform.rotation;
                }

                if (rotationGestureCounter > gestureDetectingDelay)
                {
                    rotationGesture = true;

                    if ((int)methodSwitch != 3)
                    {
                        DWC2.hide = false;
                        DWC1.hide = false;
                    }
                }
            }
            else
            {
                DWC1.hide = true;
                DWC2.hide = true;
                rotationGesture = false;
                rotationGestureCounter = 0;
            }
        }
    }

    private void adjustTranslationOffsetByRotation()
    {
        if (rotationGestureCounter > 50)
        {
            if (prev_DWC1_posSyc == false)
            {
                VRHandTwinPosOffset_Local = DWC2.GetComponent<directionWheelControl>().posParent.transform.position - DWC1.GetComponent<directionWheelControl>().posParent.transform.position;

                Quaternion tempConjugate = new Quaternion(-TempRotationReference.x, -TempRotationReference.y, -TempRotationReference.z, TempRotationReference.w);

                Quaternion VRHandTwinRotOffset = Quaternion.Inverse(DWC1.GetComponent<directionWheelControl>().posParent.transform.rotation) * DWC2.GetComponent<directionWheelControl>().posParent.transform.rotation * tempConjugate;

                VRHandTwinRotOffset.Normalize();
            }
            DWC1.GetComponent<directionWheelControl>().posSyc = true;
        }
        else
        {
            DWC1.GetComponent<directionWheelControl>().posSyc = false;
        }

        prev_DWC1_posSyc = DWC1.GetComponent<directionWheelControl>().posSyc;
    }

    private void sphereDirecting()
    {
        if (gestureDetection)
        {
            grappingDetectionArea.SetActive(false);

            directionArrow.transform.parent.GetComponent<directionWheelControl>().hide = false;
            DSC.posSyc = false;

            // change the size of the direction arrow and the orientation
            float d = DSC.magnitude;
            directionArrow.transform.localScale = new Vector3(d / 12 * 3.5f, d / 12 * 3.5f, d / 12 * 7);
            directionOrientation.transform.localPosition = palmCenter.transform.position - directionSphere.transform.position;
            directionArrow.transform.LookAt(directionOrientation.transform);

            if (d <= C_min / 2)
            {
                changeAllChildrenMAterial(directionArrow.transform, arrowIndicatorSlow);
            }
            else if (d > C_min / 2 & d <= C_max)
            {
                changeAllChildrenMAterial(directionArrow.transform, arrowIndicatorMiddle);
            }
            else
            {
                changeAllChildrenMAterial(directionArrow.transform, arrowIndicatorFast);
            }

            Vector3 stepChange = directionOrientation.transform.localPosition.normalized / 100 * d / 15;

            //stepChange *= sigmoidCurveFactor(Vector3.Distance(VRHandTwin.transform.position, this.transform.position));

            stepChange *= (float)logisticFunction(d);

            // stopping plane method - inactive
            //if (!stopPlane.transform.GetChild(2).GetComponent<BoxCollider>().bounds.Contains(VRHandTwin.transform.position))
            //{
            //    VRHandTwinPosOffset_Local += stepChange;
            //}

            VRHandTwinPosOffset_Local += stepChange;
        }
        else
        {
            grappingDetectionArea.SetActive(true);

            directionArrow.transform.parent.GetComponent<directionWheelControl>().hide = true;
            DSC.posSyc = true;
        }
    }

    private void changeAllChildrenMAterial(Transform parent, Material m)
    {
        foreach (Transform t in parent)
        {
            t.gameObject.GetComponent<MeshRenderer>().material = m;
        }
    }

    private float sigmoidCurveFactor(float magnitude)
    {
        float d = 5f;
        float ratio;

        float f1 = magnitude;

        float exp1 = Mathf.Exp(magnitude/d);
        float exp2 = Mathf.Exp(-magnitude/d);

        float f2 = d*(exp1 - exp2) / (exp1 + exp2);

        ratio = f2 / f1;

        return ratio;
    }

    private double logisticFunction(float magnitude)
    {
        if (magnitude*2 <= C_min)
        {
            return 1;
        }
        else
        {
            double ratio;
            float a = 50f;
            float b = -0.2f;
            float c = 0.6f;
            float d = 14.8f;

            float y = (a - b) / (1 + Mathf.Exp(-c * (magnitude * 2 - d))) + b;

            ratio = 2 * y / ((magnitude * 2));

            return ratio;
        }
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
        if (rotationGesture)
        {
            TempRotationReference = Quaternion.Inverse(RotationReference) * this.transform.rotation;
        }
        else
        {
            if (prev_rotationGesture != rotationGesture)
            {
                VRHandTwinRotOffset *= TempRotationReference;
                TempRotationReference = Quaternion.identity;
            }
        }

        prev_rotationGesture = rotationGesture;
    }
}

public enum HandControl
{
    Thumb,
    Center,
    Sphere,
    Portal,
    None
} 
