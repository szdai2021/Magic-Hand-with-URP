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

    }
}