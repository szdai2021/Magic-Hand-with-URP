using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DW_IndivisualControl : MonoBehaviour
{
    public bool highlightFlag;

    public Material highlightMaterial;
    public Material normalMaterial;

    public GameObject displayedDW;

    public Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        if (highlightFlag)
        {
            displayedDW.GetComponent<MeshRenderer>().material = highlightMaterial;
        }
        else
        {
            displayedDW.GetComponent<MeshRenderer>().material = normalMaterial;
        }
    }
}
