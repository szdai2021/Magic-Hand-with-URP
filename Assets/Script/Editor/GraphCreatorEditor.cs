using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GraphCreator))]
public class GraphCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GraphCreator myScript = (GraphCreator)target;

        if (GUILayout.Button("Read Data"))
        {
            myScript.defineContainersize();

            myScript.openAndReadFile();
        }

        if (GUILayout.Button("New Scatter"))
        {
            myScript.destroyScatter();

            myScript.create3DScatter();
        }
    }
}
