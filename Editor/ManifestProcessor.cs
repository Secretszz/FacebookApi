// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		ManifestProcessor.cs
//
// Author Name:		Bridge
//
// Create Time:		2023/12/04 19:13:02
// *******************************************

#if UNITY_ANDROID
namespace Bridge.FacebookApi
{
    using System.IO;
    using System.Xml.Linq;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using Common;

    internal static class ManifestProcessor
    {
        private const string METADATA_APPLICATION_ID  = "com.facebook.sdk.ApplicationId";
        private const string METADATA_CLIENT_TOKEN = "com.facebook.sdk.ClientToken";
        private const string FACEBOOK_ACTIVETY_PACKAGE = "com.facebook.FacebookActivity";
        private const string FACEBOOK_CUSTOM_TAB_PACKAGE = "com.facebook.CustomTabActivity";
        private const string ACTION_VIEW = "android.intent.action.VIEW";
        private const string DEFAULT_CATEGORY = "android.intent.category.DEFAULT";
        private const string BROWSABLE_CATEGORY = "android.intent.category.BROWSABLE";

        [PostProcessBuild(10001)]
        public static void OnPostprocessBuild(BuildTarget target, string projectPath)
        {
            ThirdSDKSettings settings = ThirdSDKSettings.Instance;
            string app_id = settings.FbAppId;
            string client_token = settings.FbClientToken;
            string display_name = settings.FbDisplayName;
            SetFacebookConfig(projectPath, app_id, client_token, display_name);
            RefreshManifest(app_id);
            CopyNativeCode(projectPath);
            Common.ManifestProcessor.ReplaceBuildDefinedCache[Common.ManifestProcessor.FACEBOOK_DEPENDENCIES] = "implementation(\"com.facebook.android:facebook-android-sdk:17.0.1\")";
        }

        private static void SetFacebookConfig(string projectPath, string app_id, string client_token, string display_name)
        {
            string stringsPath = Path.Combine(projectPath, Common.ManifestProcessor.STRINGS_XML_PATH);
            if (!File.Exists(stringsPath))
            {
                Directory.CreateDirectory(stringsPath.Replace("/strings.xml", ""));
                File.WriteAllText(stringsPath, @"<?xml version=""1.0"" encoding=""utf-8""?>
<resources>
</resources>");
            }
            XDocument strings = XDocument.Load(stringsPath);
            XElement resources = strings.Element("resources");
            if (resources == null)
            {
                resources = new XElement("resources");
                strings.Add(resources);
            }

            resources.Add(new XElement("string", new XAttribute("name", "facebook_app_id"), app_id));
            resources.Add(new XElement("string", new XAttribute("name", "fb_login_protocol_scheme"), $"fb{app_id}"));
            resources.Add(new XElement("string", new XAttribute("name", "facebook_client_token"), client_token));
            resources.Add(new XElement("string", new XAttribute("name", "display_name"), display_name));
            resources.Save(stringsPath);
        }

        private static void RefreshManifest(string app_id)
        {
            Common.ManifestProcessor.QueriesElements.Add(new XElement("provider", new XAttribute(Common.ManifestProcessor.ns + "authorities", "com.facebook.katana.provider.PlatformProvider")));
            Common.ManifestProcessor.QueriesElements.Add(new XElement("package", new XAttribute(Common.ManifestProcessor.ns + "name", "com.facebook.katana")));

            Common.ManifestProcessor.ApplicationElements.Add(new XElement("meta-data", new XAttribute(Common.ManifestProcessor.ns + "name", METADATA_APPLICATION_ID), new XAttribute(Common.ManifestProcessor.ns + "value", "@string/facebook_app_id")));
            Common.ManifestProcessor.ApplicationElements.Add(new XElement("meta-data", new XAttribute(Common.ManifestProcessor.ns + "name", METADATA_CLIENT_TOKEN), new XAttribute(Common.ManifestProcessor.ns + "value", "@string/facebook_client_token")));
            Common.ManifestProcessor.ApplicationElements.Add(new XElement("provider", new XAttribute(Common.ManifestProcessor.ns + "name", "com.facebook.FacebookContentProvider"),
                    new XAttribute(Common.ManifestProcessor.ns + "authorities", $"com.facebook.app.FacebookContentProvider{app_id}"),
                    new XAttribute(Common.ManifestProcessor.ns + "exported", "true")));
            
            Common.ManifestProcessor.ApplicationElements.Add(new XElement("activity",
                    new XAttribute(Common.ManifestProcessor.ns + "name", FACEBOOK_ACTIVETY_PACKAGE),
                    new XAttribute(Common.ManifestProcessor.ns + "configChanges", "keyboard|keyboardHidden|screenLayout|screenSize|orientation"),
                    new XAttribute(Common.ManifestProcessor.ns + "label", "@string/display_name")));

            Common.ManifestProcessor.ApplicationElements.Add(new XElement("activity",
                    new XAttribute(Common.ManifestProcessor.ns + "name", FACEBOOK_CUSTOM_TAB_PACKAGE),
                    new XAttribute(Common.ManifestProcessor.ns + "exported", "true"),
                    new XElement("intent-filter",
                            new XElement("action", new XAttribute(Common.ManifestProcessor.ns + "name", ACTION_VIEW)),
                            new XElement("category", new XAttribute(Common.ManifestProcessor.ns + "name", DEFAULT_CATEGORY)),
                            new XElement("category", new XAttribute(Common.ManifestProcessor.ns + "name", BROWSABLE_CATEGORY)),
                            new XElement("data", new XAttribute(Common.ManifestProcessor.ns + "scheme", "@string/fb_login_protocol_scheme")))
                    ));
        }

        private static void CopyNativeCode(string projectPath)
        {
            var remotePackagePath = ThirdSDKPackageManager.GetUnityPackagePath(PackageType.Facebook);
            if (string.IsNullOrEmpty(remotePackagePath))
            {
                // 这个不是通过ump下载的包，查找工程内部文件夹
                remotePackagePath = "Assets/ThirdSDK/FacebookApi";
            }

            remotePackagePath += "/Plugins/Android/facebook";
            Debug.Log("remotePackagePath===" + remotePackagePath);
            FileTool.DirectoryCopy(remotePackagePath, Path.Combine(projectPath, Common.ManifestProcessor.NATIVE_CODE_DIR, "facebook"));
        }

        private static void LogBuildFailed()
        {
            Debug.LogWarning("设置Facebook配置失败，请手动配置facebook配置");
        }
    }
}
#endif
