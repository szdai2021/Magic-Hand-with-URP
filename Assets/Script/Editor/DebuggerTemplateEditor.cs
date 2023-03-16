using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DebuggerTemplate))]
public class DebuggerTemplateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DebuggerTemplate myScript = (DebuggerTemplate)target;
        if (GUILayout.Button("Start Pos Transform Test"))
        {
            myScript.testRelativePosRotChange();
        }

        if (GUILayout.Button("Start Robot Range Test"))
        {
            myScript.robotRangeTest();
        }

        if (GUILayout.Button("Reconnect to Local Server"))
        {
            myScript.reConnectToServer();
        }
    }

}

