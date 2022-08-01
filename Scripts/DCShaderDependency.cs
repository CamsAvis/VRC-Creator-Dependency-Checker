using UnityEngine;

namespace Cam.DependencyChecker
{
    [System.Serializable]
    public class DCShaderDependency
    {
        public enum ImportStatus {
            PRESENT, INVALID_VERSION, ABSENT
        }

        public string shaderFriendlyName;
        public string shaderName;
        public string version;
        public Shader shader;
        public string link;

        public ImportStatus importStatus;


        public DCShaderDependency(Shader shader) {
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

                if (shaderName.ToLower().Contains("poi")) {
                    version = DCFunctions.GetPoiShaderVersion(shader);

                    // guessing link
                    if (version != null && version.Length > 0) {
                        if (shaderName.ToLower().Contains("toon")) {
                            link = $"https://github.com/poiyomi/PoiyomiToonShader/releases/tag/V{version}";
                        } else if(shaderName.ToLower().Contains("pro")) {
                            link = "https://www.patreon.com/poiyomi";
                        }
                    }
                }

                if (shaderName.ToLower().Contains("liltoon")) {
                    version = DCFunctions.GetLilToonShaderVersion(shader);

                    // guessing link
                    if (version != null && version.Length > 0) {
                        link = $"https://github.com/lilxyzw/lilToon/releases/tag/{version}";
                    }
                }

                if(shaderName.ToLower().Contains("arktoon")) {
                    link = "https://github.com/arktoon-archive/arktoon";
                }
            }
        }


        public void CheckImportStatus()
        {
            Shader shader = Shader.Find(shaderName);
            if (shader == null) {
                this.importStatus = ImportStatus.ABSENT;
            } else {
                this.importStatus = ValidateShaderVersion(shader)
                    ? ImportStatus.PRESENT
                    : ImportStatus.INVALID_VERSION;
            }
        }
        

        bool ValidateShaderVersion(Shader shader) {
            // Poiyomi Shader
            string psv = DCFunctions.GetPoiShaderVersion(shader);
            if(psv != null && psv.Length > 0)
                return psv.Equals(version);

            // LilToon Shader
            string lsv = DCFunctions.GetLilToonShaderVersion(shader);
            if (lsv != null && lsv.Length > 0)
                return lsv.Equals(version);

            // ...

            return true;
        }

        
        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(this.GetType()))
            {
                Shader otherShader = ((DCShaderDependency)obj).shader;
                return otherShader.Equals(this.shader);
            }

            return false;
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
