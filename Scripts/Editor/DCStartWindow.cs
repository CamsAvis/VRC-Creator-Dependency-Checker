#if UNITY_EDITOR

using System;
using System.Linq;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;


namespace Cam.DependencyChecker
{
    public class ShowStartWindow
    {

        [InitializeOnLoadMethod]
        private static void RegisterOpen()
        {
            EditorApplication.delayCall -= Init;
            EditorApplication.delayCall += Init;
        }

        public static void Init()
        {
            EditorApplication.delayCall -= Init;

            string hideOnLoad = GetHideOnLoadPath();
            string hasOpened = GetHasOpenedPath();


            if (!EditorPrefs.HasKey(hideOnLoad))
                EditorPrefs.GetBool(hideOnLoad, false);

            if (!SessionState.GetBool(hasOpened, false))
            {
                SessionState.SetBool(hasOpened, true);
                if (EditorPrefs.GetBool(hideOnLoad) == false)
                {
                    DCStartWindow.Init();
                }
            }
        }

        public static string GetHideOnLoadPath()
        {
            string[] paths = Application.dataPath.Split('/');
            string projectName = paths[paths.Length - 2].Replace(" ", "_").ToLower();
            return $"{DCConstants.HIDE_ON_LOAD}_{projectName}";
        }

        public static string GetFirstLaunchPath()
        {
            string[] paths = Application.dataPath.Split('/');
            string projectName = paths[paths.Length - 2].Replace(" ", "_").ToLower();
            return $"{DCConstants.FIRST_LAUNCH}_{projectName}";
        }
        public static string GetHasOpenedPath()
        {
            string[] paths = Application.dataPath.Split('/');
            string projectName = paths[paths.Length - 2].Replace(" ", "_").ToLower();
            return $"{DCConstants.HAS_OPENED_THIS_SESSION}_{projectName}";
        }
    }

    public class DCStartWindow : EditorWindow
    {
        string vrcsdkVersion;
        bool unityVersionSuccess;
        bool allShadersSuccess;
        bool sdkVersionSuccess;
        bool av3ManagerSuccess;

        public const string SDK_VERSION = "2022.06.03.00.04";

        static GUIStyle discordTagStyle;
        Vector2 scrollPos;

        DCData data;
        List<DCShaderDependency> absentShaderDependencies;
        List<DCShaderDependency> invalidVersionShaderDependencies;
        List<DCShaderDependency> presentShaderDependencies;

        [MenuItem("Cam/Start", false, 1999)]
        public static void Init()
        {
            DCStartWindow window = (DCStartWindow)GetWindow(typeof(DCStartWindow), true, "Avatars Start Screen");
            window.minSize = new Vector2(DCConstants.WINDOW_WIDTH, DCConstants.WINDOW_HEIGHT);
            window.maxSize = new Vector2(DCConstants.WINDOW_WIDTH, DCConstants.WINDOW_HEIGHT);
            window.Show();
        }


        private void OnEnable()
        {
            allShadersSuccess = true;
            UpdateWindow();
        }

        public void UpdateWindow()
        {
            // Dependency Data Stufff
            data = AssetDatabase.LoadAssetAtPath<DCData>(DCConstants.A_DEPENDENCY_DATA_PATH);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<DCData>();
                AssetDatabase.CreateAsset(data, DCConstants.A_DEPENDENCY_DATA_PATH);
                EditorUtility.SetDirty(data);
            }
            else
            {
                absentShaderDependencies = new List<DCShaderDependency>();
                invalidVersionShaderDependencies = new List<DCShaderDependency>();
                presentShaderDependencies = new List<DCShaderDependency>();

                for (int i = 0; i < data.shaderDependencies.Count; i++)
                {
                    data.shaderDependencies[i].CheckImportStatus();
                    switch (data.shaderDependencies[i].importStatus)
                    {
                        case DCShaderDependency.ImportStatus.ABSENT:
                            allShadersSuccess = false;
                            absentShaderDependencies.Add(data.shaderDependencies[i]);
                            break;
                        case DCShaderDependency.ImportStatus.INVALID_VERSION:
                            invalidVersionShaderDependencies.Add(data.shaderDependencies[i]);
                            allShadersSuccess = false;
                            break;
                        case DCShaderDependency.ImportStatus.PRESENT:
                            presentShaderDependencies.Add(data.shaderDependencies[i]);
                            break;
                    }
                }
            }

            unityVersionSuccess = data != null && Application.unityVersion.Equals(data.unityVersion);
            allShadersSuccess = true;
            sdkVersionSuccess = false;

