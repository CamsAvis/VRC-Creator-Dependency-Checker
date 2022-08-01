using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Cam.DependencyChecker
{
    public class DCDataHider
    {
        [MenuItem("Window/UI/Cam/Toggle DC Editability", false, 1999)]
        public static void Init()
        {
            DCData data = AssetDatabase.LoadAssetAtPath<DCData>(DCConstants.A_DEPENDENCY_DATA_PATH);
            if (data != null)
            {
                HideFlags flags = data.hideFlags;
                if (flags == HideFlags.NotEditable)
                    data.hideFlags = HideFlags.None;
                if (flags == HideFlags.None)
                    data.hideFlags = HideFlags.NotEditable;
                AssetDatabase.Refresh();
            }
        }
    }
}