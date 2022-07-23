using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Cam.DependencyChecker
{
    [System.Serializable]
    public class ShaderDependency
    {
        public enum ImportStatus {
            PRESENT, INVALID_VERSION, ABSENT
        }

        const string POI_REGEX = "\\d{1,3}.\\d{1,3}.\\d{1,3}";
        const string POI_SHADER_VERSION_PROPERTY = "shader_master_label";

        public string shaderFriendlyName;
        public string shaderName;
        public string version;
        public Shader shader;
        public string link;

        public ImportStatus importStatus;


        public ShaderDependency(Shader shader) {
            Generate(shader);
        }

        public void Generate(Shader shader)
        {
            if (shader == null)
            {
                Debug.LogError("Shader is null");
                return;
            }

            this.shader = shader;
            this.shaderName = shader.name;
            this.link = string.Empty;
            if (this.shaderName.Length > 0)
            {
                string[] shaderPathing = shaderName.Split('/');
                shaderFriendlyName = shaderPathing[Mathf.Max(0, shaderPathing.Length - 1)];

                Debug.Log($"\tShader Friendly Name: '{shaderFriendlyName}'");

                if (shaderName.ToLower().Contains("poi"))
                {
                    Debug.Log($"\tPoiyomi Shader Detected");
                    version = GetPoiShaderVersion(shader);
                    Debug.Log($"\tPoiyomi Shader version '{version}'");
                }
            }
        }

        public void CheckImportStatus()
        {
            Shader shader = Shader.Find(shaderName);
            if (shader != null) {
                this.importStatus = ValidateShaderVersions(shader)
                    ? ImportStatus.PRESENT
                    : ImportStatus.INVALID_VERSION;
            } else {
                this.importStatus = ImportStatus.ABSENT;
            }
        }

        bool ValidateShaderVersions(Shader shader) {
            // Poiyomi Shader
            string sv = GetPoiShaderVersion(shader);
            if(sv != null && sv.Length > 0)
                return sv.Equals(version);

            // ...

            return true;
        }

        static string GetPoiShaderVersion(Shader shader)
        {
            string shader_label = string.Empty;
            for (int i = 0; i < shader.GetPropertyCount(); i++)
            {
                if (shader.GetPropertyName(i).Equals(POI_SHADER_VERSION_PROPERTY))
                {
                    shader_label = shader.GetPropertyDescription(i);
                    break;
                }
            }

            MatchCollection mc = Regex.Matches(shader_label, POI_REGEX);
            if (mc.Count > 0) { 
                for (int i = 0; i < mc.Count; i++)
                {
                    Debug.Log($"\t\t\t{i}) Poiyomi shader Regex match: '{mc[i]}'");
                    shader_label = mc[i].ToString();
                }
            }

            return shader_label;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(this.GetType()))
            {
                Shader otherShader = ((ShaderDependency)obj).shader;
                return otherShader.Equals(this.shader);
            }

            return false;
        }
    }
}
