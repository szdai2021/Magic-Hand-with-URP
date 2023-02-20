using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridManager : MonoBehaviour
{
    public float xStartPos;
    public float yStartPos;
    public float zStartPos;

    public int columnNumber;
    public int rowNumber;

    public float xSpace;
    public float ySpace;
    public float zSpace;

    public string fixCoord;
    public Vector3 localScale;

    public GameObject gridElement;
    public Material gridMaterial;

    public bool existingGrid = false;

    public void createGrid()
    {
        if (!existingGrid)
        {
            switch (fixCoord)
            {
                case "x":
                    for (int i = 0; i < columnNumber * rowNumber; i++)
                    {
                        GameObject g = Instantiate(gridElement, Vector3.zero, Quaternion.identity);
                        g.transform.SetParent(this.transform);
                        g.transform.localPosition = new Vector3(xStartPos, yStartPos + (ySpace * (i % columnNumber)), zStartPos + (zSpace * (i / columnNumber)));
                        g.transform.localScale = localScale;
                        g.transform.GetComponent<MeshRenderer>().material = gridMaterial;
                    }
                    break;
                case "y":
                    for (int i = 0; i < columnNumber * rowNumber; i++)
                    {
                        GameObject g = Instantiate(gridElement, Vector3.zero, Quaternion.identity);
                        g.transform.SetParent(this.transform);
                        g.transform.localPosition = new Vector3(xStartPos + (xSpace * (i % columnNumber)), yStartPos, zStartPos + (zSpace * (i / columnNumber)));
                        g.transform.localScale = localScale;
                        g.transform.GetComponent<MeshRenderer>().material = gridMaterial;
                    }
                    break;
                case "z":
                    for (int i = 0; i < columnNumber * rowNumber; i++)
                    {
                        GameObject g = Instantiate(gridElement, Vector3.zero, Quaternion.identity);
                        g.transform.SetParent(this.transform);
                        g.transform.localPosition = new Vector3(xStartPos + (xSpace * (i % columnNumber)), yStartPos + (ySpace * (i / columnNumber)), zStartPos);
                        g.transform.localScale = localScale;
                        g.transform.GetComponent<MeshRenderer>().material = gridMaterial;
                    }
                    break;
                default:
                    break;
            }

            existingGrid = true;
        }
    }

    public void destroyGrid()
    {
        if (existingGrid)
        {
            foreach (Transform child in this.transform)
            {
                DestroyImmediate(child.gameObject);
            }

            existingGrid = false;
        }
    }
}
