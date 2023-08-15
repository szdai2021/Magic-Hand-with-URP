using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRPortalToolkit
{
    public class ScreenShot : MonoBehaviour
    {
        [SerializeField]
        private string path;
        [SerializeField]
        [Range(1, 5)]
        private int size = 1;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                path += "screenshot ";
                path += System.Guid.NewGuid().ToString() + ".png";

                ScreenCapture.CaptureScreenshot(path, size);
            }
        }
    }
}
