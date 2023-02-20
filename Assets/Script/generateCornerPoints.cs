using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class generateCornerPoints : MonoBehaviour
{
    public GameObject container;

    public GameObject[] cornerPoints;

    // Update is called once per frame
    void Update()
    {
        cornerPoints[0].transform.localPosition = container.transform.localPosition + new Vector3((1) * container.transform.localScale.x / 2, 
                                                                                                (1) * container.transform.localScale.y / 2, 
                                                                                                (1) * container.transform.localScale.z / 2);
        cornerPoints[1].transform.localPosition = container.transform.localPosition + new Vector3((1) * container.transform.localScale.x / 2,
                                                                                                (1) * container.transform.localScale.y / 2,
                                                                                                (-1) * container.transform.localScale.z / 2);
        cornerPoints[2].transform.localPosition = container.transform.localPosition + new Vector3((1) * container.transform.localScale.x / 2,
                                                                                                (-1) * container.transform.localScale.y / 2,
                                                                                                (1) * container.transform.localScale.z / 2);
        cornerPoints[3].transform.localPosition = container.transform.localPosition + new Vector3((1) * container.transform.localScale.x / 2,
                                                                                                (-1) * container.transform.localScale.y / 2,
                                                                                                (-1) * container.transform.localScale.z / 2);
        cornerPoints[4].transform.localPosition = container.transform.localPosition + new Vector3((-1) * container.transform.localScale.x / 2,
                                                                                                (1) * container.transform.localScale.y / 2,
                                                                                                (1) * container.transform.localScale.z / 2);
        cornerPoints[5].transform.localPosition = container.transform.localPosition + new Vector3((-1) * container.transform.localScale.x / 2,
                                                                                                (1) * container.transform.localScale.y / 2,
                                                                                                (-1) * container.transform.localScale.z / 2);
        cornerPoints[6].transform.localPosition = container.transform.localPosition + new Vector3((-1) * container.transform.localScale.x / 2,
                                                                                                (-1) * container.transform.localScale.y / 2,
                                                                                                (1) * container.transform.localScale.z / 2);
        cornerPoints[7].transform.localPosition = container.transform.localPosition + new Vector3((-1) * container.transform.localScale.x / 2,
                                                                                                (-1) * container.transform.localScale.y / 2,
                                                                                                (-1) * container.transform.localScale.z / 2);

    }
}

