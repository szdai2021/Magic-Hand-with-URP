using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRPortalToolkit
{
    public class CellMatchThroughPortal : MonoBehaviour
    {
        public bool enable;
        public GameObject VRhand;

        public GameObject portalA, portalB;
        public VRHandControlGoGo control;

        public Collider box;
        public GameObject target;
        public Material[] materialList;

        private bool previous;
        private Vector3 position;

        public float scale;

        // Update is called once per frame
        void Update()
        {
            if (box.bounds.Contains(VRhand.transform.position))
            {
                target.GetComponent<MeshRenderer>().material = materialList[0]; // transparent
            }
            else
            {
                target.GetComponent<MeshRenderer>().material = materialList[1]; // normal
            }

            enable = control.gestureDetection & box.bounds.Contains(VRhand.transform.position);

            if (enable & !previous)
            {
                position = VRhand.transform.position;
            }

            if (enable)
            {
                this.transform.position += (VRhand.transform.position - position)*scale;

                position = VRhand.transform.position;
            }


            previous = enable;
        }
    }
}
