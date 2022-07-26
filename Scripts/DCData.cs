﻿using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Cam.DependencyChecker
{
    [CreateAssetMenu(fileName = "Dependency Data", menuName = "Cam/Dependency Data")]
    public class DCData : ScriptableObject
    {
        public bool lockUI;

        public SceneAsset scene;
        public GameObject prefab;
        public Texture2D thumbnail;

        public string unityVersion;
        public string vrcsdkVersion;

        public List<SocialLink> socialLinks = new List<SocialLink>();
        public List<DCShaderDependency> shaderDependencies;

        public void GetShaderDependenciesFromProject()
        {
            if(shaderDependencies == null)
                shaderDependencies = new List<DCShaderDependency>();

            string[] guids = AssetDatabase.FindAssets(string.Format("t:Shader"));
            for(int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(assetPath);

                if (shader.name.Contains("Locked"))
                {
                    EditorUtility.DisplayDialog(
                        "Failed to Retrieve Locked Shader",
                        "A shader on this model is locked, I recommend unlocking all shaders before " +
                        "running this script.",
                        "Ok"
                    );
                    continue;
                }

                bool validShader = shader != null 
                    && !DCFunctions.ShaderInBlacklist(shader.name) 
                    && !DCConstants.DEFAULT_SHADER_NAMES.Contains(shader.name);
                if (validShader) {
                    DCShaderDependency sd = new DCShaderDependency(shader);
                    if (!shaderDependencies.Contains(sd))
                        shaderDependencies.Add(sd);
                }
            }
        }

        public void GetShaderDependenciesFromPrefab(GameObject prefab)
        {
            if (shaderDependencies == null)
                shaderDependencies = new List<DCShaderDependency>();

            Debug.Log($"Retrieving particle systems from {prefab.name}");
            Renderer[] renderers = prefab.gameObject.GetComponentsInChildren<Renderer>(prefab);
            for (int i = 0; i < renderers.Length; i++) {
                Material[] mats = renderers[i].sharedMaterials;
                for (int matIdx = 0; mats != null && matIdx < mats.Length; matIdx++)
                {
                    Material mat = mats[matIdx];
                    if (mat == null)
                        continue;

                    Shader shader = mat.shader;
                    if (shader == null
                        || DCConstants.DEFAULT_SHADER_NAMES.Contains(shader.name)) {
                        continue;
                    }

                    if (shader.name.Contains("Locked")) {
                        EditorUtility.DisplayDialog(
                            "Failed to Retrieve Locked Shader",
                            "A shader on this model is locked, I recommend unlocking all shaders before " +
                            "running this script.", 
                            "Ok"
                        );
                        continue;
                    }

                    DCShaderDependency dep = new DCShaderDependency(shader);
                    bool validShader = shader != null
                        && !DCFunctions.ShaderInBlacklist(shader.name)
                        && !DCConstants.DEFAULT_SHADER_NAMES.Contains(shader.name);
                    if (validShader && !shaderDependencies.Contains(dep))
                        shaderDependencies.Add(dep);
                }
            }
        }

        public void UpdateVersions()
        {
            this.unityVersion = Application.unityVersion;

            var getSDKVersionDate = System.AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.FullName?.Equals("VRC.Core.SDKClientUtilities") ?? false);

            if(getSDKVersionDate != null)
            {
                BindingFlags bf = BindingFlags.Static | BindingFlags.Public;
                object o = getSDKVersionDate.GetMethod("GetSDKVersionDate", bf).Invoke(null, null);
                this.vrcsdkVersion = o == null ? string.Empty : (string)o;
            }
        }
    }
}
