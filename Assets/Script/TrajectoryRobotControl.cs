using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrajectoryRobotControl : MonoBehaviour
{
    public UnityClient unityClient;
    public TrajectoryGenerator trajectoryGenerator;
    public GameObject virtualEndEffector;

    public List<GameObject> dataPointList;

    private bool startFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(startTrejectoryMovementTest());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            startFlag = !startFlag;
        }
    }

    IEnumerator startTrejectoryMovementTest()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            while (startFlag)
            {

                /*
                foreach (GameObject g in dataPointList)
                {
                    Vector3 newPos = unityClient.convertUnityCoord2RobotCoord(g.transform.position);

                    float angle = getDataPointAngle(g);

                    unityClient.customMove(newPos.x, newPos.y, newPos.z, -0.6, 1.47, 0.62, movementType: 1, angle6: angle);

                    yield return new WaitForSeconds(1f);
                }
                */

                //move to the first point
                Vector3 newPos = unityClient.convertUnityCoord2RobotCoord(dataPointList[0].transform.position);

                float angle = 0;
                //float angle = getDataPointAngle(dataPointList[0]);

                unityClient.customMove(newPos.x, newPos.y, newPos.z, -0.6, 1.47, 0.62, movementType: 1, angle6: angle);

                yield return new WaitForSeconds(3f);

                Debug.Log(Vector3.Distance(virtualEndEffector.transform.position, dataPointList[dataPointList.Count - 1].transform.position));

                //follow up the rest of the data points
                while (Vector3.Distance(virtualEndEffector.transform.position, dataPointList[dataPointList.Count-1].transform.position) > 0.005)
                {
                    float minDistance = 1000;
                    int minIndex = 0;
                    for (int i = 0; i < dataPointList.Count; i++)
                    {
                        if (Vector3.Distance(dataPointList[i].transform.position, virtualEndEffector.transform.position) < minDistance)
                        {
                            minDistance = Vector3.Distance(dataPointList[i].transform.position, virtualEndEffector.transform.position);
                            minIndex = i;
                        }
                    }

                    if (virtualEndEffector.transform.position.x < dataPointList[minIndex].transform.position.x || virtualEndEffector.transform.position.y < dataPointList[minIndex].transform.position.y)
                    {
                        minIndex += 1;

                        if (minIndex >= dataPointList.Count)
                        {
                            minIndex = dataPointList.Count - 1;
                        }
                    }

                    //angle = getDataPointAngle(dataPointList[minIndex]);
                    angle = 0;

                    Vector3 p2 = unityClient.convertUnityCoord2RobotCoord(virtualEndEffector.transform.position);
                    Vector3 p1 = unityClient.convertUnityCoord2RobotCoord(dataPointList[minIndex].transform.position);

                    float ax = p1.x - p2.x;
                    float ay = p1.y - p2.y;
                    float az = p1.z - p2.z;

                    float norm = Mathf.Sqrt(ax * ax + ay * ay + az * az);

                    unityClient.customMove(ax / norm, ay / norm, az / norm, -0.6, 1.47, 0.62, speed: 0.001, acc: 0.03f, movementType: 4, angle6: angle);

                    yield return new WaitForSeconds(1f);
                }

                startFlag = false;
            }

            if (!startFlag)
            {
                unityClient.stopRobot();
            }

            yield return new WaitForSeconds(3f);
        }
        
    }

    private float getDataPointAngle(GameObject g)
    {
        float angle = 0f;
        int gradientRange = 5;
        int index = g.GetComponent<DataPointDetails>().dataIndex;

        float[] a = new float[gradientRange]; // year
        float[] b = new float[gradientRange]; // price

        float[] c = new float[gradientRange];
        float[] d = new float[gradientRange];

        for (int i = 0; i < gradientRange; i++)
        {
            a[i] = trajectoryGenerator.PriceList[index - gradientRange / 2 + i] / trajectoryGenerator.xScale;
            b[i] =  (index - gradientRange / 2 + i) / trajectoryGenerator.yScale;
            c[i] = a[i] * b[i];
            d[i] = a[i] * a[i];
        }

        float aa = ((a.Average() * b.Average()) - c.Average()) / (a.Average() * a.Average() - d.Average());

        angle = Mathf.Rad2Deg * Mathf.Atan(aa);

        return angle;
    }
}
