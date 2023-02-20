using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideGameObjectRender : MonoBehaviour
{
    public bool hide = false;

    // Update is called once per frame
    void Update()
    {
        Renderer[] All = this.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in All)
        {
            r.enabled = hide;
        }
    }
}

