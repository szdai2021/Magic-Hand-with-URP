using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRPortalToolkit
{
    public class HandProjectionOnPlane : MonoBehaviour
    {
        public GameObject planeNormalParent;
        public GameObject projectedPoint;

        public GameObject VRHand;
        public UnityClient unityClient;

        private Vector3 p1;
        private Vector3 p2;

        private Vector3 normal;

        private GameObject sliderReference;
        private GameObject robotEndEffector;

        public PhysicalPropReference PPR;

        public bool startFollowUp;

        private GameObject trackedEndEffector;

        private int skipFrameCounter = 0;

        public int skipThreshold = 9;
        public int interruptible = 0;

        public float a = 1.5f;
        public float v = 1f;

        public int moveType = 1;
        public Collider robotRange;

        private void Start()
        {
            p1 = planeNormalParent.transform.GetChild(0).position;
            p2 = planeNormalParent.transform.GetChild(1).position;

            normal = (p2 - p1).normalized;

            sliderReference = PPR.Touch_Point;
            robotEndEffector = PPR.TCP_Center;

            trackedEndEffector = PPR.TCP_Center_Tracked;
        }

        // Update is called once per frame
        void Update()
        {
            projectedPoint.transform.position = Vector3.ProjectOnPlane(VRHand.transform.position - p1, normal) + p1;

            sliderReference.transform.position = projectedPoint.transform.position;

            Vector3 referencePos1 = unityClient.convertUnityCoord2RobotCoord(robotEndEffector.transform.position);
            Vector3 referencePos2 = unityClient.convertUnityCoord2RobotCoord(trackedEndEffector.transform.position);

            if (startFollowUp & skipFrameCounter > skipThreshold & robotRange.bounds.Contains(robotEndEffector.transform.position))
            {
                //float ax = referencePos1.x - referencePos2.x;
                //float ay = referencePos1.y - referencePos2.y;
                //float az = referencePos1.z - referencePos2.z;

                //float norm = Mathf.Sqrt(ax * ax + ay * ay + az * az);

                //unityClient.customMove(ax / norm, ay / norm, az / norm, -0.6, 1.47, 0.62, speed: 0.01, acc: 0.15f, movementType: 4); // strange speedl behaviour

                unityClient.customMove(referencePos1.x, referencePos1.y, referencePos1.z, -0.6, 1.47, 0.62, movementType: moveType, interruptible: interruptible, radius: 0.05f);

                skipFrameCounter = 0;
            }

            skipFrameCounter++;
        }
    }
}
