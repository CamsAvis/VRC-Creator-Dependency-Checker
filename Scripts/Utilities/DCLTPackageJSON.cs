using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cam.DependencyChecker
{
    public class DCLTPackageJSON
    {
        public string name;
        public string version;
        public string displayName;
        public string description;
        public string unityVersion;
        public string documentationUrl;
        public string changelogUrl;
        public string licenseUrl;
        public string license;
        public List<string> keywords;
        public string url;
    }

    /* 
     * {
      "name": "jp.lilxyzw.liltoon",
      "version": "1.3.4",
      "displayName": "lilToon",
      "description": "Feature-rich toon shader.",
      "unity": "2018.1",
      "documentationUrl": "https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/README.md",
      "changelogUrl": "https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/CHANGELOG.md",
      "licensesUrl": "https://github.com/lilxyzw/lilToon/blob/master/Assets/lilToon/LICENSE",
      "license": "MIT",
      "keywords": ["Toon", "Shader", "Material"],
      "url" : "https://github.com/lilxyzw/lilToon.git?path=Assets/lilToon#master"
    }
    */
}