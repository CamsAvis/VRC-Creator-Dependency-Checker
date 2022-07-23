using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static Cam.DependencyChecker.DCConstants;
using UnityEngine.UIElements;

namespace Cam.DependencyChecker
{
    [CustomEditor(typeof(DependencyData))]
    public class DependencyDataEditor : Editor
    {
        public bool ValidThumbnail;
        Texture2D prefabTexture;

        SerializedProperty thumbnail;
        SerializedProperty socialLinks;
        SerializedProperty unityVersion;
        SerializedProperty vrcsdkVersion;
        SerializedProperty scene;
        SerializedProperty prefab;

        bool socialLinksDropdown;
        bool shadersDropdown;

        private void OnEnable()
        {
            thumbnail = serializedObject.FindProperty("thumbnail");
            socialLinks = serializedObject.FindProperty("socialLinks");
            unityVersion = serializedObject.FindProperty("unityVersion");
            vrcsdkVersion = serializedObject.FindProperty("vrcsdkVersion");
            scene = serializedObject.FindProperty("scene");
            prefab = serializedObject.FindProperty("prefab");

            socialLinksDropdown = true;
            shadersDropdown = true;

            GetPrefabTexture();
            GetVersions();

            if (thumbnail.objectReferenceValue != null)
            {
                Texture2D thumbnailTex = (Texture2D)thumbnail.objectReferenceValue;
                ValidThumbnail = thumbnailTex.width == thumbnailTex.height &&
                    thumbnailTex.width == THUMBNAIL_SIZE;
            }
        }

        public Texture2D GetPrefabTexture()
        {
            if (prefab.objectReferenceValue == null)
                return null;

            string path = AssetDatabase.GetAssetPath(prefab.objectReferenceValue);
            if (path != null)
            {
                var editor = CreateEditor(prefab.objectReferenceValue);
                Texture2D tex = editor.RenderStaticPreview(
                    AssetDatabase.GetAssetPath(prefab.objectReferenceValue),
                    null, 200, 200
                );
                EditorWindow.DestroyImmediate(editor);
                return tex;
            }

            return null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawThumbnail();
            GUILayout.Space(10);
            DrawPrefabAndSceneSelect();
            GUILayout.Space(10);
            DrawGetVersions();
            GUILayout.Space(10);
            DrawSocialLinks();
            GUILayout.Space(10);
            DrawShaders();

            serializedObject.ApplyModifiedProperties();
        }

        void DrawShaders() {
            EditorGUIUtility.labelWidth = 150;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            using (new EditorGUILayout.HorizontalScope()) {
                GUILayout.Label("Shaders", EditorStyles.whiteLargeLabel);
                if(GUILayout.Button(shadersDropdown ? "v" : "<", GUILayout.Width(25))) {
                    shadersDropdown = !shadersDropdown;
                }
            }

            GUILayout.Space(5);

            if (shadersDropdown)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Retrieve Shaders From Project"))
                        RefreshShaderDependenciesFromProject();
                    if (GUILayout.Button("Retrieve Shaders From Prefab"))
                        RefreshShaderDependenciesFromPrefab();
                }
                GUI.color = Color.red;
                if (GUILayout.Button("Reset Shaders"))
                    ResetShaders();
                GUI.color = Color.white;

                GUILayout.Space(10);

