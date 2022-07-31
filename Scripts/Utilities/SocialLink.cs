using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cam.DependencyChecker
{
    [System.Serializable]
    public struct SocialLink
    {
        public enum LinkType {
            Booth, Discord, Github, Gumroad,
            LinkTree, PayHip, Reddit, Shopify, 
            TikTok, Twitter, Youtube, 
        }

        public LinkType linkType;
        public string url;
    }
}
