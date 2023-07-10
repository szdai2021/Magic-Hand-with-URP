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

        if (GUILayout.Button("Test the Animation"))
        {
            myScript.animationTest();
        }

        if (GUILayout.Button("Start the Animation"))
        {
            myScript.startAnimation();
        }

        if (GUILayout.Button("Start Robot Pin Test"))
        {
            myScript.testRobotPin();
        }

        if (GUILayout.Button("Start Speedl Test 1"))
        {
            myScript.speedlTest1();
        }

        if (GUILayout.Button("Start Speedl Test 2"))
        {
            myScript.speedlTest2();
        }

        if (GUILayout.Button("Start Robot Safety Test"))
        {
            myScript.safetyDistanceTest();
        }

        if (GUILayout.Button("Start 10s pause"))
        {
            myScript.tenSecondsPauseStart();
        }
    }

}

