using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gogo : MonoBehaviour
{
    public GameObject vrhand;
    public GameObject vrhandtwin;

    public GameObject gogoPoint1, gogoPoint2;
    public GameObject gogoCenter;

    // Update is called once per frame
    void Update()
    {
        vrhandtwin.transform.rotation = vrhand.transform.rotation;

        float offset = Vector3.Distance(gogoCenter.transform.position, this.transform.position);
        float range = Vector3.Distance(gogoPoint1.transform.position, gogoPoint2.transform.position);

        Vector3 normal = Vector3.Normalize(vrhand.transform.position - gogoCenter.transform.position);

        if (offset < range * 0.35)
        {
            //vrhandtwin.transform.position = vrhand.transform.position;

            vrhandtwin.transform.position = gogoCenter.transform.position + offset * normal;
        }
        else if (offset >= range * 0.35)
        {
            double r = offset / range;
            float new_r = Mathf.Exp((float)(4.5 * r)) - 4.49f;

            vrhandtwin.transform.position = gogoCenter.transform.position + (float)(range * 0.35) * normal + (offset - (float)(range * 0.35)) * normal * new_r;
        }
            
    }
}

