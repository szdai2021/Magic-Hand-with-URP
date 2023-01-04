using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicHandControl : MonoBehaviour
{
    public GameObject sphere;
    public GameObject sphereTwin;

    public GameObject VRHand;
    public GameObject VRHandTwin;

    public GameObject VRHandPlaceHolder;
    public GameObject VRHandTwinPlaceHolder;

    public UnityClient unityClient;

    public BoxCollider robotRange;

    public GameObject sliderReference;
    public GameObject robotEndEffector;

    public GameObject portal1;
    public GameObject portal2;

    public GameObject portal1PlaceHolder;
    public GameObject portal2PlaceHolder;

    public Options controlMethod = new Options();
    public List<GameObject> portalRelevant = new List<GameObject>();

    public VRHandArmRender ArmRender;

    private bool prev_gestureDetection = false;
    private bool current_gestureDetection = false;

    private Vector3 prev_sphereTwin = new Vector3(0,0,0);

    // Update is called once per frame
    void Update()
    {
        current_gestureDetection = VRHand.GetComponent<VRHandControl>().gestureDetection;

        switch (((int)controlMethod))
        {
            case 0: // magic hand control
                ArmRender.enable = false;

                setObjectActive(portalRelevant, false);
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;

                if (current_gestureDetection == false & prev_gestureDetection == true)
                {
                    //(sphereTwin.transform.position, sphereTwin.transform.rotation) = getTargetPosRot(VRHandTwin.transform, VRHand.transform, sphere.transform);
                    (sphereTwin.transform.position, sphereTwin.transform.rotation) = getNewPosRotAfterRotation(VRHandTwinPlaceHolder.transform, VRHandPlaceHolder.transform, sphere.transform);
                }
                break;
            case 1: // extended hand control
                ArmRender.enable = true;

                setObjectActive(portalRelevant, false);
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;

                if (current_gestureDetection == false & prev_gestureDetection == true)
                {
                    //(sphereTwin.transform.position, sphereTwin.transform.rotation) = getTargetPosRot(VRHandTwin.transform, VRHand.transform, sphere.transform);
                    (sphereTwin.transform.position, sphereTwin.transform.rotation) = getNewPosRotAfterRotation(VRHandTwinPlaceHolder.transform, VRHandPlaceHolder.transform, sphere.transform);
                }
                break;
            case 2: // portal control
                ArmRender.enable = false;

                setObjectActive(portalRelevant, true);
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = false;

                //(sphereTwin.transform.position, sphereTwin.transform.rotation) = getTargetPosRot(portal2.transform, portal1.transform, sphere.transform);
                (sphereTwin.transform.position, sphereTwin.transform.rotation) = getNewPosRotAfterRotation(portal2PlaceHolder.transform, portal1PlaceHolder.transform, sphere.transform);

                break;
            default: // magic hand control
                ArmRender.enable = false;

                setObjectActive(portalRelevant, false);
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;

                if (current_gestureDetection == false & prev_gestureDetection == true)
                {
                    //(sphereTwin.transform.position, sphereTwin.transform.rotation) = getTargetPosRot(VRHandTwin.transform, VRHand.transform, sphere.transform);
                    (sphereTwin.transform.position, sphereTwin.transform.rotation) = getNewPosRotAfterRotation(VRHandTwinPlaceHolder.transform, VRHandPlaceHolder.transform, sphere.transform);
                }
                break;
        }

        if (robotRange.bounds.Contains(sphereTwin.transform.position) & 
            Vector3.Distance(prev_sphereTwin, sphereTwin.transform.position)>0.0001 &
            unityClient.startCalibration == false
            )
        {
            sliderReference.transform.position = sphereTwin.transform.position;

            unityClient.initialPos();
            Vector3 newPos = unityClient.convertUnityCoord2RobotCoord(robotEndEffector.transform.position);

            unityClient.customMove(newPos.x, newPos.y, newPos.z, -0.6, 1.47, 0.62, movementType: 1);

            prev_sphereTwin = sphereTwin.transform.position;
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
