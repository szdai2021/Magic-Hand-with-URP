using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInContainer : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject containter;
    public Collider boundaryCheck;

    // Update is called once per frame
    void Update()
    {
        containter.GetComponent<MeshRenderer>().enabled = !boundaryCheck.bounds.Contains(mainCamera.transform.position);
    }
}

