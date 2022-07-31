using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cam.DependencyChecker
{
    public static class DCConstants
    {
        // MISC // 
        public const int THUMBNAIL_SIZE = 128;
        public const float WINDOW_WIDTH = 600;
        public const float WINDOW_HEIGHT = 500;
        public const float COLUMN_SPACER = 5;
        public static readonly string WINDOW_NAME = "Avatars Start Screen";

        // EDITOR PREFS //
        private static string[] paths = Application.dataPath.Split('/');
        private static string projectName = paths[paths.Length - 2].Replace(" ", "_").ToLower();
        public static readonly string HIDE_ON_LOAD_STRING = $"cam_dc_hide_on_load_{projectName}";

        // LINKS //
        public const string SDK_DOWNLOAD_URL = "https://vrchat.com/home/download";

        // PATHS //
        public const string A_DEPENDENCY_DATA_PATH = "Assets/!Cam/Scripts/Dependency Checker/Dependency Data.asset";

        // RESOURCES //
        #region Resources

        // ICONS //
        public const string R_DISCORD_ICON_PATH = "Cam/DC/Icons/DiscordIcon";
        public const string R_TWITTER_ICON_PATH = "Cam/DC/Icons/TwitterIcon";
        public const string R_YOUTUBE_ICON_PATH = "Cam/DC/Icons/YoutubeIcon";
        public const string R_REDDIT_ICON_PATH = "Cam/DC/Icons/RedditIcon";
        public const string R_GUMROAD_ICON_PATH = "Cam/DC/Icons/GumroadIcon";
        public const string R_BOOTH_ICON_PATH = "Cam/DC/Icons/BoothIcon";
        public const string R_GITHUB_ICON_PATH = "Cam/DC/Icons/GitHubIcon";
        public const string R_SHOPIFY_ICON_PATH = "Cam/DC/Icons/ShopifyIcon";
        public const string R_TIKTOK_ICON_PATH = "Cam/DC/Icons/TikTokIcon";
        public const string R_LINKTREE_ICON_PATH = "Cam/DC/Icons/LinkTreeIcon";
        public const string R_PAYHIP_ICON_PATH = "Cam/DC/Icons/PayHipIcon";
        public static readonly Font FUTURA_FONT = Resources.Load<Font>("Cam/DC/Fonts/Futura Heavy font");
        public static readonly Texture2D DISCORD_ICON = Resources.Load<Texture2D>(R_DISCORD_ICON_PATH);
        public static readonly Texture2D TWITTER_ICON = Resources.Load<Texture2D>(R_TWITTER_ICON_PATH);
        public static readonly Texture2D YOUTUBE_ICON = Resources.Load<Texture2D>(R_YOUTUBE_ICON_PATH);
        public static readonly Texture2D REDDIT_ICON = Resources.Load<Texture2D>(R_REDDIT_ICON_PATH);
        public static readonly Texture2D GUMROAD_ICON = Resources.Load<Texture2D>(R_GUMROAD_ICON_PATH);
        public static readonly Texture2D BOOTH_ICON = Resources.Load<Texture2D>(R_BOOTH_ICON_PATH);
        public static readonly Texture2D GITHUB_ICON = Resources.Load<Texture2D>(R_GITHUB_ICON_PATH);
        public static readonly Texture2D SHOPIFY_ICON = Resources.Load<Texture2D>(R_SHOPIFY_ICON_PATH);
        public static readonly Texture2D TIKTOK_ICON = Resources.Load<Texture2D>(R_TIKTOK_ICON_PATH);
        public static readonly Texture2D LINKTREE_ICON = Resources.Load<Texture2D>(R_LINKTREE_ICON_PATH);
        public static readonly Texture2D PAYHIP_ICON = Resources.Load<Texture2D>(R_PAYHIP_ICON_PATH);

        public static readonly Dictionary<SocialLink.LinkType, Texture2D> ICONS = new Dictionary<SocialLink.LinkType, Texture2D>() {
            {  SocialLink.LinkType.Discord, DISCORD_ICON },
            {  SocialLink.LinkType.Twitter, TWITTER_ICON },
            {  SocialLink.LinkType.Youtube, YOUTUBE_ICON },
            {  SocialLink.LinkType.Reddit, REDDIT_ICON },
            {  SocialLink.LinkType.Gumroad, GUMROAD_ICON },
            {  SocialLink.LinkType.Booth, BOOTH_ICON },
            {  SocialLink.LinkType.Github, GITHUB_ICON },
            {  SocialLink.LinkType.Shopify, SHOPIFY_ICON },
            {  SocialLink.LinkType.TikTok, TIKTOK_ICON },
            {  SocialLink.LinkType.LinkTree, LINKTREE_ICON },
            {  SocialLink.LinkType.PayHip, PAYHIP_ICON },
        };
        # endregion Resources

        // SHADER CONSTANTS //
        #region Shader Constants
        public const string POI_VERSION_REGEX = "\\d{1,3}.\\d{1,3}.\\d{1,3}";
        public const string POI_SHADER_VERSION_PROPERTY = "shader_master_label";
        public static readonly List<string> SHADER_BLACKLIST_KEYWORDS = new List<string>() {
            "Locked"
        };
        public static readonly List<string> DEFAULT_SHADER_NAMES = new List<string>() {
            "Hidden/TerrainEngine/Splatmap/Diffuse-AddPass",
            "Hidden/TerrainEngine/BillboardTree",
            "Hidden/VR/BlitFromTex2DToTexArraySlice",
            "Hidden/VR/BlitTexArraySlice",
            "Hidden/VR/BlitTexArraySliceToDepth",
            "Hidden/VR/BlitTexArraySliceToDepth_MSAA",
            "Hidden/TerrainEngine/BrushPreview",
            "Hidden/TerrainEngine/CameraFacingBillboardTree",
            "Hidden/Compositing",
            "Hidden/TerrainEngine/CrossBlendNeighbors",
            "Hidden/CubeBlend",
            "Hidden/CubeBlur",
            "Hidden/CubeBlurOdd",
            "Hidden/CubeCopy",
            "Hidden/TerrainEngine/Splatmap/Diffuse-Base",
            "Hidden/TerrainEngine/Splatmap/Diffuse-BaseGen",
            "Nature/Terrain/Diffuse",
            "FX/Flare",
            "Hidden/FrameDebuggerRenderTargetDisplay",
            "Hidden/TerrainEngine/GenerateNormalmap",
            "Hidden/BlitCopy",
            "Hidden/BlitCopyDepth",
            "Hidden/BlitCopyWithDepth",
            "Hidden/BlitToDepth",
            "Hidden/BlitToDepth_MSAA",
            "Hidden/InternalClear",
            "Hidden/Internal-Colored",
            "Hidden/Internal-CombineDepthNormals",
            "Hidden/ConvertTexture",
            "Hidden/Internal-CubemapToEquirect",
            "Hidden/Internal-DeferredReflections",
            "Hidden/Internal-DeferredShading",
            "Hidden/Internal-DepthNormalsTexture",
            "Hidden/InternalErrorShader",
            "Hidden/Internal-Flare",
            "Hidden/Internal-GUIRoundedRect",
            "Hidden/Internal-GUIRoundedRectWithColorPerBorder",
            "Hidden/Internal-GUITexture",
            "Hidden/Internal-GUITextureBlit",
            "Hidden/Internal-GUITextureClip",
            "Hidden/Internal-GUITextureClipText",
            "Hidden/Internal-Halo",
            "Hidden/Internal-MotionVectors",
            "Hidden/Internal-ODSWorldTexture",
            "Hidden/Internal-PrePassLighting",
            "Hidden/Internal-ScreenSpaceShadows",
            "Hidden/Internal-StencilWrite",
            "Hidden/Internal-UIRAtlasBlitCopy",
            "Hidden/Internal-UIRDefault",
            "Hidden/VR/Internal-VRDistortion",
            "Mobile/Diffuse",
            "Mobile/Particles/Additive",
            "Mobile/Particles/Multiply",
            "Mobile/Skybox",
            "Mobile/VertexLit",
            "Hidden/GIDebug/ShowLightMask",
            "Skybox/Cubemap",
            "Skybox/Panoramic",
            "Skybox/Procedural",
            "VR/SpatialMapping/Occlusion",
            "VR/SpatialMapping/Wireframe",
            "Hidden/TerrainEngine/Splatmap/Specular-AddPass",
            "Hidden/TerrainEngine/Splatmap/Specular-Base",
            "Nature/Terrain/Specular",
            "Nature/SpeedTree",
            "Nature/SpeedTree8",
            "Sprites/Default",
            "Sprites/Diffuse",
            "Sprites/Mask",
            "Hidden/TerrainEngine/Splatmap/Standard-AddPass",
            "Hidden/TerrainEngine/Splatmap/Standard-Base",
            "Hidden/TerrainEngine/Splatmap/Standard-BaseGen",
            "Nature/Terrain/Standard",
            "Standard",
            "AR/TangoARRender",
            "Hidden/Nature/Terrain/Utilities",
            "Hidden/TerrainEngine/HeightBlitCopy",
            "Hidden/TerrainEngine/TerrainLayerUtils",
            "Hidden/GIDebug/TextureUV",
            "Hidden/UI/CompositeOverdraw",
            "UI/Default",
            "UI/DefaultETC1",
            "UI/Lit/Bumped",
            "UI/Lit/Detail",
            "UI/Lit/Refraction",
            "UI/Lit/Transparent",
            "Hidden/UI/Overdraw",
            "UI/Unlit/Detail",
            "UI/Unlit/Text",
            "UI/Unlit/Transparent",
            "Unlit/Transparent",
            "Unlit/Color",
            "Unlit/Texture",
            "Hidden/GIDebug/UV1sAsPositions",
            "Hidden/GIDebug/VertexColors",
            "Hidden/TerrainEngine/Details/Vertexlit",
            "Hidden/VideoComposite",
            "Hidden/VideoDecode",
            "Hidden/VideoDecodeAndroid",
            "Hidden/VideoDecodeML",
            "Hidden/VideoDecodeOSX",
            "Hidden/TerrainEngine/Details/WavingDoublePass",
            "Hidden/TerrainEngine/Details/BillboardWavingDoublePass"
        };
        #endregion Shader Constants
    }
}