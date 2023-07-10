using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public ViconMixedRealityCalibration VMRC;
    public UnityClient UC;

    public float gripperDiameter = 40f;

    IEnumerator removeUnusedMesh()
    {
        // wait for 15 seconds before the calibration
        yield return new WaitForSeconds(10f);

        while (true)
        {
            Resources.UnloadUnusedAssets();

            yield return new WaitForSeconds(10f);
        }
    }

    private void Start()
    {
        StartCoroutine(removeUnusedMesh());
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r"))
        {
            VMRC.ApplyOffset();
        }

        if (Input.GetKeyDown("u"))
        {
            UC.changeGripperDiameter(gripperDiameter);
        }
    }
}
