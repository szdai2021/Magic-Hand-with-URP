using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

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

    public VRHandControl VR_Hand_Control;

    public GameObject scatterParent;
    public GameObject startPoint;
    public GameObject finishText;
    public Material outline;
    public GameObject debugText;

    public bool experimentStart = false;
    public int P_Number = -1;
    public int Scenario_No = -1; // 1 - magic hand; 2 - connected hand; 3 - portals
    public int experimentStage = -1;

    private int prevExperimentStage = -1;
    private bool prevExperimentStart = false;
    private List<int> trainingOrder = new List<int>();
    private List<int> orderList = new List<int>();
    private List<int> orderInUse = new List<int>();
    private int currentOrderIndex = -1;
    private List<float> timeList = new List<float>();
    private float timeStamp;
    private GameObject currentDataPoint;
    private int duplicateFileIndex = 0;

    static public bool startPointTouched = false;
    static public bool dataPointTouched = false;
    static public int dataPointIndex;

    private void Start()
    {
        sliderReference = PPR.Touch_Point;
        robotEndEffector = PPR.TCP_Center;

        trainingOrder.Add(11);
        trainingOrder.Add(57);
        trainingOrder.Add(100);
        trainingOrder.Add(239);
        trainingOrder.Add(427);
        trainingOrder.Add(311);
        trainingOrder.Add(86);
    }

    // Update is called once per frame
    void Update()
    {
        //// define the activated scatter slice layer and create data point in the sphere parent
        //if (sphereParent.transform.childCount > 0)
        //{
        //    foreach (Transform child in sphereParent.transform)
        //    {
        //        GameObject.Destroy(child.gameObject);
        //    }
        //}

        //if (sphereParentTwin.transform.childCount > 0)
        //{
        //    foreach (Transform child in sphereParentTwin.transform)
        //    {
        //        GameObject.Destroy(child.gameObject);
        //    }
        //}

        //// create each data point in activate layer
        //foreach (GameObject g in SSBM.selectedDataPoints)
        //{
        //    GameObject newPoint1 = Instantiate(dataPoint, g.transform.position, Quaternion.identity);
        //    newPoint1.transform.SetParent(sphereParent.transform);
        //    newPoint1.transform.GetComponent<MeshRenderer>().enabled = false;

        //    GameObject newPoint2 = Instantiate(dataPoint, g.transform.position, Quaternion.identity);
        //    newPoint2.transform.SetParent(sphereParentTwin.transform);
        //    newPoint2.transform.GetComponent<MeshRenderer>().enabled = false;
        //}

        if (experimentStart)
        {
            if (!prevExperimentStart)
            {
                readExperimentOrder();

                rearrangeOrder();

                finishText.SetActive(false);
            }

            switch (Scenario_No)
            {
                case 1:
                    controlMethod = Options.MagicHand;
                    VR_Hand_Control.methodSwitch = HandControl.Sphere;
                    break;
                case 2:
                    controlMethod = Options.ExtendedHand;
                    VR_Hand_Control.methodSwitch = HandControl.Sphere;
                    break;
                case 3:
                    controlMethod = Options.Portal;
                    VR_Hand_Control.methodSwitch = HandControl.Portal;
                    break;
                default:
                    break;
            }

            switch (experimentStage)
            {
                case 0: // training stage
                    orderInUse = trainingOrder;

                    timeList = new List<float>();
                    break;
                case 1: // in progress stage
                    orderInUse = orderList;
                    break;
                case 2: // finished
                    recordExperimentResult();

                    // show finished indication
                    finishText.SetActive(true);

                    experimentStart = false;
                    break;
                default: // do nothing
                    break;
            }

            if (prevExperimentStage != experimentStage)
            {
                currentOrderIndex = 0;

                if (experimentStage == 1 | experimentStage == 0)
                {
                    startPoint.SetActive(true);

                    GameObject newPoint2 = Instantiate(dataPoint, startPoint.transform.position, Quaternion.identity);
                    newPoint2.transform.SetParent(sphereParentTwin.transform);
                    newPoint2.transform.GetComponent<MeshRenderer>().enabled = false;
                }
            }

            afterHandCollision();

            debugText.GetComponent<TextMesh>().text = string.Join(",", orderInUse.Select(x => x.ToString())) + " , Index: " +  currentOrderIndex.ToString();

            prevExperimentStage = experimentStage;
            prevExperimentStart = experimentStart;
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
            unityClient.startCalibration == false &
            prev_gestureDetection == true & 
            current_gestureDetection == false
            )
            {
                sliderReference.transform.position = closestDataPoint.transform.position;

                //unityClient.initialPos();
                Vector3 newPos = unityClient.convertUnityCoord2RobotCoord(robotEndEffector.transform.position);

                unityClient.customMove(newPos.x, newPos.y, newPos.z, -0.6, 1.47, 0.62, movementType: 1);

                prevCloesetVector = closestDataPoint.transform.position;
            }
        }

        prev_gestureDetection = current_gestureDetection;
    }

    private void afterHandCollision()
    {
        if (startPointTouched)
        {
            // start timer here
            if (experimentStage == 1)
            {
                timeStamp = Time.time;
            }

            currentDataPoint = scatterParent.transform.GetChild(orderInUse[currentOrderIndex]).gameObject;

            currentDataPoint.GetComponent<SphereCollider>().enabled = true;

            if (currentDataPoint.GetComponent<MeshRenderer>().materials.Length < 2)
            {
                // add the outline effect as the second render material
                Material[] materials = currentDataPoint.GetComponent<MeshRenderer>().materials;
                Array.Resize(ref materials, materials.Length + 1);
                materials[materials.Length - 1] = outline;
                currentDataPoint.GetComponent<MeshRenderer>().materials = materials;
            }

            foreach (Transform g in sphereParentTwin.transform)
            {
                GameObject.Destroy(g.gameObject);
            }

            if (sphereParentTwin.transform.childCount == 0)
            {
                GameObject newPoint2 = Instantiate(dataPoint, currentDataPoint.transform.position, Quaternion.identity);
                newPoint2.transform.SetParent(sphereParentTwin.transform);
                newPoint2.transform.GetComponent<MeshRenderer>().enabled = false;
            }

            print("start point touched");
        }
        else if(dataPointTouched)
        {
            if (dataPointIndex == orderInUse[currentOrderIndex])
            {
                // stop timer here
                if (experimentStage == 1)
                {
                    timeList.Add(Time.time - timeStamp);
                }

                currentDataPoint.GetComponent<SphereCollider>().enabled = false;

                // remove the outline effect by deleting the second render material
                if (currentDataPoint.GetComponent<MeshRenderer>().materials.Length >= 2)
                {
                    // Remove the second material from the materials array
                    Material[] materials = currentDataPoint.GetComponent<MeshRenderer>().materials;
                    Array.Resize(ref materials, 1);
                    currentDataPoint.GetComponent<MeshRenderer>().materials = materials;
                }

                if (currentOrderIndex + 1 > orderInUse.Count)
                {
                    experimentStage += 1;

                    currentOrderIndex = 0;
                }
                else
                {
                    currentOrderIndex += 1;

                    startPoint.SetActive(true);

                    foreach (Transform g in sphereParentTwin.transform)
                    {
                        GameObject.Destroy(g.gameObject);
                    }

                    GameObject newPoint2 = Instantiate(dataPoint, startPoint.transform.position, Quaternion.identity);
                    newPoint2.transform.SetParent(sphereParentTwin.transform);
                    newPoint2.transform.GetComponent<MeshRenderer>().enabled = false;
                }

                print("data point touched");
            }
        }
    }

    private void rearrangeOrder()
    {
        int temp = Scenario_No - P_Number;

        while (temp < 0)
        {
            temp += 3;
        }

        int skipSize = temp * 5 + (P_Number - 1) * 15;

        while (skipSize > 30)
        {
            skipSize -= 30;
        }

        List<int> firstValues = orderList.Take(skipSize).ToList();

        orderList.RemoveRange(0, skipSize);

        orderList.AddRange(firstValues);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.name == "startPoint")
    //    {
    //        startPoint.SetActive(false);

    //        // start timer here
    //        if (experimentStage == 1)
    //        {
    //            timeStamp = Time.time;
    //        }

    //        currentDataPoint = scatterParent.transform.GetChild(orderInUse[currentOrderIndex]).gameObject;

    //        currentDataPoint.layer = LayerMask.NameToLayer("Outline");

    //        foreach (Transform g in sphereParentTwin.transform)
    //        {
    //            GameObject.Destroy(g.gameObject);
    //        }

    //        GameObject newPoint2 = Instantiate(dataPoint, currentDataPoint.transform.position, Quaternion.identity);
    //        newPoint2.transform.SetParent(sphereParentTwin.transform);
    //        newPoint2.transform.GetComponent<MeshRenderer>().enabled = false;

    //        print("start point touched");
    //    }
    //    else if (other.gameObject.tag == "DataPoint")
    //    {
    //        if (other.transform.GetSiblingIndex() == currentOrderIndex)
    //        {
    //            // stop timer here
    //            if (experimentStage == 1)
    //            {
    //                timeList.Add(Time.time - timeStamp);
    //            }

    //            currentDataPoint.layer = LayerMask.NameToLayer("Default");

    //            if (currentOrderIndex + 1 > orderInUse.Count)
    //            {
    //                experimentStage += 1;
    //            }
    //            else
    //            {
    //                currentOrderIndex += 1;

    //                startPoint.SetActive(true);

    //                foreach (Transform g in sphereParentTwin.transform)
    //                {
    //                    GameObject.Destroy(g.gameObject);
    //                }

    //                GameObject newPoint2 = Instantiate(dataPoint, startPoint.transform.position, Quaternion.identity);
    //                newPoint2.transform.SetParent(sphereParentTwin.transform);
    //                newPoint2.transform.GetComponent<MeshRenderer>().enabled = false;
    //            }

    //            print("data point touched");
    //        }
    //    }
    //}

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

    private void readExperimentOrder() // read pre-defined order based on the participant number
    {
        string file = "Assets/RawData/Experiment_Order.csv";

        StreamReader reader = new StreamReader(file);

        while (!reader.EndOfStream)
        {
            orderList.Add(int.Parse(reader.ReadLine()));
        }

        reader.Close();
    }

    private void recordExperimentResult() // save the result of experiment with participant number, scenario number in a txt file
    {
        string fileName = "YO-YO" + DateTime.Now.ToString("yyyy-MM-dd") + "_P" + P_Number.ToString() + "_S" + Scenario_No.ToString();
        string saveFileName = "Assets/RawData/" + fileName + ".txt";

        while (File.Exists(saveFileName))
        {
            duplicateFileIndex++;
            saveFileName = "Assets/RawData/" + fileName + "_D" + duplicateFileIndex.ToString() + ".txt";
        }

        StreamWriter sw = new StreamWriter(saveFileName);

        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd\\THH:mm:ss\\Z"));
        sw.WriteLine(" ");

        sw.WriteLine("Time Taken");
        foreach (var value in timeList)
        {
            sw.WriteLine(value.ToString());
        }
        sw.WriteLine(" ");

        sw.Close();
    }
}

public enum Options // your custom enumeration
{
    MagicHand,
    ExtendedHand,
    Portal
};
