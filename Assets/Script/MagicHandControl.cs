using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;

public class MagicHandControl : MonoBehaviour
{
    public GameObject sphereParent;
    public GameObject sphereParentTwin;

    public GameObject VRHand;
    public GameObject VRHandTwin;

    public GameObject VRHandPlaceHolder;
    public GameObject VRHandTwinPlaceHolder;

    public UnityClient unityClient;

    public MeshCollider robotRange;

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
    public GameObject DW2_PlaceHolder;
    public GameObject Camera;
    public GameObject cameraParent;
    private Vector3 cameraOffset = new Vector3(0f, 0f, 0.11f);

    public ScatterSlicerBoxManagement SSBM;
    public GameObject dataPoint;

    private bool prev_gestureDetection = false;
    private bool current_gestureDetection = false;

    private GameObject closestDataPoint;
    private Vector3 prevCloesetVector = Vector3.zero;

    //public VRHandControl VR_Hand_Control;
    public VRHandControlGoGo VR_Hand_Control;

    public GameObject scatterParent;
    public GameObject startPoint;
    public GameObject finishText;
    public Material outline;
    public GameObject debugText;

    public bool experimentStart = false;
    public int P_Number = -1;
    public int Scenario_No = -1; // 1 - magic hand; 2 - connected hand; 3 - portals
    public int experimentStage = -1;

    public bool rotationLock = false;

    private int prevExperimentStage = -1;
    private int prevScenarioNo = -1;
    private bool prevExperimentStart = false;
    private List<int> trainingOrder = new List<int>();
    private List<int> orderList = new List<int>();
    private List<int> experimentOrder = new List<int>();
    private List<int> orderInUse = new List<int>();
    public int currentOrderIndex = -1;
    public int firstIndex = 0;
    private List<Vector3> posList = new List<Vector3>();
    private List<Vector3> handList = new List<Vector3>();
    private List<Vector3> elbowList = new List<Vector3>();
    private List<Vector3> shoulderList = new List<Vector3>();
    private List<float> TimeStampList = new List<float>();
    private List<float> DistanceList = new List<float>();
    private Dictionary<int, List<Vector3>> trajectoryDict = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, List<Vector3>> handDict = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, List<Vector3>> elbowDict = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, List<Vector3>> shoulderDict = new Dictionary<int, List<Vector3>>();
    private Dictionary<int, List<float>> trajectoryTimeStampDict = new Dictionary<int, List<float>>();
    private Dictionary<int, List<float>> trajectoryDistanceDict = new Dictionary<int, List<float>>();
    private float timeStamp;
    private GameObject currentDataPoint = null;
    private int duplicateFileIndex = 0;

    private Quaternion rotationReference;
    private Quaternion lastFrameHandRotation;

    private bool prevStartPointTouched = false;
    private bool prevDataPointTouched = false;

    public bool startPointTouched = false;
    public bool dataPointTouched = false;
    static public int dataPointIndex;

    public Collider warningArea;
    public Collider touchDetection;

    private int touchFrameCounter = 0;

    public GameObject cameraPoint;

    public GameObject animationObject;

    public GameObject animationObjectPortal;

    public GameObject stopPlane;
    private bool startAnimationFlag = false;
    private bool resetAnimationFlag = false;

    public GameObject projectedPoint;
    private int projectionPlaneIndex = 1;

    private Vector3 p1, p2;
    private Vector3 normal;
    public Vector3 pointOnPlane;
    private GameObject trackedEndEffector;

    public Collider robotRangeEndEffector;
    public GameObject planeNormalParent;
    public bool startFollowUp;
    public int skipThreshold;
    public int linearDistanceTest = 0;

    private int skipFrameCounter;
    //private int maxLinearActuatorDistance = 1;

    public bool printOrderFlag = false;
    public TextMeshPro titleDisplay;

    public Collider resetPosCollider;

    private float FarDistance, CloseDistance;
    private bool resetFlagStage1 = false;
    public bool resetTest = false;
    public bool startInitialPoint = false;

    private bool robotMoveFlag = false;
    public GameObject PortalFlyingStartPoint;

    public GameObject breakText;
    private bool InExperimentRestFlag = false;
    private bool PreInExperimentRestFlag = false;

    private bool prevDynamicFlag = false;

    public bool inPortalLookUpFlag = false;
    public GameObject mainCamera;
    public GameObject portalOnCameraPlaceholder;

    private Vector3 InPortalPos = new Vector3(0.408f,0.313f,-0.039f);
    private Vector3 InPortalRotation = new Vector3(0, 180, 0);

    public GameObject elbowVICON;
    public GameObject shoulderVICON;

    public Material indicatorM;
    public bool autoResume = false;
    public AudioSource successGrab;
    public AudioSource fatigueReport;

