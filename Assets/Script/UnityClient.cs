using System.Collections;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;
using System.Threading;

public class UnityClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private StreamReader inChannel;
    private StreamWriter outChannel;

    public string host_ip = "localhost";
    public int host_port = 27;

    private double prev_x, prev_y, prev_z;

    public bool receiveFlag = false;
    public float scale = 3f;

    public string fromRobot;

    private Thread getSpeedInfo;

    public bool robotStopped = true;

    private Matrix4x4 transMatrix = Matrix4x4.zero;
    private Matrix4x4 unityCoordMatrix = Matrix4x4.zero;
    private Matrix4x4 robotCoordMatrix = Matrix4x4.zero;

    private Vector3 robotCoordTemp;

    public PhysicalPropReference PPR;
    public bool startCalibration = true;

    public Vector3 posVector = new Vector3(0.2f, 0.2f, 0.2f);
    public Vector3 rotVector = new Vector3(-0.6f, 1.47f, 0.62f);
    public float[] jointAngleBias = { 0, 0, 0, 0, 0, 0 };
    public int moveType = 1;

    private double jointAngleBias_6 = -0.15;
    private GameObject virtualEndEffector;

    public bool homePosition = false;
    public float linearVoltageMax = 5;

    void Start()
    {
        client = new TcpClient(host_ip, host_port);
        //Debug.Log("Connected to relay server");

        stream = client.GetStream();
        inChannel = new StreamReader(client.GetStream());
        outChannel = new StreamWriter(client.GetStream());

        virtualEndEffector = PPR.TCP_Center_Tracked;

        initialPos();

        getSpeedInfo = new Thread(getInfo);

        getSpeedInfo.Start(); 
        
        StartCoroutine(RobotCoordUnityCoordCalibration());
    }

    IEnumerator RobotCoordUnityCoordCalibration()
    {
        // wait for 5 seconds before the calibration
        yield return new WaitForSeconds(5f);

        while (true)
        {
            while (startCalibration)
            {
                // send command to robot for the first movement and wait for 5 seconds
                customMove(0.2f, 0.2f, 0.2f, -0.6f, 1.47f, 0.62f, movementType: 1, interruptible: 0);
                yield return new WaitForSeconds(1f);
                customMove(0.2f, 0.2f, 0.2f, -0.6f, 1.47f, 0.62f, movementType: 1, interruptible: 0);
                yield return new WaitForSeconds(1f);
                customMove(0.2f, 0.2f, 0.2f, -0.6f, 1.47f, 0.62f, movementType: 1, interruptible: 0);
                yield return new WaitForSeconds(3f);
                // record unity coord and robot coord in matrix

                //Debug.Log(robotCoordTemp.ToString("f6") + " " + virtualEndEffector.transform.position.ToString("f6"));

                robotCoordMatrix[0, 0] = robotCoordTemp.x;
                robotCoordMatrix[1, 0] = robotCoordTemp.y;
                robotCoordMatrix[2, 0] = robotCoordTemp.z;
                robotCoordMatrix[3, 0] = 1f;

                unityCoordMatrix[0, 0] = virtualEndEffector.transform.position.x;
                unityCoordMatrix[1, 0] = virtualEndEffector.transform.position.y;
                unityCoordMatrix[2, 0] = virtualEndEffector.transform.position.z;
                unityCoordMatrix[3, 0] = 1f;

                // send command to robot for the second movement and wait for 5 seconds
                customMove(0.35f, 0.1f, 0.15f, -0.6f, 1.47f, 0.62f, movementType: 0, interruptible: 0);
                yield return new WaitForSeconds(1f);
                customMove(0.35f, 0.1f, 0.15f, -0.6f, 1.47f, 0.62f, movementType: 0, interruptible: 0);
                yield return new WaitForSeconds(1f);
                customMove(0.35f, 0.1f, 0.15f, -0.6f, 1.47f, 0.62f, movementType: 0, interruptible: 0);
                yield return new WaitForSeconds(3f);
                // record unity coord and robot coord in matrix

                //Debug.Log(robotCoordTemp + " " + virtualEndEffector.transform.position);

                robotCoordMatrix[0, 1] = robotCoordTemp.x;
                robotCoordMatrix[1, 1] = robotCoordTemp.y;
                robotCoordMatrix[2, 1] = robotCoordTemp.z;
                robotCoordMatrix[3, 1] = 1f;

                unityCoordMatrix[0, 1] = virtualEndEffector.transform.position.x;
                unityCoordMatrix[1, 1] = virtualEndEffector.transform.position.y;
                unityCoordMatrix[2, 1] = virtualEndEffector.transform.position.z;
                unityCoordMatrix[3, 1] = 1f;

                // send command to robot for the third movement and wait for 5 seconds
                customMove(0f, 0.45f, 0.3f, -0.6f, 1.47f, 0.62f, movementType: 0, interruptible: 0);
                yield return new WaitForSeconds(1f);
                customMove(0f, 0.45f, 0.3f, -0.6f, 1.47f, 0.62f, movementType: 0, interruptible: 0);
                yield return new WaitForSeconds(1f);
                customMove(0f, 0.45f, 0.3f, -0.6f, 1.47f, 0.62f, movementType: 0, interruptible: 0);
                yield return new WaitForSeconds(3f);
                // record unity coord and robot coord in matrix

                //Debug.Log(robotCoordTemp + " " + virtualEndEffector.transform.position);

                robotCoordMatrix[0, 2] = robotCoordTemp.x;
                robotCoordMatrix[1, 2] = robotCoordTemp.y;
                robotCoordMatrix[2, 2] = robotCoordTemp.z;
                robotCoordMatrix[3, 2] = 1f;

                unityCoordMatrix[0, 2] = virtualEndEffector.transform.position.x;
                unityCoordMatrix[1, 2] = virtualEndEffector.transform.position.y;
                unityCoordMatrix[2, 2] = virtualEndEffector.transform.position.z;
                unityCoordMatrix[3, 2] = 1f;

                // send command to robot for the fourth movement and wait for 5 seconds
                customMove(0.3f, 0f, 0.35f, -0.6f, 1.47f, 0.62f, movementType: 0, interruptible: 0);
                yield return new WaitForSeconds(1f);
                customMove(0.3f, 0f, 0.35f, -0.6f, 1.47f, 0.62f, movementType: 0, interruptible: 0);
                yield return new WaitForSeconds(1f);
                customMove(0.3f, 0f, 0.35f, -0.6f, 1.47f, 0.62f, movementType: 0, interruptible: 0);
                yield return new WaitForSeconds(3f);
                // record unity coord and robot coord in matrix

                //Debug.Log(robotCoordTemp + " " + virtualEndEffector.transform.position);

                robotCoordMatrix[0, 3] = robotCoordTemp.x;
                robotCoordMatrix[1, 3] = robotCoordTemp.y;
                robotCoordMatrix[2, 3] = robotCoordTemp.z;
                robotCoordMatrix[3, 3] = 1f;

                unityCoordMatrix[0, 3] = virtualEndEffector.transform.position.x;
                unityCoordMatrix[1, 3] = virtualEndEffector.transform.position.y;
                unityCoordMatrix[2, 3] = virtualEndEffector.transform.position.z;
                unityCoordMatrix[3, 3] = 1f;

                transMatrix = robotCoordMatrix * unityCoordMatrix.inverse;

                //print(transMatrix);

                customMove(-2.95435f, -1.64447f, 2.18844f, -0.564082f, 0.984871f, -7.98365f, movementType: 3, interruptible: 0);

                startCalibration = false;
            }
            yield return new WaitForSeconds(5f);
        }
    }

    public void setupServerConnection()
    {
        client = new TcpClient(host_ip, host_port);

        stream = client.GetStream();
        inChannel = new StreamReader(client.GetStream());
        outChannel = new StreamWriter(client.GetStream());
    }

    public void initialPos()
    {
        customMove(-2.95435f, -1.64447f, 2.18844f, -0.564082f, 0.984871f, -7.98365f, movementType: 3, interruptible: 0);
        homePosition = true;
    }

    public void customMove(double xi, double yi, double zi, double rxi, double ryi, double rzi,
        double acc = 0.3, double speed = 0.3, double btn_press = 0, double scenario = 0, bool speedAdopt = false,
        double angle1 = 0, double angle2 = 0, double angle3 = 0, double angle4 = 0, double angle5 = 0, double angle6 = 0,
        int movementType = 0, double extra1 = 0, double extra2 = 0, double extra3 = 0, double radius = 0, int interruptible = 1, float linearActuatorDistance = 2.5f) 
        // movementType 0: jointspace linear;
        // Type 1: toolspace linear;
        // Type 2: circular;
        // Type 3: jointspace linear by joint pos;
        // Type 4: speedl;
        // Type 5: gripper only;
    {
        if (linearActuatorDistance > linearVoltageMax)
        {
            linearActuatorDistance = linearVoltageMax;
        }
        else if (linearActuatorDistance < 0)
        {
            linearActuatorDistance = 2.5f;
        }

        string cmd = packCMD(xi, yi, zi, rxi, ryi, rzi, acc, speed, btn_press, scenario, speedAdopt, angle1, angle2, angle3, angle4, angle5, angle6 + jointAngleBias_6, movementType, extra1, extra2, extra3, radius, interruptible, linearActuatorDistance: linearActuatorDistance);

        if (zi < 0.15 & movementType < 3)
        {
            print("Risk Alert: Robot position is too low");
        }
        else
        {
            outChannel.Write(cmd);
            outChannel.Flush();
            receiveFlag = false;
            homePosition = false;
        }
    }

    public void changeGripperDiameter(float d)
    {
        string cmd = packCMD(movementType:5, gripperEnable: 1, gripperDiameter: d);
        outChannel.Write(cmd);
        outChannel.Flush();
        receiveFlag = false;
    }

    private string packCMD(double Pos_x = 0.2, double Pos_y = 0.2, double Pos_z = 0.07, double Rot_x = -0.6, double Rot_y = 1.47, double Rot_z = 0.62, 
        double acc = 0.3, double speed = 0.3, double btn_press = 0, double scenario = 0, bool speedAdopt = false,
        double angle1 = 0, double angle2 = 0, double angle3 = 0, double angle4 = 0, double angle5 = 0, double angle6 = 0,
        int movementType = 0, double extra1 = 0, double extra2 = 0, double extra3 = 0, double radius = 0, int interruptible = 1, int gripperEnable = 0, float gripperDiameter = 20, float linearActuatorDistance = 0) // movementType 0: jointspace linear; Type 1: toolspace linear; Type 2: circular; Type 3: jointspace linear by joint pos
    {
        if (speedAdopt)
        {
            var distance = Vector3.Distance(new Vector3((float)prev_x, (float)prev_y, (float)prev_z), new Vector3((float)Pos_x, (float)Pos_y, (float)Pos_z));

            acc = Math.Log(1 + distance) * scale + 0.3;
            speed = Math.Log(1 + distance) * scale + 0.3;
        }

        string cmd = "(" + Pos_x + "," + Pos_y + "," + Pos_z + ","
               + Rot_x + "," + Rot_y + "," + Rot_z + ","
               + acc + "," + speed + "," + btn_press + "," + scenario + "," + 
               angle1 + "," + angle2 + "," + angle3 + "," + angle4 + "," + angle5 + "," + angle6 + "," + 
               movementType + "," + extra1 + "," + extra2 + "," + extra3 + "," + radius + "," + interruptible + "," + gripperEnable + "," + gripperDiameter +"," + linearActuatorDistance + ")";

        prev_x = Pos_x;
        prev_y = Pos_y;
        prev_z = Pos_z;

        robotStopped = false;

        return cmd;
    }

    private void getInfo()
    {
        while (true)
        {
            fromRobot = inChannel.ReadLine();
        }
    }

    public void stopRobot()
    {
        string cmd = packCMD(scenario: 2);
        outChannel.Write(cmd);
        outChannel.Flush();
        receiveFlag = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            customMove(posVector.x, posVector.y, posVector.z, rotVector.x, rotVector.y, rotVector.z, angle6:jointAngleBias[5], movementType: moveType, interruptible: 0);
        }

        if (Input.GetKeyDown("i"))
        {
            initialPos();
        }

        if (fromRobot.StartsWith("R"))
        {
            DateTime dt2 = DateTime.Now;
            //Debug.Log("Returned Time: " + dt2);
            //Debug.Log("Difference: " + (dt2 - controlmanager.dt).TotalMilliseconds.ToString("F6")+"ms");
        }
        else if (fromRobot.StartsWith("p"))
        {
            var items = fromRobot.Split(new string[] { "p", "[", "]", "," }, StringSplitOptions.RemoveEmptyEntries);
            robotCoordTemp[0] = float.Parse(items[0]);
            robotCoordTemp[1] = float.Parse(items[1]);
            robotCoordTemp[2] = float.Parse(items[2]);
        }
        else if (fromRobot.StartsWith("i"))
        {
            var items = fromRobot.Split(new string[] { "i", "p", "[", "]", "," }, StringSplitOptions.RemoveEmptyEntries);

            float sp = float.Parse(items[0]) * float.Parse(items[0]) + float.Parse(items[1]) * float.Parse(items[1]) + float.Parse(items[2]) * float.Parse(items[2]);

            robotStopped = sp < 0.0001;
        }

        //getSpeedInfo.Abort();
        //getSpeedInfo = new Thread(getInfo);
        //getSpeedInfo.Start();
    }

    public Vector3 convertUnityCoord2RobotCoord(Vector3 p1)
    {
        Vector3 new_p = transMatrix.MultiplyPoint3x4(p1);

        //print(p1);
        //print(new_p);
        //print(transMatrix);

        return new_p;
    }

    public void testRobotPin()
    {
        customMove(-2.95435f, -1.64447f, 2.18844f, -0.564082f, 0.984871f, -7.98365f, movementType: 3, interruptible: 0, scenario: 5);
    }
}