                SerializedProperty shaderDependices = serializedObject.FindProperty("shaderDependencies");
                if (shaderDependices == null)
                {
                    Debug.LogError("Dependencies are null");
                }
                else
                {
                    for (int i = 0; i < shaderDependices.arraySize; i++)
                    {
                        using (new EditorGUILayout.HorizontalScope("box"))
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                //EditorGUI.BeginChangeCheck();
                                EditorGUILayout.PropertyField(shaderDependices
                                    .GetArrayElementAtIndex(i)
                                    .FindPropertyRelative("shader")
                                );
                                /*
                                if(EditorGUI.EndChangeCheck()) {
                                    typeof(ShaderDependency).GetMethod("Generate").Invoke(
                                        shaderDependices.GetArrayElementAtIndex(i).objectReferenceValue,
                                        new object[] { 
                                            shaderDependices.GetArrayElementAtIndex(i).FindPropertyRelative("shader"),
                                        }
                                    );
                                }
                                */

                                EditorGUILayout.PropertyField(shaderDependices
                                    .GetArrayElementAtIndex(i)
                                    .FindPropertyRelative("shaderFriendlyName")
                                );

                                EditorGUILayout.PropertyField(shaderDependices
                                    .GetArrayElementAtIndex(i)
                                    .FindPropertyRelative("link")
                                );
                            }
                            EditorGUILayout.EndVertical();
                            GUI.color = Color.red;
                            if (GUILayout.Button("Remove", GUILayout.Width(75), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
                            {
                                shaderDependices.DeleteArrayElementAtIndex(i);
                                i = Mathf.Max(0, shaderDependices.arraySize - 1);
                            }
                            GUI.color = Color.white;
                        }
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }

        void DrawPrefabAndSceneSelect()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Prefab and Scene Selection", EditorStyles.whiteLargeLabel);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(scene);
            GUILayout.Space(5);
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    /*GUILayout.Label("Prefab");
                    if (prefab.objectReferenceValue != null) {
                        GUILayout.Space(31);
                    }
                    EditorGUILayout.PropertyField(prefab, GUIContent.none, GUILayout.Height(30));
                    if (EditorGUI.EndChangeCheck()) {
                        prefabTexture = GetPrefabTexture();
                    }*/
                    EditorGUILayout.PropertyField(prefab);
                    if (scene.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox(
                            "You are missing a scene, your buyers will be very confused when trying to find your model",
                            MessageType.Error,
                            true
                        );
                    }

                    if (prefab.objectReferenceValue == null)
                    {
                        EditorGUILayout.HelpBox(
                            "You are missing a prefab, I recommend creating a prefab out of your model so buyers will have a backup",
                            MessageType.Error,
                            true
                        );
                    }
                }
                /*
                if (prefab.objectReferenceValue != null)
                {
                    GUILayout.Box(prefabTexture, GUILayout.Width(80), GUILayout.Height(80));
                }*/
            }


