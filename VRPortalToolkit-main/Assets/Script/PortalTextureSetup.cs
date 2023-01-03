using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTextureSetup : MonoBehaviour
{
    public Camera portalCamera;

    public RenderTexture cameraTexture;

    public bool resetCamera = false;

    // Start is called before the first frame update
    void Update()
    {
        if (resetCamera)
        {

            if (portalCamera.targetTexture != null)
            {
                portalCamera.targetTexture.Release();
            }

            cameraTexture.width = Screen.width;
            cameraTexture.height = Screen.height;

            resetCamera = false;
        }
    }

}
