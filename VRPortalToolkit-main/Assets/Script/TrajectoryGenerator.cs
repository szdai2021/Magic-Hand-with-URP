using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class TrajectoryGenerator : MonoBehaviour
{
    [Header("GameObject Assignment")]
    public GameObject pointPrefb;
    public GameObject linePrefb;
    public GameObject scatterPoint;
    public GameObject plotParent;

    [Header("Settings")]
    public string sourceFile = "BTC-USD.csv";
    public int stepInt = 1;
    public int starterInt = 0;

    public float xScale = 1;
    public float yScale = 0.001f;

    [HideInInspector]
    public List<string> DateList = new List<string>();
    public List<float> PriceList = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        readFile();

        drewPlot();

        //this.transform.parent.gameObject.SetActive(false);
    }

    private void drewPlot()
    {
        GameObject newLine = Instantiate(linePrefb, new Vector3(0, 0, 0), Quaternion.identity);
        newLine.transform.SetParent(plotParent.transform);

        for (int i = starterInt; i < PriceList.Count; i+=stepInt)
        {
            float y = PriceList[i]*yScale;
            float x = i*xScale;
            GameObject newPoint = Instantiate(scatterPoint, new Vector3(0, 0, 0), Quaternion.identity);
            newPoint.transform.SetParent(plotParent.transform);
            newPoint.transform.localPosition = new Vector3(x, y, 0);
            newPoint.GetComponent<DataPointDetails>().dataIndex = i;

            GameObject newPointGizmos = Instantiate(pointPrefb, new Vector3(0, 0, 0), Quaternion.identity);
            newPointGizmos.transform.SetParent(newLine.transform);
            newPointGizmos.transform.localPosition = new Vector3(x, y, 0);
        }
    }

    private void readFile()
    {
        StreamReader strReader = new StreamReader("Assets/RawData/" + sourceFile);
        bool endOfFile = false;

        int counter = 0;
        while (!endOfFile)
        {
            string data_string = strReader.ReadLine();
            if (data_string == null)
            {
                endOfFile = true;
                break;
            }

            if(counter != 0)
            {
                string[] dataItems = data_string.Split(',');

                DateList.Add(dataItems[0]);
                PriceList.Add(float.Parse(dataItems[1]));
            }

            counter++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")) 
        {
            updateMainPlot();
        }
    }

    private void updateMainPlot()
    {
        foreach (Transform child in plotParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        GameObject newLine = Instantiate(linePrefb, new Vector3(0, 0, 0), Quaternion.identity);
        newLine.transform.SetParent(plotParent.transform);

        for (int i = starterInt; i < PriceList.Count; i += stepInt)
        {
            float y = PriceList[i] * yScale;
            float x = i * xScale;
            GameObject newPoint = Instantiate(scatterPoint, new Vector3(0, 0, 0), Quaternion.identity);
            newPoint.transform.SetParent(plotParent.transform);
            newPoint.transform.localPosition = new Vector3(x, y, 0);
            newPoint.GetComponent<DataPointDetails>().dataIndex = i;

            GameObject newPointGizmos = Instantiate(pointPrefb, new Vector3(0, 0, 0), Quaternion.identity);
            newPointGizmos.transform.SetParent(newLine.transform);
            newPointGizmos.transform.localPosition = new Vector3(x, y, 0);
        }
    }

}
