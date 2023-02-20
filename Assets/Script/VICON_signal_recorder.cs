using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class VICON_signal_recorder : MonoBehaviour
{
    public GameObject VICON_object;

    public string fileName = "Record No XX - DIRECTION";
    public bool startRecording = false;

    private List<Vector3> posVector = new List<Vector3>();
    private bool timerCountDown = true;

    IEnumerator countDown()
    {
        while (true)
        {
            print("1");
            while (startRecording)
            {
                print("2");
                timerCountDown = true;
                yield return new WaitForSeconds(10f);

                print("3");
                timerCountDown = false;
                break;
            }

            print("4");
            yield return new WaitForSeconds(3f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(countDown());
    }

    // Update is called once per frame
    void Update()
    {
        if (startRecording & timerCountDown)
        {
            posVector.Add(VICON_object.transform.position);
        }

        if (!timerCountDown)
        {
            startRecording = false;
            saveToLocal();
            posVector = new List<Vector3>();

            timerCountDown = true;
        }
    }

    private void saveToLocal()
    {
        string filePath = "data/" + fileName + ".txt";

        StreamWriter sw = new StreamWriter(filePath);

        foreach (Vector3 item in posVector)
        {
            sw.WriteLine(item.ToString("F8"));
        }

        sw.Close();
    }
}
