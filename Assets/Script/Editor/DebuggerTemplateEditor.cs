using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VRPortalToolkit
{
    [CustomEditor(typeof(DebuggerTemplate))]
    public class DebuggerTemplateEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            DebuggerTemplate myScript = (DebuggerTemplate)target;
            if (GUILayout.Button("Start Test"))
            {
                myScript.testRelativePosRotChange();
            }
        }
    }
}
