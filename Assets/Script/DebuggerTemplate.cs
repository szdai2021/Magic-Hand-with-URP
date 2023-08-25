using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class DebuggerTemplate : MonoBehaviour
{
    public GameObject portal1;
    public GameObject portal2;

    public GameObject portal1PlaceHolder;
    public GameObject portal2PlaceHolder;

    public GameObject target;

    public PhysicalPropReference PPR;

    private GameObject sliderReference;
    private GameObject robotEndEffector;

    public GameObject positionInUnity;
    public UnityClient unityClient;

    public Collider robotRange;

    public Animator animator;
    public AnimationClip clip_idle;
    public AnimationClip clip_flying;
    public int frameIndex = 0;
    public GameObject animationObject;

    public GameObject p1;
    public GameObject p2;

    public GameObject unityCoodP1;
    public GameObject unityCoodP2;

    private bool safetyTestFlag = false;

    private bool tenSecondsPause = false;

    public GameObject unityObjectTarget;


    IEnumerator robotSafetyTestDelayer()
    {
        while (true)
        {
            if (safetyTestFlag)
            {
                sliderReference.transform.position = unityCoodP1.transform.position;

                Vector3 referencePos1 = unityClient.convertUnityCoord2RobotCoord(robotEndEffector.transform.position);

                unityClient.customMove(referencePos1.x, referencePos1.y, referencePos1.z, -0.6, 1.47, 0.62, movementType: 1, interruptible: 1, radius: 0.05f);

                yield return new WaitForSeconds(3f);

                sliderReference.transform.position = unityCoodP2.transform.position;

                Vector3 referencePos2 = unityClient.convertUnityCoord2RobotCoord(robotEndEffector.transform.position);

                unityClient.customMove(referencePos2.x, referencePos2.y, referencePos2.z, -0.6, 1.47, 0.62, movementType: 1, interruptible: 1, radius: 0.05f);

                print(Vector3.Distance(referencePos1, referencePos2).ToString("F7"));

                safetyTestFlag = false;
            }
            else
            {
                yield return new WaitForSeconds(3f);
            }
        }
    }

    IEnumerator tenSecondsPauseDelayer()
    {
        while (true)
        {
            if (tenSecondsPause)
            {
                yield return new WaitForSeconds(10f);

                EditorApplication.isPaused = true;

                tenSecondsPause = false;
            }
            else
            {
                yield return new WaitForSeconds(3f);
            }
        }
    }

    private void Start()
    {
        sliderReference = PPR.Touch_Point;
        robotEndEffector = PPR.TCP_Center;

        StartCoroutine(robotSafetyTestDelayer());
        StartCoroutine(tenSecondsPauseDelayer());
    }

    public void testRelativePosRotChange()
    {
        GameObject newObj = Instantiate(target);

        //(newObj.transform.position, newObj.transform.rotation) = MagicHandControl.getTargetPosRot(portal1.transform, portal2.transform, target.transform);

        GameObject globalReference1 = GameObject.FindGameObjectsWithTag("GlobalReference")[0];
        GameObject globalReference2 = GameObject.FindGameObjectsWithTag("GlobalReference")[1];
        GameObject globalReference3 = GameObject.FindGameObjectsWithTag("GlobalReference")[2];

        globalReference1.transform.position = portal2PlaceHolder.transform.position;
        globalReference1.transform.rotation = portal2PlaceHolder.transform.rotation;
        globalReference1.transform.localScale = portal2PlaceHolder.transform.localScale;

        globalReference3.transform.position = target.transform.position;
        globalReference3.transform.rotation = target.transform.rotation;
        globalReference3.transform.localScale = target.transform.localScale;

        globalReference3.transform.SetParent(globalReference2.transform);

        globalReference2.transform.position = globalReference1.transform.position;
        globalReference2.transform.rotation = globalReference1.transform.rotation;
        globalReference2.transform.localScale = globalReference1.transform.localScale;

        newObj.transform.position = globalReference3.transform.position;
        newObj.transform.rotation = globalReference3.transform.rotation;
    }

    public void robotRangeTest()
    {
        if (robotRange.bounds.Contains(positionInUnity.transform.position))
        {
            sliderReference.transform.position = positionInUnity.transform.position;

            Vector3 newPos = unityClient.convertUnityCoord2RobotCoord(robotEndEffector.transform.position);

            unityClient.customMove(newPos.x, newPos.y, newPos.z, -0.6, 1.47, 0.62, movementType: 0);
        }
    }

    public void reConnectToServer()
    {
        unityClient.setupServerConnection();
    }

    public void animationTest()
    {
        // Get the position of the GameObject
        Vector3 currentPosition = animationObject.transform.position;

        // Update the keyframe at the specified frame index with the current position
        Keyframe keyframeX = new Keyframe(clip_flying.frameRate * 0, currentPosition.x);
        Keyframe keyframeY = new Keyframe(clip_flying.frameRate * 0, currentPosition.y);
        Keyframe keyframeZ = new Keyframe(clip_flying.frameRate * 0, currentPosition.z);

        Keyframe keyframeX1 = new Keyframe(clip_flying.frameRate * 1 / 60, -0.793f);
        Keyframe keyframeY1 = new Keyframe(clip_flying.frameRate * 1 / 60, 0.398f);
        Keyframe keyframeZ1 = new Keyframe(clip_flying.frameRate * 1 / 60, 0.48f);

        AnimationCurve curveX = new AnimationCurve(keyframeX, keyframeX1);
        AnimationCurve curveY = new AnimationCurve(keyframeY, keyframeY1);
        AnimationCurve curveZ = new AnimationCurve(keyframeZ, keyframeZ1);

        clip_flying.SetCurve(animationObject.name, typeof(Transform), "localPosition.x", curveX);
        clip_flying.SetCurve(animationObject.name, typeof(Transform), "localPosition.y", curveY);
        clip_flying.SetCurve(animationObject.name, typeof(Transform), "localPosition.z", curveZ);
    }

    public void startAnimation()
    {
        animator.SetTrigger("start flying");
    }

    public void testRobotPin()
    {
        unityClient.testRobotPin();
    }

    public void speedlTest1()
    {
        Vector3 referencePos1 = unityClient.convertUnityCoord2RobotCoord(p1.transform.position);
        Vector3 referencePos2 = unityClient.convertUnityCoord2RobotCoord(p2.transform.position);

        float ax = referencePos1.x - referencePos2.x;
        float ay = referencePos1.y - referencePos2.y;
        float az = referencePos1.z - referencePos2.z;

        float norm = Mathf.Sqrt(ax * ax + ay * ay + az * az);

        unityClient.customMove(ax / norm, ay / norm, az / norm, -0.6, 1.47, 0.62, speed: 0.01, acc: 0.15f, movementType: 4); // strange speedl behaviour

    }

    public void speedlTest2()
    {
        Vector3 referencePos1 = unityClient.convertUnityCoord2RobotCoord(p2.transform.position);
        Vector3 referencePos2 = unityClient.convertUnityCoord2RobotCoord(p1.transform.position);

        float ax = referencePos1.x - referencePos2.x;
        float ay = referencePos1.y - referencePos2.y;
        float az = referencePos1.z - referencePos2.z;

        float norm = Mathf.Sqrt(ax * ax + ay * ay + az * az);

        unityClient.customMove(ax / norm, ay / norm, az / norm, -0.6, 1.47, 0.62, speed: 0.01, acc: 0.15f, movementType: 4); // strange speedl behaviour
    }

    public void safetyDistanceTest()
    {
        safetyTestFlag = true;
    }

    public void tenSecondsPauseStart()
    {
        tenSecondsPause = true;
    }

    public void robotPosSyncTest()
    {
        sliderReference.transform.position = unityObjectTarget.transform.position;

        Vector3 newPos = unityClient.convertUnityCoord2RobotCoord(robotEndEffector.transform.position);

        unityClient.customMove(newPos.x, newPos.y, newPos.z, -0.6, 1.47, 0.62, movementType: 0);
    }

    public void saveTransM()
    {
        // Define the path to the text file
        string filePath = "transMatrixConfig.txt";

        // Create or overwrite the file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // Write the matrix to the file
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    writer.Write(unityClient.transMatrix[i, j]);
                    if (j < 4 - 1)
                    {
                        writer.Write("\t"); // Use a tab to separate values
                    }
                }
                writer.WriteLine(); // Move to the next line
            }
        }
    }

    public void loadSavedTranM()
    {
        // Define the path to the text file
        string filePath = "transMatrixConfig.txt";

        // Check if the file exists
        if (File.Exists(filePath))
        {
            // Read all lines from the file
            string[] lines = File.ReadAllLines(filePath);

            // Check if the file contains exactly 4 lines
            if (lines.Length == 4)
            {
                double[,] matrix = new double[4, 4];

                // Parse and populate the matrix from the lines
                for (int i = 0; i < 4; i++)
                {
                    string[] values = lines[i].Split('\t');
                    // Check if each line contains exactly 4 values
                    if (values.Length == 4)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            if (double.TryParse(values[j], out double value))
                            {
                                matrix[i, j] = value;
                            }
                            else
                            {
                                print("Error parsing value at row " + (i + 1) + ", column " + (j + 1));
                            }
                        }
                    }
                    else
                    {
                        print("Invalid number of values in line " + (i + 1));
                        break;
                    }
                }

                // Create a Unity Matrix4x4 using the loaded values
                unityClient.transMatrix = new Matrix4x4(
                    new Vector4((float)matrix[0, 0], (float)matrix[1, 0], (float)matrix[2, 0], (float)matrix[3, 0]),
                    new Vector4((float)matrix[0, 1], (float)matrix[1, 1], (float)matrix[2, 1], (float)matrix[3, 1]),
                    new Vector4((float)matrix[0, 2], (float)matrix[1, 2], (float)matrix[2, 2], (float)matrix[3, 2]),
                    new Vector4((float)matrix[0, 3], (float)matrix[1, 3], (float)matrix[2, 3], (float)matrix[3, 3])
                );
            }
            else
            {
                print("The file must contain exactly 4 lines.");
            }
        }
        else
        {
            print("File not found: " + filePath);
        }
    }

}

