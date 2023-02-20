using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicHandControl : MonoBehaviour
{
    public GameObject sphereParent;
    public GameObject sphereParentTwin;

    public GameObject VRHand;
    public GameObject VRHandTwin;

    public GameObject VRHandPlaceHolder;
    public GameObject VRHandTwinPlaceHolder;

    public UnityClient unityClient;

    public BoxCollider robotRange;

    public PhysicalPropReference PPR;

    private GameObject sliderReference;
    private GameObject robotEndEffector;

    public GameObject portal1;
    public GameObject portal2;

    public GameObject portal1PlaceHolder;
    public GameObject portal2PlaceHolder;

    public Options controlMethod = new Options();
    public List<GameObject> portalRelevant = new List<GameObject>();

    public VRHandArmRender ArmRender;

    public Vector3 cameraToHandOffset = Vector3.zero;
    public GameObject Camera;
    public GameObject cameraParent;

    public ScatterSlicerBoxManagement SSBM;
    public GameObject dataPoint;

    private bool prev_gestureDetection = false;
    private bool current_gestureDetection = false;

    private GameObject closestDataPoint;
    private Vector3 prevCloesetVector = Vector3.zero;

    private void Start()
    {
        sliderReference = PPR.Touch_Point;
        robotEndEffector = PPR.TCP_Center;
    }

    // Update is called once per frame
    void Update()
    {
        // define the activated scatter slice layer and create data point in the sphere parent
        if (sphereParent.transform.childCount > 0)
        {
            foreach (Transform child in sphereParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        if (sphereParentTwin.transform.childCount > 0)
        {
            foreach (Transform child in sphereParentTwin.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        // create each data point in activate layer
        foreach (GameObject g in SSBM.selectedDataPoints)
        {
            GameObject newPoint1 = Instantiate(dataPoint, g.transform.position, Quaternion.identity);
            newPoint1.transform.SetParent(sphereParent.transform);
            newPoint1.transform.GetComponent<MeshRenderer>().enabled = false;

            GameObject newPoint2 = Instantiate(dataPoint, g.transform.position, Quaternion.identity);
            newPoint2.transform.SetParent(sphereParentTwin.transform);
            newPoint2.transform.GetComponent<MeshRenderer>().enabled = false;
        }

        // fetch the gesture detection flag from hand model
        current_gestureDetection = (VRHand.GetComponent<VRHandControl>().gestureDetection | VRHand.GetComponent<VRHandControl>().rotationGesture);

        switch (((int)controlMethod))
        {
            case 0: // magic hand control
                ArmRender.enable = false;

                setObjectActive(portalRelevant, false);
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;

                cameraParent.SetActive(true);
                Camera.transform.position = VRHandTwin.transform.position;

                if (current_gestureDetection == false & prev_gestureDetection == true)
                {
                    //(sphereTwin.transform.position, sphereTwin.transform.rotation) = getTargetPosRot(VRHandTwin.transform, VRHand.transform, sphere.transform);
                    (sphereParentTwin.transform.position, sphereParentTwin.transform.rotation) = getNewPosRotAfterRotation(VRHandTwinPlaceHolder.transform, VRHandPlaceHolder.transform, sphereParent.transform);
                }
                break;
            case 1: // extended hand control
                ArmRender.enable = true;

                setObjectActive(portalRelevant, false);
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;

                cameraParent.SetActive(true);
                Camera.transform.position = VRHandTwin.transform.position;

                if (current_gestureDetection == false & prev_gestureDetection == true)
                {
                    //(sphereTwin.transform.position, sphereTwin.transform.rotation) = getTargetPosRot(VRHandTwin.transform, VRHand.transform, sphere.transform);
                    (sphereParentTwin.transform.position, sphereParentTwin.transform.rotation) = getNewPosRotAfterRotation(VRHandTwinPlaceHolder.transform, VRHandPlaceHolder.transform, sphereParent.transform);
                }
                break;
            case 2: // portal control
                ArmRender.enable = false;

                setObjectActive(portalRelevant, true);

                cameraParent.SetActive(false);

                //(sphereTwin.transform.position, sphereTwin.transform.rotation) = getTargetPosRot(portal2.transform, portal1.transform, sphere.transform);
                (sphereParentTwin.transform.position, sphereParentTwin.transform.rotation) = getNewPosRotAfterRotation(portal2PlaceHolder.transform, portal1PlaceHolder.transform, sphereParent.transform);

                break;
            default: // magic hand control
                ArmRender.enable = false;

                setObjectActive(portalRelevant, false);
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;

                cameraParent.SetActive(true);
                Camera.transform.position = VRHandTwin.transform.position;

                if (current_gestureDetection == false & prev_gestureDetection == true)
                {
                    //(sphereTwin.transform.position, sphereTwin.transform.rotation) = getTargetPosRot(VRHandTwin.transform, VRHand.transform, sphere.transform);
                    (sphereParentTwin.transform.position, sphereParentTwin.transform.rotation) = getNewPosRotAfterRotation(VRHandTwinPlaceHolder.transform, VRHandPlaceHolder.transform, sphereParent.transform);
                }
                break;
        }

        // move the robot to the relative position if the data point is inside the reachable range
        if (sphereParentTwin.transform.childCount > 0)
        {
            // find the closest data point
            float d_Min = 1000;

            foreach (Transform t in sphereParentTwin.transform)
            {
                float d = Vector3.Distance(t.position, VRHand.transform.position);

                if (d < d_Min)
                {
                    d_Min = d;
                    closestDataPoint = t.gameObject;
                }
            }

            if (robotRange.bounds.Contains(closestDataPoint.transform.position) &
            Vector3.Distance(prevCloesetVector, closestDataPoint.transform.position) > 0.0001 &
            unityClient.startCalibration == false
            )
            {
                sliderReference.transform.position = closestDataPoint.transform.position;

                unityClient.initialPos();
                Vector3 newPos = unityClient.convertUnityCoord2RobotCoord(robotEndEffector.transform.position);

                unityClient.customMove(newPos.x, newPos.y, newPos.z, -0.6, 1.47, 0.62, movementType: 1);

                prevCloesetVector = closestDataPoint.transform.position;
            }
        }

        prev_gestureDetection = current_gestureDetection;
    }

    public static (Vector3 targetPos,Quaternion targetRot) getTargetPosRot(Transform T_from, Transform T_to, Transform source)
    {
        Vector3 targetPos;
        Quaternion targetRot;

        Matrix4x4 transformation = Matrix4x4.identity;

        transformation = T_from.worldToLocalMatrix * T_to.localToWorldMatrix;

        targetPos = T_to.TransformPoint(T_from.InverseTransformPoint(source.position));

        targetRot = source.rotation * transformation.rotation; // wrong orientation

        return (targetPos, targetRot);
    }

    public static (Vector3 targetPos, Quaternion targetRot) getNewPosRotAfterRotation(Transform T_from_placeholder, Transform T_to_placeholder, Transform source)
    {

        T_from_placeholder.transform.position = source.transform.position;
        T_from_placeholder.transform.rotation = source.transform.rotation;
        T_from_placeholder.transform.localScale = source.transform.localScale;

        T_to_placeholder.transform.localPosition = T_from_placeholder.transform.localPosition;
        T_to_placeholder.transform.localRotation = T_from_placeholder.transform.localRotation;
        T_to_placeholder.transform.localScale = T_from_placeholder.transform.localScale;

        return (T_to_placeholder.transform.position, T_to_placeholder.transform.rotation);
    }

    private void setObjectActive(List<GameObject> listObj, bool f)
    {
        foreach(GameObject g in listObj)
        {
            g.SetActive(f);
        }
    }

}

public enum Options // your custom enumeration
{
    MagicHand,
    ExtendedHand,
    Portal
};
