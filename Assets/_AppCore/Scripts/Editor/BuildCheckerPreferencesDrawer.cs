using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AppCore
{
    [CustomEditor(typeof(BuildCheckerSettings))]
    public class BuildCheckerSettingsDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("Do not try to modify or remove this asset. It's used for BuildSettingsCheckerWindow.", MessageType.Warning);

            if (GUILayout.Button("Open Build Settings Checker"))
            {
                BuildSettingsCheckerWindow.ShowWindow();
            }
        }
    }
}