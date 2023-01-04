using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRPortalToolkit
{
    public class CopyMesh : MonoBehaviour
    {
        public GameObject targetObject;

        // Update is called once per frame
        void Update()
        {
            this.transform.position = targetObject.transform.position;
            this.transform.rotation = targetObject.transform.rotation;
            
            Mesh mesh = new Mesh();
                
            targetObject.GetComponent<SkinnedMeshRenderer>().BakeMesh(mesh);

            //AssetDatabase.CreateAsset(mesh, "Assets/test.fbx" );
            //AssetDatabase.SaveAssets();

            this.GetComponent<MeshFilter>().mesh = mesh;
        }
    }
}
