using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CopyMesh : MonoBehaviour
{
    public GameObject targetObject;

    // Update is called once per frame
    void Update()
    {
        //Resources.UnloadUnusedAssets(); //using this in the update loop will cause high CPU usage by GC.markDependencies
            
        //if (this.GetComponent<MeshFilter>())
        //{
        //    Mesh m = this.GetComponent<MeshFilter>().sharedMesh;
        //    GameObject.Destroy(m);
        //}

        this.transform.position = targetObject.transform.position;
        this.transform.rotation = targetObject.transform.rotation;
            
        Mesh mesh = new Mesh();
                
        targetObject.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);

        this.GetComponent<MeshFilter>().mesh = mesh;
    }
}

