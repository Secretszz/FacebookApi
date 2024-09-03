// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		AndroidBridgeImpl.cs
//
// Author Name:		Bridge
//
// Create Time:		2024/09/03 10:21:27
// *******************************************

namespace Bridge.FacebookApi
{
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

		void IBridge.InitSDK(IInitListener callback)
		{
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaClass jc = new AndroidJavaClass(MANAGER_PATH);
			mSdkManager = jc.CallStatic<AndroidJavaObject>("getInstance");
			mSdkManager.Call("initSDK", currentActivity, new FBInitCallback(callback));
		}

		void IBridge.SetAdvertiserTrackingEnabled(bool _enabled)
		{
		}

		bool IBridge.IsInstalled()
		{
			return mSdkManager.Call<bool>("isInstalled", currentActivity);
		}

		void IBridge.RetrieveLoginStatus(ILoginListener callback)
		{
			mSdkManager.Call("retrieveLoginStatus", currentActivity, new LoginCallback(callback));
		}

		void IBridge.Login(ILoginListener callback)
		{
			mSdkManager.Call("login", currentActivity, new LoginCallback(callback));
		}

		void IBridge.Logout()
		{
			mSdkManager.Call("logout");
		}

		void IBridge.ShareLink(string linkUrl, IShareListener shareListener)
		{
			mSdkManager.Call("shareLink", currentActivity, linkUrl, new FBShareCallback(shareListener));
		}

		void IBridge.ShareVideo(string videoUrl, IShareListener shareListener)
		{
			mSdkManager.Call("shareVideo", currentActivity, videoUrl, new FBShareCallback(shareListener));
		}

		void IBridge.ShareImage(string imagePath, IShareListener shareListener)
		{
			mSdkManager.Call("shareImage", currentActivity, imagePath, new FBShareCallback(shareListener));
		}

		void IBridge.ShareImage(byte[] imageData, IShareListener shareListener)
		{
			mSdkManager.Call("shareImage", currentActivity, imageData, new FBShareCallback(shareListener));
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

		private class FBInitCallback : AndroidJavaProxy
		{
			private IInitListener callback;

			public FBInitCallback(IInitListener callback) : base(CALLBACK_PATH)
			{
				this.callback = callback;
			}

			public void onSuccess()
			{
				//Debug.Log("onSuccess: " + result.Call<string>("getPostId"));
				callback?.OnSuccess();
			}

			public void onCancel()
			{
				
			}

			public void onError(int errCode, string errMsg)
			{
				//Debug.Log("onError: " + msg);
				callback?.OnError(errCode, errMsg);
			}
		}

		private class FBShareCallback : AndroidJavaProxy
		{
			private IShareListener callback;

			public FBShareCallback(IShareListener callback) : base(CALLBACK_PATH)
			{
				this.callback = callback;
			}

			public void onSuccess()
			{
				callback?.OnSuccess();
			}

			public void onCancel()
			{
				callback?.OnCancel();
			}

			public void onError(int errCode, string errMsg)
			{
				callback?.OnError(errCode, errMsg);
			}
		}

		private class LoginCallback : AndroidJavaProxy
		{
			private ILoginListener callback;

			public LoginCallback(ILoginListener callback) : base(CALLBACK_PATH)
			{
				this.callback = callback;
			}

			public void onSuccess()
			{
				callback?.OnSuccess();
			}

			public void onCancel()
			{
				callback?.OnCancel();
			}

			public void onError(int errCode, string errMsg)
			{
				callback?.OnError(errCode, errMsg);
			}
		}
	}
}