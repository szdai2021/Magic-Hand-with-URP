using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRHandControlGoGo : MonoBehaviour
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

    public GameObject VRHandTwin;
    public GameObject handShadow;
    public bool turnOnHandShadow = true;

    public HandControl methodSwitch = new HandControl();
    private Vector3 prevHandPos = new Vector3(0, 0, 0);

    public PlaneUsageExample PUE;
    public GameObject VRHandHull;
    public Material VRHandMaterial;
    public GameObject HandHullClone;

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
    public bool resetPosFlag = false;
    public GameObject grappingDetectionArea;

    private float C_min = 11.448f;
    private float C_max = 21;

    private Vector3 redirectionOffset = Vector3.zero;

    public GameObject gogoPoint1, gogoPoint2;
    public GameObject gogoCenter;

    private void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        DirectionArrow_Parent = directionArrow.transform.parent.gameObject.GetComponent<directionWheelControl>().posParent;
        DWC2_Parent = DWC2.GetComponent<directionWheelControl>().posParent;
    }

    // Update is called once per frame
    void Update()
    {
        DWC2.transform.position = DWC1.transform.position + VRHandTwinPosOffset_Local + redirectionOffset;
        DWC2.transform.rotation = DWC1.transform.rotation * VRHandTwinRotOffset * TempRotationReference;

        if (((int)methodSwitch) == 3)
        {
            PortalCutOff.enable = true; // enable slicing plane on portals

            directionArrow.transform.parent.gameObject.GetComponent<directionWheelControl>().posParent = selectedPortal;
            DWC2.GetComponent<directionWheelControl>().posParent = selectedPortal;
            VRHandTwinPosOffset_Local = selectedPortal.transform.position - this.transform.position;

            gogoInteraction();

            if (selectedPortal != null)
            {
                if (gestureDetection) // position change
                {
                    selectedPortal.transform.position = VRHandTwinPosOffset_Local;
                }
                else if (rotationGesture) // rotation change
                {
                    Quaternion relativeRotation = this.transform.rotation * Quaternion.Inverse(RotationReference);
                    selectedPortal.transform.rotation = relativeRotation * selectedPortal.transform.rotation;
                }
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
                break;
            case 1:
                break;
            case 2:
                gogoInteraction();
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

    private void gogoInteraction()
    {
        if (gestureDetection)
        {
            float offset = Vector3.Distance(gogoCenter.transform.position, this.transform.position);
            float range = Vector3.Distance(gogoPoint1.transform.position, gogoPoint2.transform.position);

            Vector3 normal = Vector3.Normalize(this.transform.position - gogoCenter.transform.position);

            if (offset < range * 0.35)
            {
                VRHandTwinPosOffset_Local = gogoCenter.transform.position + offset * normal;
            }
            else if (offset >= range * 0.35)
            {
                double r = offset / range;
                float new_r = Mathf.Exp((float)(4.5 * r)) - 4.49f;

                VRHandTwinPosOffset_Local = gogoCenter.transform.position + (float)(range * 0.35) * normal + (offset - (float)(range * 0.35)) * normal * new_r;
            }
        }
    }

    public void resetConfig()
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
        redirectionOffset = Vector3.zero;

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
