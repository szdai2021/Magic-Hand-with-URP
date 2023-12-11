using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRPortalToolkit
{
    public class leftToRight : MonoBehaviour
    {
        public GameObject leftHand;
        public GameObject rightHand;

        public bool enable = true;

        // Update is called once per frame
        void Update()
        {
            if (enable)
            {
                MatchChildrenTransformsRecursive(rightHand.transform, leftHand.transform);
            }
        }

        private void MatchChildrenTransformsRecursive(Transform source, Transform target)
        {
            foreach (Transform childA in source)
            {
                char[] charArray = childA.name.ToCharArray();
                charArray[childA.name.Length - 1] = 'l';
                string modifiedString = new string(charArray);

                Transform childB = target.Find(modifiedString);

                if (childB != null)
                {
                    if (modifiedString.Contains("thumb"))
                    {
                        // Match the local position and rotation of Object A's child to Object B's child
                        childA.localPosition = childB.localPosition;
                        childA.localRotation = new Quaternion(-childB.localRotation.x, childB.localRotation.y, -childB.localRotation.z, childB.localRotation.w);

                    }
                    else
                    {
                        // Match the local position and rotation of Object A's child to Object B's child
                        childA.localPosition = childB.localPosition;
                        //childA.localRotation = childB.localRotation;
                        childA.localRotation = new Quaternion(childB.localRotation.x, childB.localRotation.y, -childB.localRotation.z, childB.localRotation.w);
                    }
                    

                    // Recursively match the children of this child
                    MatchChildrenTransformsRecursive(childA, childB);
                }
                else
                {
                    Debug.LogWarning("Child " + childA.name + " not found in Object B.");
                }
            }
        }
    }
}