    string IP = "192.168.50.255";
    string currentFileName;
    int port = 9000;
    string xml;

    IPEndPoint remoteEndPoint;
    UdpClient client;

    bool inWaite = false;
    bool pre_inWaite = false;

    bool portalBuffer = false;
    int portalBufferCounter = 10;

    IEnumerator ResumeAfter15s()
    {
        while (true)
        {
            if (autoResume)
            {
                if (InExperimentRestFlag)
                {
                    inWaite = true;

                    if (!breakText.activeSelf)
                    {
                        breakText.GetComponent<TextMeshPro>().text = "Break !";

                        breakText.SetActive(true);
                    }

                    fatigueReport.Play();

                    yield return new WaitForSeconds(10f);

                    breakText.GetComponent<TextMeshPro>().text = "5";

                    yield return new WaitForSeconds(1f);

                    breakText.GetComponent<TextMeshPro>().text = "4";

                    yield return new WaitForSeconds(1f);

                    breakText.GetComponent<TextMeshPro>().text = "3";

                    yield return new WaitForSeconds(1f);

                    breakText.GetComponent<TextMeshPro>().text = "2";

                    yield return new WaitForSeconds(1f);

                    breakText.GetComponent<TextMeshPro>().text = "1";

                    fatigueReport.Play();

                    yield return new WaitForSeconds(1f);

                    inWaite = false;
                    startInitialPoint = true;

                    InExperimentRestFlag = false;
                    breakText.SetActive(false);
                }
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Start()
    {
        p1 = planeNormalParent.transform.GetChild(0).position;
        p2 = planeNormalParent.transform.GetChild(1).position;

        normal = (p2 - p1).normalized;

        pointOnPlane = p1;

        trackedEndEffector = PPR.TCP_Center_Tracked;

        sliderReference = PPR.Touch_Point;
        robotEndEffector = PPR.TCP_Center;

        trainingOrder.Add(11);
        trainingOrder.Add(57);
        trainingOrder.Add(100);
        trainingOrder.Add(239);
        trainingOrder.Add(427);
        trainingOrder.Add(311);
        trainingOrder.Add(1000);
        trainingOrder.Add(1230);
        trainingOrder.Add(2031);
        trainingOrder.Add(592);
        trainingOrder.Add(2560);
        trainingOrder.Add(1830);
        trainingOrder.Add(0);

        rotationReference = Camera.transform.rotation;

        // Vicon Control
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        StartCoroutine(ResumeAfter15s());
    }

    // Update is called once per frame
    void Update()
    {
        if (printOrderFlag)
        {
            printOrder();
            printOrderFlag = false;
        }

        if (experimentStart)
        {
            if (!prevExperimentStart)
            {
                readExperimentOrder();

                rearrangeOrder();

                finishText.SetActive(false);

                // reset experiment here
                resetDataList();

                experimentStage = 0;
            }

            if (prevScenarioNo != Scenario_No)
            {
                rearrangeOrder();
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
                    //VR_Hand_Control.methodSwitch = HandControl.Sphere;
                    break;
                case 4: // portal with robot
                    controlMethod = Options.HapticPortal;
                    VR_Hand_Control.methodSwitch = HandControl.Portal;
                    break;
                default:
                    break;
            }

            switch (experimentStage)
            {
                case 0: // training stage
                    titleDisplay.text = "Training" + " " + currentOrderIndex.ToString();
                    orderInUse = trainingOrder;

                    posList = new List<Vector3>();
                    handList = new List<Vector3>();
                    elbowList = new List<Vector3>();
                    shoulderList = new List<Vector3>();
                    DistanceList = new List<float>();
                    TimeStampList = new List<float>();
                    trajectoryDict = new Dictionary<int, List<Vector3>>();
                    handDict = new Dictionary<int, List<Vector3>>();
                    elbowDict = new Dictionary<int, List<Vector3>>();
                    shoulderDict = new Dictionary<int, List<Vector3>>();
                    trajectoryTimeStampDict = new Dictionary<int, List<float>>();
                    trajectoryDistanceDict = new Dictionary<int, List<float>>();
                    break;
                case 1: // in progress stage
                    titleDisplay.text = "User Study" + " " + currentOrderIndex.ToString();
                    orderInUse = experimentOrder;

                    if (!inWaite)
                    {
                        posList.Add(VRHandTwin.transform.position);
                        handList.Add(VRHand.transform.position);
                        elbowList.Add(elbowVICON.transform.position);
                        shoulderList.Add(shoulderVICON.transform.position);
                        DistanceList.Add(Vector3.Distance(VRHandTwin.transform.position, currentDataPoint.transform.position));
                        TimeStampList.Add(Time.time);
                    }
                    
                    break;
                case 2: // finished
                    if (prevExperimentStage == 1)
                    {
                        trajectoryDict.Add(orderInUse[currentOrderIndex], posList);
                        handDict.Add(orderInUse[currentOrderIndex], handList);
                        elbowDict.Add(orderInUse[currentOrderIndex], elbowList);
                        shoulderDict.Add(orderInUse[currentOrderIndex], shoulderList);
                        trajectoryDistanceDict.Add(orderInUse[currentOrderIndex], DistanceList);
                        trajectoryTimeStampDict.Add(orderInUse[currentOrderIndex], TimeStampList);

                        InExperimentRestFlag = true;
                    }

                    if (!InExperimentRestFlag)
                    {
                        titleDisplay.text = "Finished";

                        // show finished indication
                        finishText.SetActive(true);

                        experimentStart = false;
                    }

                    if (prevExperimentStage != 2)
                    {
                        ViconStop();

                        saveHandMovement();
                    }
                    
                    //titleDisplay.text = "Finished";

                    //// show finished indication
                    //finishText.SetActive(true);

                    //experimentStart = false;

                    break;
                default: // do nothing
                    break;
            }

            if (prevExperimentStage != experimentStage)
            {
                currentOrderIndex = 0;
            }

            prevExperimentStage = experimentStage;
            prevScenarioNo = Scenario_No;

            afterHandCollision();
        }

        prevExperimentStart = experimentStart;

        // fetch the gesture detection flag from hand model
        current_gestureDetection = (VRHand.GetComponent<VRHandControlGoGo>().gestureDetection | VRHand.GetComponent<VRHandControlGoGo>().rotationGesture);

        switch (((int)controlMethod))
        {
            case 0: // magic hand control
                    //ArmRender.enable = false;

                //setObjectActive(portalRelevant, false);
                //VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;
                //VRHandTwin.GetComponent<VRHandDisplay>().hideVRHand = false;

                //cameraParent.SetActive(false);

                //Camera.transform.position = VRHandTwin.transform.position + cameraOffset;

                //if (rotationLock)
                //{
                //    Camera.transform.rotation = DW2_PlaceHolder.transform.rotation * Quaternion.Inverse(lastFrameHandRotation) * Camera.transform.rotation;
                //}

                //(sphereParentTwin.transform.position, sphereParentTwin.transform.rotation) = getNewPosRotAfterRotation(VRHandTwinPlaceHolder.transform, VRHandPlaceHolder.transform, sphereParent.transform);

                //break;
                ArmRender.enable = true;

                setObjectActive(portalRelevant, false);
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;
                VRHandTwin.GetComponent<VRHandDisplay>().hideVRHand = false;

                cameraParent.SetActive(false);

                Camera.transform.position = VRHandTwin.transform.position + cameraOffset;
                if (rotationLock)
                {
                    Camera.transform.rotation = DW2_PlaceHolder.transform.rotation * Quaternion.Inverse(lastFrameHandRotation) * Camera.transform.rotation;
                }

                (sphereParentTwin.transform.position, sphereParentTwin.transform.rotation) = getNewPosRotAfterRotation(VRHandTwinPlaceHolder.transform, VRHandPlaceHolder.transform, sphereParent.transform);

                break;
            case 1: // extended hand control
                ArmRender.enable = true;

                setObjectActive(portalRelevant, false);
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;
                VRHandTwin.GetComponent<VRHandDisplay>().hideVRHand = false;

                cameraParent.SetActive(false);

                Camera.transform.position = VRHandTwin.transform.position + cameraOffset;
                if (rotationLock)
                {
                    Camera.transform.rotation = DW2_PlaceHolder.transform.rotation * Quaternion.Inverse(lastFrameHandRotation) * Camera.transform.rotation;
                }

                (sphereParentTwin.transform.position, sphereParentTwin.transform.rotation) = getNewPosRotAfterRotation(VRHandTwinPlaceHolder.transform, VRHandPlaceHolder.transform, sphereParent.transform);
                
                break;
            case 2: // portal control
                ArmRender.enable = true;

                setObjectActive(portalRelevant, true);

                cameraParent.SetActive(false);

                //(sphereTwin.transform.position, sphereTwin.transform.rotation) = getTargetPosRot(portal2.transform, portal1.transform, sphere.transform);
                (sphereParentTwin.transform.position, sphereParentTwin.transform.rotation) = getNewPosRotAfterRotation(portal2PlaceHolder.transform, portal1PlaceHolder.transform, sphereParent.transform);

                if (current_gestureDetection)
                {
                    ArmRender.GetComponent<VRHandArmRender>().normalFlag = false;
                }
                else
                {
                    ArmRender.GetComponent<VRHandArmRender>().normalFlag = true;
                }

                inPortalLookAt();

                break;
            case 3: // portal control + robot
                ArmRender.enable = true;

                setObjectActive(portalRelevant, true);

                cameraParent.SetActive(false);

                //(sphereTwin.transform.position, sphereTwin.transform.rotation) = getTargetPosRot(portal2.transform, portal1.transform, sphere.transform);
                (sphereParentTwin.transform.position, sphereParentTwin.transform.rotation) = getNewPosRotAfterRotation(portal2PlaceHolder.transform, portal1PlaceHolder.transform, sphereParent.transform);

                if (current_gestureDetection)
                {
                    ArmRender.GetComponent<VRHandArmRender>().normalFlag = false;
                }
                else
                {
                    ArmRender.GetComponent<VRHandArmRender>().normalFlag = true;
                }

                inPortalLookAt();

                break;
            default: // magic hand control
                ArmRender.enable = false;

                setObjectActive(portalRelevant, false);
                VRHand.GetComponent<VRHandDisplay>().hideVRHand = true;

                cameraParent.SetActive(false);

                Camera.transform.position = VRHandTwin.transform.position + cameraOffset;
                if (rotationLock)
                {
                    Camera.transform.rotation = DW2_PlaceHolder.transform.rotation * Quaternion.Inverse(lastFrameHandRotation) * Camera.transform.rotation;
                }

                if (current_gestureDetection == false & prev_gestureDetection == true)
                {
                    //(sphereTwin.transform.position, sphereTwin.transform.rotation) = getTargetPosRot(VRHandTwin.transform, VRHand.transform, sphere.transform);
                    (sphereParentTwin.transform.position, sphereParentTwin.transform.rotation) = getNewPosRotAfterRotation(VRHandTwinPlaceHolder.transform, VRHandPlaceHolder.transform, sphereParent.transform);
                }
                break;
        }

        if ((int)controlMethod == 3) 
        {
            robotMoveFlag = true;
        }
        else
        {
            robotMoveFlag = false;
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

            //if (prev_gestureDetection == true & current_gestureDetection == false & !animatorPortal.GetBool("start flying"))
            //{
            //    setAnimationStartingPos();

            //    startAnimationFlag = true;
            //}

            //if (prev_gestureDetection == false & current_gestureDetection == true)
            //{
            //    resetAnimation();
            //}

            prevDynamicFlag = VRHand.GetComponent<VRHandControlGoGo>().DynamicFlag;

            // robot move
            if (robotMoveFlag)
            {
                sliderReference.transform.position = closestDataPoint.transform.position;

                if (robotRange.bounds.Contains(robotEndEffector.transform.position) &
                            unityClient.startCalibration == false &
                            (unityClient.homePosition | Vector3.Distance(prevCloesetVector, closestDataPoint.transform.position) > 0.0001) & 
                            !current_gestureDetection
                            )
                {
                    Vector3 newPos = unityClient.convertUnityCoord2RobotCoord(robotEndEffector.transform.position);

                    unityClient.customMove(newPos.x, newPos.y, newPos.z, -0.6, 1.47, 0.62, movementType: 0);

                    prevCloesetVector = closestDataPoint.transform.position;
                }

                if (prev_gestureDetection == false & current_gestureDetection == true & !unityClient.homePosition)
                {
                    unityClient.initialPos();
                }
            }

            if (Vector3.Distance(sphereParent.transform.GetChild(0).position, VRHandTwin.transform.position) < 0.15 & current_gestureDetection == false)
            {
                VR_Hand_Control.posDetectionLock = true;
            }
            else
            {
                VR_Hand_Control.posDetectionLock = false;
            }

            if (touchDetection.bounds.Contains(sphereParent.transform.GetChild(0).position) & !current_gestureDetection)
            {
                touchFrameCounter += 1;
            }
            else
            {
                touchFrameCounter = 0;
            }
        }
        prev_gestureDetection = current_gestureDetection;
        lastFrameHandRotation = DW2_PlaceHolder.transform.rotation;

        if (touchFrameCounter > 30)
        {
            if (startPoint.activeSelf)
            {
                startPointTouched = true;
            }
            else
            {
                dataPointTouched = true;
            }

            touchFrameCounter = 0;
        }
        else
        {
            startPointTouched = false;
            dataPointTouched = false;
        }

        if (startFollowUp)
        {
            robotFollowUp();
        }

        if (resetTest)
        {
            // reset robot position when the gesture is detected after each collision 
            resetRobotPosInExperiment();
        }
        
        if (resetPosCollider.bounds.Contains(VRHand.transform.position))
        {
            VR_Hand_Control.resetConfig();
        }
    }

    private void resetDataList()
    {
        posList = new List<Vector3>();
        handList = new List<Vector3>();
        elbowList = new List<Vector3>();
        shoulderList = new List<Vector3>();
        DistanceList = new List<float>();
        TimeStampList = new List<float>();
        trajectoryDict = new Dictionary<int, List<Vector3>>();
        handDict = new Dictionary<int, List<Vector3>>();
        elbowDict = new Dictionary<int, List<Vector3>>();
        shoulderDict = new Dictionary<int, List<Vector3>>();
        trajectoryTimeStampDict = new Dictionary<int, List<float>>();
        trajectoryDistanceDict = new Dictionary<int, List<float>>();
    }

    private void resetRobotPosInExperiment()
    {
        if (!startPointTouched & prevStartPointTouched)
        {
            resetFlagStage1 = true;
        }

        if (resetFlagStage1)
        {
            if (current_gestureDetection & !unityClient.homePosition)
            {
                unityClient.initialPos();
                resetFlagStage1 = false;
            }
        }
    }

    private void printOrder()
    {
        foreach (int i in orderInUse)
        {
            print(i);
        }
    }

    private void robotFollowUp()
    {
        projectionPlaneIndex = Mathf.CeilToInt(Mathf.Abs(Vector3.Dot(normal, p1 - closestDataPoint.transform.position)) / (FarDistance - CloseDistance));

        if (projectionPlaneIndex > 2)
        {
            projectionPlaneIndex = 2;
        }

        if (projectionPlaneIndex < 1)
        {
            projectionPlaneIndex = 1;
        }

        pointOnPlane = p1 + normal * (FarDistance - CloseDistance)*(projectionPlaneIndex-1);

        projectedPoint.transform.position = Vector3.ProjectOnPlane(closestDataPoint.transform.position - pointOnPlane, normal) + pointOnPlane;

        sliderReference.transform.position = projectedPoint.transform.position;

        Vector3 referencePos1 = unityClient.convertUnityCoord2RobotCoord(robotEndEffector.transform.position);

        if (skipFrameCounter > skipThreshold & robotRangeEndEffector.bounds.Contains(robotEndEffector.transform.position) & current_gestureDetection)
        {
            // change the distance of the linear actuator
            float distance = Mathf.Abs(Vector3.Dot(normal, pointOnPlane - closestDataPoint.transform.position));
                
            // print(distance + " " + CloseDistance + " " + FarDistance + " " + (distance - CloseDistance) / (FarDistance - CloseDistance) * 5 + " " + projectionPlaneIndex);

            unityClient.customMove(referencePos1.x, referencePos1.y, referencePos1.z, -0.6, 1.47, 0.62, acc:330, movementType: 1, interruptible: 1, radius: 0.05f, linearActuatorDistance: (distance - CloseDistance) / (FarDistance - CloseDistance) * 5);

            skipFrameCounter = 0;
            prevCloesetVector = closestDataPoint.transform.position;

            // unityClient.customMove(referencePos1.x, referencePos1.y, referencePos1.z, -0.6, 1.47, 0.62, movementType: 1, interruptible: 1, scenario: 5, linearActuatorDistance: linearDistanceTest);
        }
        
        if(!robotRangeEndEffector.bounds.Contains(robotEndEffector.transform.position) & current_gestureDetection)
        {
            if (resetFlagStage1)
            {
                if (!unityClient.homePosition)
                {
                    unityClient.initialPos();
                }
            }
        }

        skipFrameCounter++;
    }

    // this method require user to return to the start point after each interaction
    //private void afterHandCollision()
    //{
    //    if (startPointTouched & !prevStartPointTouched)
    //    {
    //        startPoint.SetActive(false);

    //        // start timer here
    //        if (experimentStage == 1)
    //        {
    //            timeStamp = Time.time;
    //        }

    //        currentDataPoint = scatterParent.transform.GetChild(orderInUse[currentOrderIndex]).gameObject;

    //        currentDataPoint.GetComponent<SphereCollider>().enabled = true;

    //        if (currentDataPoint.GetComponent<MeshRenderer>().materials.Length < 2)
    //        {
    //            // add the outline effect as the second render material
    //            Material[] materials = currentDataPoint.GetComponent<MeshRenderer>().materials;
    //            Array.Resize(ref materials, materials.Length + 1);
    //            materials[materials.Length - 1] = outline;
    //            currentDataPoint.GetComponent<MeshRenderer>().materials = materials;
    //        }

    //        foreach (Transform g in sphereParentTwin.transform)
    //        {
    //            GameObject.Destroy(g.gameObject);
    //        }

    //        foreach (Transform g in sphereParent.transform)
    //        {
    //            GameObject.Destroy(g.gameObject);
    //        }

    //        GameObject newPoint1 = Instantiate(dataPoint, currentDataPoint.transform.position, Quaternion.identity);
    //        newPoint1.transform.SetParent(sphereParentTwin.transform);
    //        newPoint1.transform.GetComponent<MeshRenderer>().enabled = false;
            

    //        GameObject newPoint2 = Instantiate(dataPoint, currentDataPoint.transform.position, Quaternion.identity);
    //        newPoint2.transform.SetParent(sphereParent.transform);
    //        newPoint2.transform.GetComponent<MeshRenderer>().enabled = false;

    //        newPoint1.transform.localPosition = newPoint2.transform.localPosition;

    //        // stopping plane method - inactive
    //        //stopPlane.transform.position = currentDataPoint.transform.position;

    //        if (currentOrderIndex != 0) // save pos list and the corresponding index to dict
    //        {
    //            trajectoryDict.Add(orderInUse[currentOrderIndex], posList);

    //            posList = new List<Vector3>();
    //        }

    //    }
    //    else if(dataPointTouched & !prevDataPointTouched)
    //    {
    //        // stop timer here
    //        if (experimentStage == 1)
    //        {
    //            timeList.Add(Time.time - timeStamp);
    //        }

    //        currentDataPoint.GetComponent<SphereCollider>().enabled = false;

    //        // remove the outline effect by deleting the second render material
    //        if (currentDataPoint.GetComponent<MeshRenderer>().materials.Length >= 2)
    //        {
    //            // Remove the second material from the materials array
    //            Material[] materials = currentDataPoint.GetComponent<MeshRenderer>().materials;
    //            Array.Resize(ref materials, 1);
    //            currentDataPoint.GetComponent<MeshRenderer>().materials = materials;
    //        }

    //        startPoint.SetActive(true);

    //        foreach (Transform g in sphereParentTwin.transform)
    //        {
    //            GameObject.Destroy(g.gameObject);
    //        }

    //        foreach (Transform g in sphereParent.transform)
    //        {
    //            GameObject.Destroy(g.gameObject);
    //        }

    //        GameObject newPoint1 = Instantiate(dataPoint, startPoint.transform.position, Quaternion.identity);
    //        newPoint1.transform.SetParent(sphereParentTwin.transform);
    //        newPoint1.transform.GetComponent<MeshRenderer>().enabled = false;

    //        GameObject newPoint2 = Instantiate(dataPoint, startPoint.transform.position, Quaternion.identity);
    //        newPoint2.transform.SetParent(sphereParent.transform);
    //        newPoint2.transform.GetComponent<MeshRenderer>().enabled = false;

    //        newPoint1.transform.localPosition = newPoint2.transform.localPosition;

    //        if (currentOrderIndex + 1 >= orderInUse.Count)
    //            {
    //                experimentStage += 1;

    //                currentOrderIndex = 0;
    //            }
    //            else
    //            {
    //                currentOrderIndex += 1;

    //                // stopping plane method - inactive
    //                //stopPlane.transform.position = startPoint.transform.position;
    //            }

    //    }

    //    prevStartPointTouched = startPointTouched;
    //    prevDataPointTouched = dataPointTouched;
    //}

    private void afterHandCollision()
    {
        if ((currentOrderIndex == 7 | currentOrderIndex == 14 | currentOrderIndex == 21 | currentOrderIndex == 28) & !InExperimentRestFlag & !PreInExperimentRestFlag)
        {
            InExperimentRestFlag = true;
        }

        if ((dataPointTouched & !prevDataPointTouched & !InExperimentRestFlag) | startInitialPoint)
        {
            if (currentOrderIndex == 0 & experimentStage == 1)
            {
                ViconStart();
            }

            VR_Hand_Control.InRangePos = new List<Vector3>();
            animationObjectPortal.SetActive(false);
            startInitialPoint = false;
            successGrab.Play();

            PreInExperimentRestFlag = InExperimentRestFlag;

            if (((currentOrderIndex == 7 | currentOrderIndex == 14 | currentOrderIndex == 21 | currentOrderIndex == 28) & PreInExperimentRestFlag) |
                (currentOrderIndex != 7 & currentOrderIndex != 14 & currentOrderIndex != 21 & currentOrderIndex != 28) |
                experimentStage == 0)
            {

                if ((currentOrderIndex == 7 | currentOrderIndex == 14 | currentOrderIndex == 21 | currentOrderIndex == 28) & inWaite)
                {
                    return;
                }

                if (currentDataPoint != null)
                {
                    currentDataPoint.GetComponent<SphereCollider>().enabled = false;

                    // remove the outline effect by deleting the second render material
                    if (currentDataPoint.GetComponent<MeshRenderer>().materials.Length >= 2)
                    {
                        // Remove the second material from the materials array
                        Material[] materials = currentDataPoint.GetComponent<MeshRenderer>().materials;
                        Array.Resize(ref materials, 1);
                        currentDataPoint.GetComponent<MeshRenderer>().materials = materials;
                    }
                }

                foreach (Transform g in sphereParentTwin.transform)
                {
                    GameObject.Destroy(g.gameObject);
                }

                foreach (Transform g in sphereParent.transform)
                {
                    GameObject.Destroy(g.gameObject);
                }

                if (currentOrderIndex < orderInUse.Count)
                {
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

                    GameObject newPoint1 = Instantiate(dataPoint, currentDataPoint.transform.position, Quaternion.identity);
                    newPoint1.transform.SetParent(sphereParentTwin.transform);
                    newPoint1.transform.GetComponent<MeshRenderer>().enabled = false;

                    GameObject newPoint2 = Instantiate(dataPoint, currentDataPoint.transform.position, Quaternion.identity);
                    newPoint2.transform.SetParent(sphereParent.transform);
                    newPoint2.transform.GetComponent<MeshRenderer>().enabled = false;

                    newPoint1.transform.localPosition = newPoint2.transform.localPosition;

                    if (currentOrderIndex != 0) // save pos list and the corresponding index to dict
                    {
                        if (orderInUse[currentOrderIndex] != 11)
                        {
                            trajectoryDict.Add(orderInUse[currentOrderIndex], posList);
                            handDict.Add(orderInUse[currentOrderIndex], handList);
                            elbowDict.Add(orderInUse[currentOrderIndex], elbowList);
                            shoulderDict.Add(orderInUse[currentOrderIndex], shoulderList);
                            trajectoryDistanceDict.Add(orderInUse[currentOrderIndex], DistanceList);
                            trajectoryTimeStampDict.Add(orderInUse[currentOrderIndex], TimeStampList);
                        }

                        posList = new List<Vector3>();
                        handList = new List<Vector3>();
                        elbowList = new List<Vector3>();
                        shoulderList = new List<Vector3>();
                        DistanceList = new List<float>();
                        TimeStampList = new List<float>();
                    }

                    currentOrderIndex += 1;
                }
                else
                {
                    experimentStage += 1;

                    currentOrderIndex = 0;
                }

                resetAnimationFlag = true;
            }

            InExperimentRestFlag = false;
        }

        prevDataPointTouched = dataPointTouched;
        pre_inWaite = inWaite;
    }

    private void rearrangeOrder()
    {
        int skipSize = (P_Number - 1) * 5 + (Scenario_No - 1) * 5;

        skipSize = skipSize % 30;

        experimentOrder = new List<int>();
        experimentOrder.AddRange(orderList.Skip(skipSize).ToList());
        experimentOrder.AddRange(orderList.Take(skipSize).ToList());

        experimentOrder.Insert(7, trainingOrder[0]);
        experimentOrder.Insert(14, trainingOrder[0]);
        experimentOrder.Insert(21, trainingOrder[0]);
        experimentOrder.Insert(28, trainingOrder[0]);

        firstIndex = experimentOrder[1];
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

    private void readExperimentOrder() // read pre-defined order based on the participant number
    {
        orderList = new List<int>();

        string file = "Assets/RawData/Experiment_Order.csv";

        StreamReader reader = new StreamReader(file);

        while (!reader.EndOfStream)
        {
            orderList.Add(int.Parse(reader.ReadLine()));
        }

        reader.Close();
    }

    private void saveHandMovement()
    {
        // hand position
        string fileName = "Linear GoGo Hand Trajectory " + DateTime.Now.ToString("yyyy-MM-dd") + " P" + P_Number.ToString() + " S" + Scenario_No.ToString();
        string saveFileName = "Assets/RawData/" + fileName + ".txt";

        while (File.Exists(saveFileName))
        {
            duplicateFileIndex++;
            saveFileName = "Assets/RawData/" + fileName + "_D" + duplicateFileIndex.ToString() + ".txt";
        }

        StreamWriter sw = new StreamWriter(saveFileName);

        sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd\\THH:mm:ss\\Z"));
        sw.WriteLine(" ");

        sw.WriteLine("Index_No. VRHandPos.x VRHandPos.y VRHandPos.z HandPos.x HandPos.y HandPos.z ElbowPos.x ElbowPos.y ElbowPos.z ShoulderPos.x ShoulderPos.y ShoulderPos.z Time_Stamp Distance");
        foreach (int index in trajectoryDict.Keys)
        {
            for (int i = 0; i < trajectoryDict[index].Count; i++)
            {
                Vector3 pos = trajectoryDict[index][i];
                Vector3 handPos = handDict[index][i];
                Vector3 elbowPos = elbowDict[index][i];
                Vector3 shoulderPos = shoulderDict[index][i];
                float t = trajectoryTimeStampDict[index][i];
                float d = trajectoryDistanceDict[index][i];

                string s = index.ToString() + " " + 
                            pos.x + " " + pos.y + " " + pos.z + " " +
                            handPos.x + " " + handPos.y + " " + handPos.z + " " +
                            elbowPos.x + " " + elbowPos.y + " " + elbowPos.z + " " +
                            shoulderPos.x + " " + shoulderPos.y + " " + shoulderPos.z + " " +
                            t.ToString() + " " +
                            d.ToString();

                sw.WriteLine(s);
            }
        }

        sw.Close();

        //// hand time stamp
        //string fileName1 = "Linear GoGo Hand Trajectory Time Stamp " + DateTime.Now.ToString("yyyy-MM-dd") + " P" + P_Number.ToString() + " S" + Scenario_No.ToString();
        //string saveFileName1 = "Assets/RawData/" + fileName1 + ".txt";

        //while (File.Exists(saveFileName1))
        //{
        //    duplicateFileIndex++;
        //    saveFileName1 = "Assets/RawData/" + fileName1 + "_D" + duplicateFileIndex.ToString() + ".txt";
        //}

        //StreamWriter sw1 = new StreamWriter(saveFileName1);

        //sw1.WriteLine(DateTime.Now.ToString("yyyy-MM-dd\\THH:mm:ss\\Z"));
        //sw1.WriteLine(" ");

        //sw1.WriteLine("Index_No.     Time_Stamp"); // Time Stamp
        //foreach (int index in trajectoryTimeStampDict.Keys)
        //{
        //    foreach (float t in trajectoryTimeStampDict[index])
        //    {
        //        sw1.WriteLine(index.ToString() + "      " + t.ToString());
        //    }
        //}
        //sw1.WriteLine(" ");

        //sw1.Close();

        //// hand target distance
        //string fileName2 = "Linear GoGo Hand Trajectory Distance " + DateTime.Now.ToString("yyyy-MM-dd") + " P" + P_Number.ToString() + " S" + Scenario_No.ToString();
        //string saveFileName2 = "Assets/RawData/" + fileName2 + ".txt";

        //while (File.Exists(saveFileName2))
        //{
        //    duplicateFileIndex++;
        //    saveFileName2 = "Assets/RawData/" + fileName2 + "_D" + duplicateFileIndex.ToString() + ".txt";
        //}

        //StreamWriter sw2 = new StreamWriter(saveFileName2);

        //sw2.WriteLine(DateTime.Now.ToString("yyyy-MM-dd\\THH:mm:ss\\Z"));
        //sw2.WriteLine(" ");

        //sw2.WriteLine("Index_No.    Distance"); // Distance
        //foreach (int index in trajectoryDistanceDict.Keys)
        //{
        //    foreach (float d in trajectoryDistanceDict[index])
        //    {
        //        sw2.WriteLine(index.ToString() + "      " + d.ToString());
        //    }
        //}
        //sw2.WriteLine(" ");

        //sw2.Close();
    }

    private void inPortalLookAt()
    {
        if (inPortalLookUpFlag)
        {
            if (current_gestureDetection)
            {
                animationObjectPortal.SetActive(true);

                animationObjectPortal.transform.position = portalOnCameraPlaceholder.transform.position;
                animationObjectPortal.transform.rotation = portalOnCameraPlaceholder.transform.rotation;
            }
            else
            {
                if (indicatorM.color == Color.red | Scenario_No == 3)
                {
                    animationObjectPortal.transform.localPosition = InPortalPos;
                    animationObjectPortal.transform.localEulerAngles = InPortalRotation;
                }
            }
            
        }
    }

    //-------------------------------------------------------------------------------Hardware Configuration------------------------------------------------------------------------

    void GenerateFileName()
    {
        currentFileName = "" + DateTime.Now.ToString("HH''mm''ss");

    }

    public void ViconStart()
    {
        GenerateFileName();
        int packetNum = UnityEngine.Random.Range(10000, 99999);
        string num = packetNum.ToString();

        // Need to change the path in here...
        xml = $@"<?xml version=""1.0"" encoding=""UTF - 8"" standalone=""no""?><CaptureStart><Name VALUE=""{currentFileName}""/><Notes VALUE=""notesval""/><Description VALUE=""{num}""/><DatabasePath VALUE=""C:\JimNexus\JimTesting\Jim\JimAnimTesting\""/><Delay VALUE=""0""/><PacketID VALUE=""{num}""/></CaptureStart>";
        SendString(xml);

    }

    public void ViconStop()
    {
        int packetNum = UnityEngine.Random.Range(10000, 99999);
        string num = packetNum.ToString();
        xml = $@"<?xml version=""1.0"" encoding=""UTF - 8"" standalone=""no""?><CaptureStop><Name VALUE=""{currentFileName}""/><DatabasePath VALUE=""C:\JimNexus\JimTesting\Jim\JimAnimTesting\""/><Delay VALUE=""0""/><PacketID VALUE=""{num}""/></CaptureStop>";
        SendString(xml);

    }

    void SendString(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);


            client.Send(data, data.Length, remoteEndPoint);

        }
        catch (Exception err)
        {
        }
    }
}

public enum Options // your custom enumeration
{
    MagicHand,
    ExtendedHand,
    Portal,
    HapticPortal
};