            // VRCSDK
            var vrcsdk = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.FullName?.Equals("VRC.SDK3.Avatars.Components.VRCAvatarDescriptor") ?? false);
            //test
            if (vrcsdk != null)
            {
                var getSDKVersionDate = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .FirstOrDefault(x => x.FullName?.Equals("VRC.Core.SDKClientUtilities") ?? false);

                if (getSDKVersionDate != null)
                {
                    System.Reflection.BindingFlags bf = System.Reflection.BindingFlags.Static
                        | System.Reflection.BindingFlags.Public;

                    object o = getSDKVersionDate.GetMethod("GetSDKVersionDate", bf).Invoke(null, null);

                    if (o != null)
                    {
                        vrcsdkVersion = (String)o;
                        sdkVersionSuccess = data != null && vrcsdkVersion.Equals(data.vrcsdkVersion);
                    }
                    else
                    {
                        vrcsdkVersion = null;
                    }
                }
                else
                {
                    vrcsdkVersion = null;
                }
            }
            else
            {
                vrcsdkVersion = null;
                Debug.Log("Failed to find VRCSDK");
            }

            // Other
            if (DCConstants.FUTURA_FONT == null)
                Debug.LogError("Font not detected");


            // AV3 Manager
            var av3ManagerCloner = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.FullName?.Equals("VRLabs.AV3Manager.AnimatorCloner") ?? false);
            av3ManagerSuccess = av3ManagerCloner != null;
        }

        public void OnGUI()
        {
            // load data if not exists
            if (data == null)
            {
                data = AssetDatabase.LoadAssetAtPath<DCData>(DCConstants.A_DEPENDENCY_DATA_PATH);
                if (data != null)
                {
                    data.shaderDependencies.ForEach(sd => sd.CheckImportStatus());
                }
                return;
            }

            InitStyles();

            using (new EditorGUILayout.HorizontalScope(GUIStyle.none, GUILayout.ExpandWidth(true)))
            {
                float columnOneWidth = DCConstants.WINDOW_WIDTH / 3f;
                float columnTwoWidth = DCConstants.WINDOW_WIDTH - columnOneWidth - DCConstants.COLUMN_SPACER - 10;

                using (new EditorGUILayout.VerticalScope(
                    GUILayout.Width(columnOneWidth), GUILayout.Height(DCConstants.WINDOW_HEIGHT - 5)))
                {
                    DrawSocialLinks();
                    DrawHideOnLoad();
                }

                GUILayout.Space(DCConstants.COLUMN_SPACER);

                using (new EditorGUILayout.VerticalScope(
                    GUILayout.Width(columnTwoWidth), GUILayout.Height(DCConstants.WINDOW_HEIGHT - 5)))
                {
                    ShowStarterAssets();
                    ShowDependencies();
                }
            }
        }

        void InitStyles()
        {
            discordTagStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                richText = true,
                font = DCConstants.FUTURA_FONT
            };
        }

        void DrawSocialLinks()
        {
            GUILayout.Label(new GUIContent("Helpful Links"), discordTagStyle);

            if (data.thumbnail != null)
            {
                Rect curViewRect = EditorGUILayout.GetControlRect();
                curViewRect = new Rect(
                  curViewRect.x + 40, curViewRect.y, 128, 128
                );
                GUI.DrawTexture(curViewRect, data.thumbnail);
            }

            GUILayout.Space(115);

            // Draw Social Links
            for (int i = 0; i < data.socialLinks.Count; i++)
            {
                SocialLink link = data.socialLinks[i];
                LinkButton(
                    link.linkType.ToString(),
                    link.url,
                    DCConstants.ICONS[link.linkType]
                );
            }
        }

        void ShowStarterAssets()
        {
            GUILayout.Label(new GUIContent("Starter Assets"), discordTagStyle);

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox,
                GUILayout.Height(DCConstants.WINDOW_HEIGHT / 3.0f - 60)))
            {
                bool allDependenciesSatisfied = unityVersionSuccess && allShadersSuccess && sdkVersionSuccess;
                using (new EditorGUI.DisabledGroupScope(!allDependenciesSatisfied))
                {
                    if (data.scene != null)
                    {
                        GUILayout.Label("Scene");
                        EditorGUILayout.BeginHorizontal();

                        using (new EditorGUI.DisabledGroupScope(true))
                            EditorGUILayout.ObjectField(data.scene, typeof(object), false);

                        if (GUILayout.Button("Open Scene", GUILayout.Width(150)))
                        {
                            SaveAndOpenScene(data.scene);
                        }
                        EditorGUILayout.EndHorizontal();

                        GUILayout.Space(10);
                    }

                    if (data.prefab != null)
                    {
                        GUILayout.Label("Avatar Prefab");
                        EditorGUILayout.BeginHorizontal();

                        using (new EditorGUI.DisabledGroupScope(true))
                            EditorGUILayout.ObjectField(data.prefab, typeof(object), false);

                        if (GUILayout.Button("Place Into Scene", GUILayout.Width(150)))
                        {
                            GameObject instanced = InstantiatePrefabAndReturn(data.prefab);
                            EditorGUIUtility.PingObject(instanced);
                            Selection.activeGameObject = instanced;
                            SceneView.FrameLastActiveSceneView();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
        }

        static GameObject InstantiatePrefabAndReturn(GameObject prefab)
        {
            GameObject instantiatedPrefab = GameObject.Instantiate(prefab);
            instantiatedPrefab.name = prefab.name;
            instantiatedPrefab.transform.position = Vector3.zero;
            instantiatedPrefab.transform.rotation = Quaternion.identity;
            return instantiatedPrefab;
        }

        void SaveAndOpenScene(SceneAsset scene)
        {
            UnityEngine.SceneManagement.Scene currentScene = EditorSceneManager.GetActiveScene();
            bool saveScene = EditorUtility.DisplayDialog(
                "Save Scene",
                "You are about to open a new scene, would you like to save your current scene first?",
                "yes",
                "no"
            );

            if (saveScene)
            {
                EditorSceneManager.SaveScene(currentScene, currentScene.path);
            }

            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(scene));
        }

        void DrawHideOnLoad()
        {
            Rect labelRect = new Rect(5, DCConstants.WINDOW_HEIGHT - 45, DCConstants.WINDOW_WIDTH, 20);
            Rect buttonRect = new Rect(5, DCConstants.WINDOW_HEIGHT - 45 + 20, 200, 20);

            GUI.Label(labelRect, "Show this window on load?");
            if (EditorPrefs.HasKey(DCConstants.HIDE_ON_LOAD_STRING))
            {
                bool hideOnLoadBool = EditorPrefs.GetBool(DCConstants.HIDE_ON_LOAD_STRING);
                string text = hideOnLoadBool ? "Hidden" : "Shown";
                GUI.color = hideOnLoadBool ? Color.gray : Color.white;

                if (GUI.Button(buttonRect, text))
                {
                    EditorPrefs.SetBool(DCConstants.HIDE_ON_LOAD_STRING, !hideOnLoadBool);
                }
                GUI.color = Color.white;
            }
            else
            {
                EditorPrefs.SetBool(DCConstants.HIDE_ON_LOAD_STRING, false);
            }
        }

        void LinkButton(string buttonText, string buttonLink, Texture2D buttonIcon)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Box(buttonIcon, GUILayout.Height(25), GUILayout.Width(25));
                if (GUILayout.Button(buttonText, GUILayout.Height(25)))
                    Application.OpenURL(buttonLink);
            }
        }

        void ShowDependencies()
        {
            float boxHeight = DCConstants.WINDOW_HEIGHT * 2.0f / 3.0f + 10;
            bool allDependenciesSatisfied = unityVersionSuccess && allShadersSuccess && sdkVersionSuccess;

            //Rect currentViewRect = EditorGUILayout.GetControlRect(false);
            GUILayout.Label("Dependencies", discordTagStyle);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Height(boxHeight)))
            {
                EditorGUILayout.BeginVertical("box");
                if (allDependenciesSatisfied)
                {
                    string message = $"Everything looks good!!";
                    EditorGUILayout.HelpBox(new GUIContent(message, DCConstants.CHECK_ICON, ""), true);
                }
                else
                {
                    string message = $"Some dependencies have not been satisfied.";
                    EditorGUILayout.HelpBox(message, MessageType.Warning, true);
                }

                GUILine(!allDependenciesSatisfied);

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                using (new EditorGUI.DisabledGroupScope(allDependenciesSatisfied))
                {

                    // UNITY VERSION
                    #region Unity Version
                    if (Application.unityVersion.Equals(data.unityVersion))
                    {
                        string message = $"The correct Unity Version '{data.unityVersion}' has been detected!";
                        EditorGUILayout.HelpBox(new GUIContent(message, DCConstants.CHECK_ICON, ""), true);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox(
                            $"The creator built this package on Unity Version '{data.unityVersion}'\n" +
                            $"Your project is running on Unity Version '{Application.unityVersion}'.\n" +
                            $"Some things my not function as intended",
                            MessageType.Error,
                            true
                        );
                    }
                    #endregion Unity Version

                    // SDK VERSION
                    #region VRCSDK Version
                    if (vrcsdkVersion == null || vrcsdkVersion.Length < 1)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.HelpBox(
                            $"The VRChat SDK 'VRCSDK3-AVATAR-{data.vrcsdkVersion}' was not detected in this project.\n\n" +
                            $"This package will be unable to be uploaded to VRChat until the VRChat SDK has been imported.\n\n" +
                            "Additionally, importing the VRChat SDK AFTER this package may cause issues with VRChat-associated scripts; " +
                            "if anything in this package is broken in game, this issue may be the cause.",
                            MessageType.Error,
                            true
                        );
                        if (GUILayout.Button("Fix", GUILayout.Width(50), GUILayout.Height(138)))
                            Application.OpenURL(DCConstants.SDK_DOWNLOAD_URL);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (sdkVersionSuccess)
                    {
                        string message = $"The correct VRCSDK Version '{data.vrcsdkVersion}' has been detected!";
                        EditorGUILayout.HelpBox(new GUIContent(message, DCConstants.CHECK_ICON, ""), true);
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.HelpBox(
                            $"The creator built this package using VRCSDK Version '{data.vrcsdkVersion}'\n" +
                            $"Your project is running on VRCSDK Version '{vrcsdkVersion}'.\n" +
                            $"Some things my not function as intended",
                            MessageType.Error,
                            true
                        );
                        if (GUILayout.Button("Fix", GUILayout.Width(50), GUILayout.Height(66)))
                            Application.OpenURL(DCConstants.SDK_DOWNLOAD_URL);
                        EditorGUILayout.EndHorizontal();
                    }
                    #endregion VRCSDK Version

                    // SHADERS
                    #region Shaders
                    absentShaderDependencies.ForEach(sd => DrawShaderDependency(sd));
                    invalidVersionShaderDependencies.ForEach(sd => DrawShaderDependency(sd));
                    presentShaderDependencies.ForEach(sd => DrawShaderDependency(sd));
                    #endregion

                    EditorGUILayout.EndScrollView();
                    EditorGUILayout.EndVertical();
                }
            }

            if (allDependenciesSatisfied)
            {
                GUI.DrawTexture(new Rect(280, 225, 256, 256), DCConstants.CHECK_ICON_HP);
                GUI.DrawTexture(new Rect(500, 430, 64, 64), DCConstants.PEEPO_HYPERS_ICON);
            }
        }

        void DrawShaderDependency(DCShaderDependency sd)
        {
            GUIContent messageContent = new GUIContent(string.Empty);
            
            switch (sd.importStatus)
            {
                case DCShaderDependency.ImportStatus.PRESENT:
                    string message = string.Empty;
                    if (sd.version != null && sd.version.Length > 0)
                        message = $"'{sd.shaderFriendlyName}' v{sd.version} has been detected!";
                    else if (sd.version != null)
                        message = $"'{sd.shaderFriendlyName}' has been detected!";

                    messageContent.text = message;
                    messageContent.image = DCConstants.CHECK_ICON;
                    break;
                case DCShaderDependency.ImportStatus.INVALID_VERSION:
                    allShadersSuccess = false;
                    string versionMessage = $"You have an invalid version of '{sd.shaderFriendlyName}' installed.\n" +
                        $"This project requires '{sd.shaderFriendlyName}' version '{sd.version}'";

                    messageContent.text = versionMessage;
                    messageContent.image = EditorGUIUtility.IconContent("console.erroricon").image;
                    break;
                case DCShaderDependency.ImportStatus.ABSENT:
                    allShadersSuccess = false;
                    string version = sd.version.Length > 0
                        ? $"This project requires '{sd.shaderFriendlyName}' version '{sd.version}'"
                        : $"This project requires '{sd.shaderFriendlyName}'";

                    messageContent.text = $"You do not have '{sd.shaderFriendlyName}' installed.\n{version}";
                    messageContent.image = EditorGUIUtility.IconContent("console.erroricon").image;
                    break;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.HelpBox(messageContent);
                if(sd.importStatus != DCShaderDependency.ImportStatus.PRESENT)
                {
                    float boxHeight = new GUIStyle("HelpBox").CalcHeight(messageContent, 335);// rect.width);
                    if (sd.link.Length > 0 && GUILayout.Button("Fix", GUILayout.Width(35), GUILayout.Height(boxHeight)))
                        Application.OpenURL(sd.link);
                }
            }
        }

        static void GUILine(bool enabled, int i_height = 1)
        {
            GUILayout.Space(10);
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            if (rect != null)
            {
                //rect.width = EditorGUIUtility.currentViewWidth - 50;
                GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
            }
            GUILayout.Space(10);
        }
    }
}
#endif