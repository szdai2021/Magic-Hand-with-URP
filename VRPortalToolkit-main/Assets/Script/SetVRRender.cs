using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVRRender : MonoBehaviour
{
    Material mat;
    Renderer rend;
   public  RenderTexture rendTex;
    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
        //rendTex = rend.material.mainTexture;
        rendTex.vrUsage = VRTextureUsage.TwoEyes;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
