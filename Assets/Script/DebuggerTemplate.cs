using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        sliderReference = PPR.Touch_Point;
        robotEndEffector = PPR.TCP_Center;
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

        Keyframe keyframeX1 = new Keyframe(clip_flying.frameRate * 1/60, -0.793f);
        Keyframe keyframeY1 = new Keyframe(clip_flying.frameRate * 1/60, 0.398f);
        Keyframe keyframeZ1 = new Keyframe(clip_flying.frameRate * 1/60, 0.48f);

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
}

