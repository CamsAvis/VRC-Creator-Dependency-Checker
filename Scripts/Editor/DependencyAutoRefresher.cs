using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Cam.DependencyChecker
{
    [InitializeOnLoad]
    static class DependencyAutoRefresher
    {
        static DependencyAutoRefresher() {
            UpdateWindow("");
            AssetDatabase.importPackageCompleted += UpdateWindow;
        }

        static void UpdateWindow(string packageName)
        {
            if(EditorWindow.HasOpenInstances<StartWindow>()) {
                EditorWindow.GetWindow<StartWindow>().UpdateWindow();
            }
        }
    }
}