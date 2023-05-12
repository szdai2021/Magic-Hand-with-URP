using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyAndCombineMesh : MonoBehaviour
{
    public GameObject targetObject;

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
        this.transform.position = targetObject.transform.position;
        this.transform.rotation = targetObject.transform.rotation;

        MeshFilter[] meshFilters = targetObject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].mesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            //meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);

        this.GetComponent<MeshFilter>().mesh = mesh;
    }
}

