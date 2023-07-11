using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using ViconDataStreamSDK.CSharp;


namespace UnityVicon
{
    public class RBScript : MonoBehaviour
    {
        public string ObjectName = "";
        public NewModdedDataStreamClient Client;

        private Quaternion m_LastGoodRotation;
        private Vector3 m_LastGoodPosition;
        private bool m_bHasCachedPose = false;

        private GameObject VICON_P1;
        private GameObject VICON_P2;

        private GameObject UNITY_P1;
        private GameObject UNITY_P2;

        //private float ratio = 0.002007f;
        private float ratio = 0.001f;

        private void Start()
        {
            VICON_P1 = GameObject.FindGameObjectsWithTag("VICON_P1")[0];
            VICON_P2 = GameObject.FindGameObjectsWithTag("VICON_P2")[1];

            UNITY_P1 = GameObject.FindGameObjectsWithTag("UNITY_P1")[0];
            UNITY_P2 = GameObject.FindGameObjectsWithTag("UNITY_P2")[1];
        }

        public RBScript()
        {
        }

        void Update()
        {
            //if (VICON_P1 != null & VICON_P2 != null & UNITY_P1 != null & UNITY_P2 != null)
            //{
            //    ratio = 0.001f * (Vector3.Distance(UNITY_P1.transform.position, UNITY_P2.transform.position)/Vector3.Distance(VICON_P1.transform.position, VICON_P2.transform.position));
            //}

            Output_GetSubjectRootSegmentName OGSRSN = Client.GetSubjectRootSegmentName(ObjectName);
            string SegRootName = OGSRSN.SegmentName;

            // UNITY-49 - Don't apply root motion to parent object
            Transform Root = transform;
            if (Root == null)
            {
                throw new Exception("fbx doesn't have root");
            }

            Output_GetSegmentLocalRotationQuaternion ORot = Client.GetSegmentRotation(ObjectName, SegRootName);
            Output_GetSegmentLocalTranslation OTran = Client.GetSegmentTranslation(ObjectName, SegRootName);

            if (ORot.Result == Result.Success && OTran.Result == Result.Success && !OTran.Occluded)
            {
                // Input data is in Vicon co-ordinate space; z-up, x-forward, rhs.
                // We need it in Unity space, y-up, z-forward lhs
                //           Vicon Unity
                // forward    x     z
                // up         z     y
                // right     -y     x
                // See https://gamedev.stackexchange.com/questions/157946/converting-a-quaternion-in-a-right-to-left-handed-coordinate-system

                Root.localRotation = new Quaternion((float)ORot.Rotation[1], -(float)ORot.Rotation[2], -(float)ORot.Rotation[0], (float)ORot.Rotation[3]);
                Root.localPosition = new Vector3(-(float)OTran.Translation[1] * ratio, (float)OTran.Translation[2] * ratio, (float)OTran.Translation[0] * ratio);

                m_LastGoodPosition = Root.localPosition;
                m_LastGoodRotation = Root.localRotation;
                m_bHasCachedPose = true;
            }
            else
            {
                if (m_bHasCachedPose)
                {
                    Root.localRotation = m_LastGoodRotation;
                    Root.localPosition = m_LastGoodPosition;
                }
            }

        }
    } //end of program
}// end of namespace
