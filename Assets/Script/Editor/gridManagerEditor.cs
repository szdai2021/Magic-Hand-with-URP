using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(gridManager))]
public class gridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        gridManager myScript = (gridManager)target;

        if (GUILayout.Button("New Grid"))
        {
            myScript.destroyGrid();

            myScript.createGrid();
        }
    }
}
