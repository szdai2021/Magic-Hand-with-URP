using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRPortalToolkit
{
    public class scaleChange : MonoBehaviour
    {
        public GameObject controllarA, controllerB;

        public bool enable = false;

        private float distanceReference;
        private bool prev_enable = false;

        // Update is called once per frame
        void Update()
        {

            if (enable)
            {
                if (!prev_enable)
                {
                    distanceReference = Vector3.Distance(controllarA.transform.position, controllerB.transform.position);
                }

                float newDistance = Vector3.Distance(controllarA.transform.position, controllerB.transform.position);

                this.transform.localScale = new Vector3(newDistance/ distanceReference, newDistance / distanceReference, newDistance / distanceReference);
            }

            prev_enable = enable;
        }
    }
}
