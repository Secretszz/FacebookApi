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
    using Editor;

    public static class ManifestProcessor
    {
        private const string MANIFEST_RELATIVE_PATH = "unityLibrary/src/main/AndroidManifest.xml";
        private const string STRINGS_XML_PATH = "unityLibrary/src/main/res/values/strings.xml";
        private const string METADATA_APPLICATION_ID  = "com.facebook.sdk.ApplicationId";
        private const string METADATA_CLIENT_TOKEN = "com.facebook.sdk.ClientToken";

        private static XNamespace ns = "http://schemas.android.com/apk/res/android";

        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget target, string projectPath)
        {
            ThirdSDKSettings settings = ThirdSDKSettings.Instance;
            string app_id = settings.FbAppId;
            string client_token = settings.FbClientToken;
            SetFacebookConfig(projectPath, app_id, client_token);
            RefreshLaunchManifest(projectPath, app_id);
        }

        private static void SetFacebookConfig(string projectPath, string app_id, string client_token)
        {
            string stringsPath = Path.Combine(projectPath, STRINGS_XML_PATH);
            if (!File.Exists(stringsPath))
            {
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
            resources.Save(stringsPath);
        }

        private static void RefreshLaunchManifest(string projectPath, string app_id)
        {
            string manifestPath = Path.Combine(projectPath, MANIFEST_RELATIVE_PATH);

            XDocument manifest;
            try
            {
                manifest = XDocument.Load(manifestPath);
            }
#pragma warning disable 0168
            catch (IOException e)
#pragma warning restore 0168
            {
                LogBuildFailed();
                return;
            }

            XElement elemManifest = manifest.Element("manifest");
            if (elemManifest == null)
            {
                LogBuildFailed();
                return;
            }
            
            XElement queries = elemManifest.Element("queries");
            if (queries == null)
            {
                queries = new XElement("queries");
                elemManifest.Add(queries);
            }
            
            queries.Add(new XElement("provider", new XAttribute(ns + "authorities", "com.facebook.katana.provider.PlatformProvider")));
            queries.Add(new XElement("package", new XAttribute(ns + "name", "com.facebook.katana")));

            XElement elemApplication = elemManifest.Element("application");
            if (elemApplication == null)
            {
                LogBuildFailed();
                return;
            }

            elemApplication.Add(new XElement("meta-data", new XAttribute(ns + "name", METADATA_APPLICATION_ID), new XAttribute(ns + "value", "@string/facebook_app_id")));
            elemApplication.Add(new XElement("meta-data", new XAttribute(ns + "name", METADATA_CLIENT_TOKEN), new XAttribute(ns + "value", "@string/facebook_client_token")));
            elemApplication.Add(new XElement("provider", new XAttribute(ns + "authorities", $"com.facebook.app.FacebookContentProvider{app_id}"),
                    new XAttribute(ns + "name", "com.facebook.FacebookContentProvider"),
                    new XAttribute(ns + "exported", "true")));

            elemManifest.Save(manifestPath);
        }

        private static void LogBuildFailed()
        {
            Debug.LogWarning("设置Facebook配置失败，请手动配置facebook配置");
        }
    }
}
#endif
