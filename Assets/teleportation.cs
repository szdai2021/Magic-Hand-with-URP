using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRPortalToolkit
{
    public class teleportation : MonoBehaviour
    {
        public GameObject prevPosition;

        public GameObject xrig;

        public bool enable;

        // Update is called once per frame
        void Update()
        {
            if (enable)
            {
                xrig.transform.position += this.transform.position - prevPosition.transform.position;

                enable = false;
            }
        }
    }
}
