using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace Cam.DependencyChecker
{
    public static class DCFunctions
    {
        public static string GetPoiShaderVersion(Shader shader)
        {
            string shader_label = string.Empty;
            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                if (shader.GetPropertyName(i).Equals(DCConstants.POI_SHADER_VERSION_PROPERTY))
                {
                    shader_label = shader.GetPropertyDescription(i);
                    break;
                }
            }

            MatchCollection mc = Regex.Matches(shader_label, DCConstants.POI_VERSION_REGEX);
            if (mc.Count > 0)
            {
                for (int i = 0; i < mc.Count; i++)
                    shader_label = mc[i].ToString();
            }

            return shader_label;
        }

        public static string GetLilToonShaderVersion(Shader shader)
        {
            // we are guessing the location of the package.json based on the current knowledge
            // that we possess regarding the default location of the shader

            // get path of shader file
            string shaderPath = AssetDatabase.GetAssetPath(shader);
            if (shaderPath == null || shaderPath.Length < 1)
                goto Failed;

            // get absolute path for assetdatabase
            string[] assetDBAbsPathList = Application.dataPath.Split('/');
            string assetDBAbsPath = string.Join(
                "/", assetDBAbsPathList.Take(assetDBAbsPathList.Length - 1)
            );

            // get get assetdatabse path for lilToon package.json
            string[] lilToonAssetDBPathList = shaderPath.Split('/');
            string assetDBLilToonPackagePath = string.Join(
                "/", lilToonAssetDBPathList.Take(lilToonAssetDBPathList.Length - 2)
            ) + "/package.json";

            // get abs path of lilToon package.json and check if it exists
            string absLilToonPackagePath = $"{assetDBAbsPath}/{assetDBLilToonPackagePath}";
            if (System.IO.File.Exists(absLilToonPackagePath))
            {
                // get package.json data
                TextAsset packageJson = AssetDatabase.LoadAssetAtPath<TextAsset>(assetDBLilToonPackagePath);
                if (packageJson == null)
                    goto Failed;

                // deserialize
                LilToonPackageJSON lilToonPackageJSON = JsonUtility.FromJson<LilToonPackageJSON>(packageJson.text);
                if (lilToonPackageJSON == null)
                    goto Failed;

                // return version
                return lilToonPackageJSON.version;
            }

            Failed:
            {
                // failed
                return null;
            }
        }

        public static bool ShaderInBlacklist(string shaderName)
        {
            foreach (string blacklistedShaderKeyword in DCConstants.SHADER_BLACKLIST_KEYWORDS)
            {
                if (shaderName.Contains(blacklistedShaderKeyword))
                    return true;
            }
            return false;
        }

        public static bool ValidateLink(string url) {
            return System.Uri.IsWellFormedUriString(url, System.UriKind.Absolute);
        }
    }
}