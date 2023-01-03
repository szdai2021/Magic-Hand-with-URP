using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraPosFollowUp : MonoBehaviour
{
    public GameObject targetCamera;
    public Vector3 cameraOffset;

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = targetCamera.transform.position + cameraOffset;

        this.gameObject.transform.rotation = targetCamera.transform.rotation;
    }
}
