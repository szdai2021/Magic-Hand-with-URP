using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionWithHand : MonoBehaviour
{
    private bool startPointTouched = false;
    private bool dataPointTouched = false;

    private Collider otherCollider;
    IEnumerator sendTriggerSignalAfterSeconds()
    {
        while (true)
        {
            if (startPointTouched)
            {
                yield return new WaitForSeconds(1.5f);

                MagicHandControl.startPointTouched = true;
            }

            if (dataPointTouched)
            {
                yield return new WaitForSeconds(1.5f);

                MagicHandControl.dataPointTouched = true;

                MagicHandControl.dataPointIndex = otherCollider.transform.GetSiblingIndex();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void Start()
    {
        StartCoroutine(sendTriggerSignalAfterSeconds());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "startPoint")
        {
            startPointTouched = true;

            print("start point touched");

            //MagicHandControl.startPointTouched = true;
        }
        else if (other.gameObject.tag == "DataPoint")
        {
            dataPointTouched = true;

            print("data point touched");

            //MagicHandControl.dataPointTouched = true;

            //MagicHandControl.dataPointIndex = this.transform.GetSiblingIndex();
        }

        otherCollider = other;
    }

    private void OnTriggerExit(Collider other)
    {
        startPointTouched = false;
        dataPointTouched = false;

        MagicHandControl.startPointTouched = false;
        MagicHandControl.dataPointTouched = false;
    }
}

