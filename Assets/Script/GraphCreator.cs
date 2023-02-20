using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class GraphCreator : MonoBehaviour
{
    public string fileName; //Assets/Data/MINST.csv
    public GameObject Container;

    public int dimensionIndex1 = 1;
    public int dimensionIndex2 = 2;
    public int dimensionIndex3 = 3;

    public int stepSize = 1;

    public GameObject scatterPoint;
    public GameObject scatterParent;

    public bool existingScatter = false;

    public Material[] clasterMaterial;

    //private Matrix rawData = new Matrix(725, 18623);
    private Matrix rawData = new Matrix(5, 11122);

    private Vector2 x_range = new Vector2(0f, 0f);
    private Vector2 y_range = new Vector2(0f, 0f);
    private Vector2 z_range = new Vector2(0f, 0f);

    private void Start()
    {
        defineContainersize();

        openAndReadFile();

        create3DScatter();
    }

    public void defineContainersize()
    {
        x_range = new Vector2(Container.transform.position.x - Container.transform.localScale.x / 2, Container.transform.position.x + Container.transform.localScale.x / 2);
        y_range = new Vector2(Container.transform.position.y - Container.transform.localScale.y / 2, Container.transform.position.y + Container.transform.localScale.y / 2);
        z_range = new Vector2(Container.transform.position.z - Container.transform.localScale.z / 2, Container.transform.position.z + Container.transform.localScale.z / 2);
    }

    public void create3DScatter()
    {
        Vector2 x_in_range = new Vector2(Mathf.Max(x_range.x, x_range.y) - x_range.magnitude * 0.9f, Mathf.Min(x_range.x, x_range.y) + x_range.magnitude * 0.9f);
        Vector2 y_in_range = new Vector2(Mathf.Max(y_range.x, y_range.y) - y_range.magnitude * 0.9f, Mathf.Min(y_range.x, y_range.y) + y_range.magnitude * 0.9f);
        Vector2 z_in_range = new Vector2(Mathf.Max(z_range.x, z_range.y) - z_range.magnitude * 0.9f, Mathf.Min(z_range.x, z_range.y) + z_range.magnitude * 0.9f);

        for(int i = 1; i<rawData.Columns; i+= stepSize)
        {
            GameObject g = Instantiate(scatterPoint);
            print(rawData.getValue(dimensionIndex1, i));
            print(rawData.getValue(dimensionIndex2, i));
            print(rawData.getValue(dimensionIndex3, i));

            g.transform.position = new Vector3(
                            mapValueToNewRange(x_in_range.y, x_in_range.x, 8f, -8f, float.Parse(rawData.getValue(dimensionIndex1, i))),
                            mapValueToNewRange(y_in_range.y, y_in_range.x, 8f, -8f, float.Parse(rawData.getValue(dimensionIndex2, i))),
                            mapValueToNewRange(z_in_range.y, z_in_range.x, 8f, -8f, float.Parse(rawData.getValue(dimensionIndex3, i))));

            g.transform.SetParent(scatterParent.transform);

            string[] temp = rawData.GetRow(4);

            HashSet<string> uniqueStrings = new HashSet<string>(temp);

            int categoryIndex = Array.IndexOf(uniqueStrings.ToArray(), rawData.getValue(4, i));

            g.GetComponent<MeshRenderer>().material = clasterMaterial[categoryIndex];
        }

        existingScatter = true;
    }

    public void destroyScatter()
    {
        if (existingScatter)
        {
            foreach (Transform child in scatterParent.transform)
            {
                Destroy(child.gameObject);
            }

            existingScatter = false;
        }
    }

    public void openAndReadFile()
    {
        StreamReader reader = new StreamReader(fileName);

        int i = 0;
        while (!reader.EndOfStream & i < rawData.Columns)
        {
            string newLine = reader.ReadLine();

            int j = 0;
            foreach(string s in newLine.Split(','))
            {
                try
                {
                    rawData.setValue(j, i, s);
                }
                catch (FormatException)
                {
                    print(s);
                }

                j++;
            }

            i++;
        }

        reader.Close();
    }

    private float mapValueToNewRange(float Max, float Min, float localMax, float localMin, float p)
    {
        return (p - localMin) / (localMax - localMin) * (Max - Min) + Min;
    }
}

public class Matrix
{
    private readonly string[,] values;
    public int Rows { get; }
    public int Columns { get; }

    public Matrix(int rows, int columns)
    {
        values = new string[rows, columns];
        Rows = rows;
        Columns = columns;
    }

    public string getValue(int row, int column)
    {
        return values[row, column];
    }

    public void setValue(int row, int column, string value)
    {
        values[row, column] = value;
    }

    public string[] GetRow(int rowIndex)
    {
        if (rowIndex < 0 || rowIndex >= Rows)
        {
            throw new ArgumentOutOfRangeException(nameof(rowIndex), "Row index must be between 0 and the number of rows - 1.");
        }

        string[] row = new string[Columns];
        for (int i = 0; i < Columns; i++)
        {
            row[i] = values[rowIndex, i];
        }
        return row;
    }

    public string[] GetColumn(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= Columns)
        {
            throw new ArgumentOutOfRangeException(nameof(columnIndex), "Column index must be between 0 and the number of Columns - 1.");
        }

        string[] col = new string[Rows];
        for (int i = 0; i < Rows; i++)
        {
            col[i] = values[columnIndex, i];
        }
        return col;
    }
}