            if (prefab.objectReferenceValue != null && scene.objectReferenceValue != null)
            {
                EditorGUILayout.HelpBox(
                    "Nice! :)\nYour buyers will appreciate having both a prefab and a scene",
                    MessageType.Info,
                    true
                );
            }
            EditorGUILayout.EndVertical();
        }

        void DrawGetVersions()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Software Versions", EditorStyles.whiteLargeLabel);
                GUILayout.Space(5);
                using (new EditorGUILayout.HorizontalScope())
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("VRCSDK Version", GUILayout.Width(150));
                        using (new EditorGUI.DisabledGroupScope(true))
                            vrcsdkVersion.stringValue = EditorGUILayout.TextField(vrcsdkVersion.stringValue);
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Unity Version", GUILayout.Width(150));
                        using (new EditorGUI.DisabledGroupScope(true))
                            unityVersion.stringValue = EditorGUILayout.TextField(unityVersion.stringValue);
                        EditorGUILayout.EndHorizontal();
                    }
                }

                if (unityVersion == null || unityVersion.stringValue.Length < 1)
                {
                    GUILayout.Space(5);
                    EditorGUILayout.HelpBox(
                        "You have not retrieved this project's Unity Version, please hit \"Retrieve\" above to do so.",
                        MessageType.Error,
                        true
                    );
                }
                if (vrcsdkVersion == null || vrcsdkVersion.stringValue.Length < 1)
                {
                    GUILayout.Space(5);
                    EditorGUILayout.HelpBox(
                        "You have not retrieved this project's Unity Version, please hit \"Retrieve\" above to do so.",
                        MessageType.Error,
                        true
                    );
                }
            }
        }

        void DrawThumbnail()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                // label
                EditorGUILayout.BeginVertical();
                GUILayout.Label("Thumbnail", EditorStyles.whiteLargeLabel);
                if (thumbnail.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox(
                        $"You do not have a thumbnail image.\n\n" +
                        $"This won't break anything, but it's always nice to add your own flair to the welocme screen <3\n\n" +
                        $"Make sure the image is\n{THUMBNAIL_SIZE}px x {THUMBNAIL_SIZE}px square",
                        MessageType.Warning
                    );
                }
                else if (!ValidThumbnail)
                {
                    Texture2D thumbnailTex = (Texture2D)thumbnail.objectReferenceValue;
                    EditorGUILayout.HelpBox(
                        $"Your image is {thumbnailTex.width}px x {thumbnailTex.height}px.\n\n" +
                        $"Thumbnails are recommended to be \n{THUMBNAIL_SIZE}px x {THUMBNAIL_SIZE}px square.\n\n" +
                        $"If they are not, they will be cropped to fit.",
                        MessageType.Error,
                        true
                    );
                }
                else
                {
                    EditorGUILayout.HelpBox(
                        "Yay! Nice Thumbnail <3",
                        MessageType.Info
                    );
                }
                EditorGUILayout.EndVertical();

                // draw large thumbnail
                EditorGUI.BeginChangeCheck();
                Texture2D iconImage = (Texture2D)thumbnail.objectReferenceValue;
                iconImage = (Texture2D)EditorGUILayout.ObjectField(
                    iconImage,
                    typeof(Texture2D),
                    false,
                    GUILayout.Width(128),
                    GUILayout.Height(128)
                );
                thumbnail.objectReferenceValue = iconImage;
            }

            // check for valid thumbnail
            if (EditorGUI.EndChangeCheck() && thumbnail.objectReferenceValue != null)
            {
                Texture2D thumbnailTex = (Texture2D)thumbnail.objectReferenceValue;
                ValidThumbnail = thumbnailTex.width == thumbnailTex.height &&
                    thumbnailTex.width == THUMBNAIL_SIZE;
            }
        }

        void DrawSocialLinks()
        {
            if (socialLinks == null)
                return;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Social Links", EditorStyles.whiteLargeLabel);
                    if (GUILayout.Button(socialLinksDropdown ? "v" : "<", GUILayout.Width(25))) {
                        socialLinksDropdown = !socialLinksDropdown;
                    }
                }

                GUILayout.Space(5);

                if (!socialLinksDropdown)
                    return;

                using (new EditorGUI.DisabledGroupScope(socialLinks != null && socialLinks.arraySize >= 11))
                {
                    if (GUILayout.Button("Add Social Link", GUILayout.Height(25)))
                    {
                        int idx = Mathf.Max(socialLinks.arraySize - 1, 0);
                        socialLinks.InsertArrayElementAtIndex(idx);
                    }
                }

                GUILayout.Space(5);

                for (int i = 0; i < socialLinks.arraySize; i++)
                {
                    SerializedProperty link = socialLinks.GetArrayElementAtIndex(i);
                    SerializedProperty url = link.FindPropertyRelative("url");
                    SerializedProperty linkType = link.FindPropertyRelative("linkType");

                    using (new EditorGUILayout.HorizontalScope("box"))
                    {
                        GUILayout.Box(
                            DCConstants.ICONS[(SocialLink.LinkType)linkType.enumValueIndex],
                            GUILayout.Height(35),
                            GUILayout.Width(35)
                        );

                        EditorGUIUtility.labelWidth = 100;
                        using (new EditorGUILayout.VerticalScope(GUILayout.Height(35)))
                        {
                            linkType.enumValueIndex = (int)(SocialLink.LinkType)EditorGUILayout.EnumPopup(
                                "Link Type",
                                (SocialLink.LinkType)linkType.enumValueIndex
                            );
                            url.stringValue = EditorGUILayout.TextField(
                                "URL",
                                url.stringValue
                            );
                        }

                        GUI.color = Color.red;
                        if (GUILayout.Button("Remove", GUILayout.Height(35), GUILayout.Width(75)))
                        {
                            socialLinks.DeleteArrayElementAtIndex(i);
                            serializedObject.ApplyModifiedProperties();
                            GUI.color = Color.white;
                            return;
                        }
                        GUI.color = Color.white;
                    }
                }
            }
        }
    
        void GetVersions() {
            System.Reflection.MethodInfo updateVersions = typeof(DependencyData).GetMethod("UpdateVersions");
            if (updateVersions != null) {
                updateVersions.Invoke(serializedObject.targetObject, null);
            }
        }
    
        void RefreshShaderDependenciesFromProject()
        {
            System.Reflection.MethodInfo updateShaders = typeof(DependencyData).GetMethod("GetShaderDependenciesFromProject");
            if (updateShaders != null) {
                updateShaders.Invoke(serializedObject.targetObject, null);
            }
        }

        void RefreshShaderDependenciesFromPrefab()
        {
            System.Reflection.MethodInfo updateShaders = typeof(DependencyData).GetMethod("GetShaderDependenciesFromPrefab");
            if (updateShaders != null)
            {
                updateShaders.Invoke(
                    serializedObject.targetObject, 
                    new object[] { prefab.objectReferenceValue }
                );
            }
        }

        void ResetShaders() {
            serializedObject.FindProperty("shaderDependencies").ClearArray();
        }
    }
}