using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRPortalToolkit
{
    public class DebuggerTemplate : MonoBehaviour
    {
        public GameObject portal1;
        public GameObject portal2;

        public GameObject portal1PlaceHolder;
        public GameObject portal2PlaceHolder;

        public GameObject target;

        public void testRelativePosRotChange()
        {
            GameObject newObj = Instantiate(target);

            //(newObj.transform.position, newObj.transform.rotation) = MagicHandControl.getTargetPosRot(portal1.transform, portal2.transform, target.transform);

            portal1PlaceHolder.transform.position = target.transform.position;
            portal1PlaceHolder.transform.rotation = target.transform.rotation;
            portal1PlaceHolder.transform.localScale = target.transform.localScale;

            portal2PlaceHolder.transform.localPosition = portal1PlaceHolder.transform.localPosition;
            portal2PlaceHolder.transform.localRotation = portal1PlaceHolder.transform.localRotation;
            portal2PlaceHolder.transform.localScale = portal1PlaceHolder.transform.localScale;

            newObj.transform.position = portal2PlaceHolder.transform.position;
            newObj.transform.rotation = portal2PlaceHolder.transform.rotation;
        }

    }
}
