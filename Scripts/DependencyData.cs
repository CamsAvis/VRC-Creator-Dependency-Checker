﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Cam.DependencyChecker
{
    [CreateAssetMenu(fileName = "Dependency Data", menuName = "Cam/Dependency Data")]
    public class DependencyData : ScriptableObject
    {
        public SceneAsset scene;
        public GameObject prefab;
        public Texture2D thumbnail;

        public string unityVersion;
        public string vrcsdkVersion;

        public List<SocialLink> socialLinks = new List<SocialLink>();
        public List<ShaderDependency> shaderDependencies;

        public void GetShaderDependenciesFromProject()
        {
            if(shaderDependencies == null)
                shaderDependencies = new List<ShaderDependency>();

            string[] guids = AssetDatabase.FindAssets(string.Format("t:Shader"));
            for(int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(assetPath);
                if (shader != null && !DCConstants.ShaderInBlacklist(shader.name) && !DCConstants.DEFAULT_SHADER_NAMES.Contains(shader.name)) {
                    ShaderDependency sd = new ShaderDependency(shader);
                    if(!shaderDependencies.Contains(sd))
                        shaderDependencies.Add(sd);
                }
            }
        }

        public void GetShaderDependenciesFromPrefab(GameObject prefab)
        {
            if (shaderDependencies == null)
                shaderDependencies = new List<ShaderDependency>();

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
                    if (shader == null || DCConstants.DEFAULT_SHADER_NAMES.Contains(shader.name))
                        continue;

                    ShaderDependency dep = new ShaderDependency(shader);
                    if (!shaderDependencies.Contains(dep))
                        shaderDependencies.Add(dep);
                }
            }
        }

        public void UpdateVersions()
        {
            this.unityVersion = Application.unityVersion;
            this.vrcsdkVersion = VRC.Core.SDKClientUtilities.GetSDKVersionDate();
        }
    }
}