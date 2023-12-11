using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRPortalToolkit
{
    public class lockToHand : MonoBehaviour
    {
        public bool enable;
        public Collider box;
        public GameObject VRhand;
        public VRHandControlGoGo control;

        private bool previous;
        private Vector3 position;

        // Update is called once per frame
        void Update()
        {
            
            enable = control.gestureDetection & box.bounds.Contains(VRhand.transform.position);

            if (enable & !previous)
            {
                position = VRhand.transform.position;
            }

            if (enable)
            {
                this.transform.position += VRhand.transform.position - position;

                position = VRhand.transform.position;
            }


            previous = enable;
        }
    }
}
