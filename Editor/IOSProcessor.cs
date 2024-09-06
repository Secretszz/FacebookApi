// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		IOSProcessor.cs
//
// Author Name:		Bridge
//
// Create Time:		2023/12/26 18:23:11
// *******************************************

#if UNITY_IOS
namespace Bridge.FacebookApi
{
	using System.IO;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using UnityEditor.iOS.Xcode;
	using Editor;

	/// <summary>
	/// 
	/// </summary>
	internal static class IOSProcessor
	{
		[PostProcessBuild(10002)]
		public static void OnPostProcessBuild(BuildTarget target, string pathToBuildProject)
		{
			if (target == BuildTarget.iOS)
			{
				ThirdSDKSettings instance = ThirdSDKSettings.Instance;
				var plistPath = Path.Combine(pathToBuildProject, "Info.plist");
				var plist = new PlistDocument();
				plist.ReadFromFile(plistPath);
				var rootDic = plist.root;
			
				var items = new[]
				{
						"fbapi",
						"fb-messenger-share-api"
				};
			
				rootDic.AddApplicationQueriesSchemes(items);
				rootDic.SetString("FacebookAppID", instance.FbAppId);
				rootDic.SetString("FacebookClientToken", instance.FbClientToken);
				rootDic.SetString("FacebookDisplayName", instance.FbDisplayName);
				var array = rootDic.GetElementArray("CFBundleURLTypes");
				array.AddCFBundleURLTypes("Editor", "facebook", new[] { $"fb{instance.FbAppId}" });
				plist.WriteToFile(plistPath);
			}
		}
	}
}
#endif
