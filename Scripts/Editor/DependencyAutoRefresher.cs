using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Cam.DependencyChecker
{
    [InitializeOnLoad]
    static class DependencyAutoRefresher
    {
        public const string SDK_DOWNLOAD_URL = "https://www.vrchat.com/home/download";

        static DependencyAutoRefresher() {
            UpdateWindow("");
            AssetDatabase.importPackageCompleted += UpdateWindow;

            CheckForVRCSDK("");
            AssetDatabase.importPackageCompleted += CheckForVRCSDK;
        }

        static void UpdateWindow(string packageName)
        {
            if(EditorWindow.HasOpenInstances<DCStartWindow>()) {
                DCStartWindow[] windows = Resources.FindObjectsOfTypeAll<DCStartWindow>();
                for (int i = 0; i < windows.Length; i++) {
                    windows[i].UpdateWindow();
                    windows[i].Repaint();
                }
            }
        }

        static void CheckForVRCSDK(string packageName)
        {
            AssetDatabase.Refresh();
            if (packageName.Contains("VRCSDK3") || AssetDatabase.IsValidFolder("Assets/VRCSDK"))
            {
                AssetDatabase.importPackageCompleted -= CheckForVRCSDK;
                return;
            }

#if !VRC_SDK_VRCSDK3
            bool done = EditorUtility.DisplayDialog("VRCSDK NOT DETECTED",
                "Hello there :)\n\n" +
                "I do not detect the VRCSDK in your project. This avatar WILL NOT FUNCTION " +
                "until the SDK is present in the project.\n\n",
                "Click me to download the SDK"
            );

            if(done) {
                Application.OpenURL(SDK_DOWNLOAD_URL);
            }
#else
            AssetDatabase.importPackageCompleted -= CheckForVRCSDK;
#endif
        }
    }
}