// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		AndroidBridgeImpl.cs
//
// Author Name:		Bridge
//
// Create Time:		2024/09/03 10:21:27
// *******************************************

#if UNITY_ANDROID
namespace Bridge.FacebookApi
{
	using Common;
	using Newtonsoft.Json;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// 
	/// </summary>
	internal class AndroidBridgeImpl : IBridge
	{
		private const string MANAGER_PATH = "com.bridge.facebook.FacebookSdkManager";
		private const string CALLBACK_PATH = "com.bridge.facebook.listener.IBridgeListener";
		private AndroidJavaObject mSdkManager;
		private AndroidJavaObject currentActivity;

		void IBridge.InitSDK(IBridgeListener callback)
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaClass jc = new AndroidJavaClass(MANAGER_PATH);
			mSdkManager = jc.CallStatic<AndroidJavaObject>("getInstance");
			mSdkManager.Call("initSDK", currentActivity, new BridgeCallback(callback));
		}

		void IBridge.SetAdvertiserTrackingEnabled(bool _enabled)
		{
		}

		bool IBridge.IsInstalled()
		{
			return mSdkManager.Call<bool>("isInstalled", currentActivity);
		}

		void IBridge.RetrieveLoginStatus(IBridgeListener callback)
		{
			mSdkManager.Call("retrieveLoginStatus", currentActivity, new BridgeCallback(callback));
		}

		void IBridge.Login(IBridgeListener callback)
		{
			mSdkManager.Call("login", currentActivity, new BridgeCallback(callback));
		}

		void IBridge.Logout()
		{
			mSdkManager.Call("logout");
		}

		void IBridge.ShareLink(string linkUrl, IBridgeListener shareListener)
		{
			mSdkManager.Call("shareLink", currentActivity, linkUrl, new BridgeCallback(shareListener));
		}

		void IBridge.ShareVideo(string videoUrl, IBridgeListener shareListener)
		{
			mSdkManager.Call("shareVideo", currentActivity, videoUrl, new BridgeCallback(shareListener));
		}

		void IBridge.ShareImage(string imagePath, IBridgeListener shareListener)
		{
			mSdkManager.Call("shareImage", currentActivity, imagePath, new BridgeCallback(shareListener));
		}

		void IBridge.ShareImage(byte[] imageData, IBridgeListener shareListener)
		{
			mSdkManager.Call("shareImage", currentActivity, imageData, new BridgeCallback(shareListener));
		}

		void IBridge.FBLogPurchase(float priceAmount, string priceCurrency, string packageName)
		{
			var iapParameters = new Dictionary<string, string>();
			iapParameters["BillingID"] = packageName;
			mSdkManager.Call("logPurchase", (double)priceAmount, priceCurrency, JsonConvert.SerializeObject(iapParameters));
		}

		void IBridge.FBLogEvent(string logEvent, float valueToSum, Dictionary<string, string> pars)
		{
			mSdkManager.Call("logEvent", logEvent, (double)valueToSum, JsonConvert.SerializeObject(pars));
		}

		void IBridge.FBLogEvent(string logEvent, Dictionary<string, string> pars)
		{
			mSdkManager.Call("logEvent", logEvent, JsonConvert.SerializeObject(pars));
		}
	}
}
#endif