// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		iOSBridgeImpl.cs
//
// Author Name:		Bridge
//
// Create Time:		2024/09/03 10:53:45
// *******************************************

#if UNITY_IOS
namespace Bridge.FacebookApi
{
	using AOT;
	using Common;
	using Newtonsoft.Json;
	using UnityEngine;
	using System;
	using System.Runtime.InteropServices;
	using System.Collections.Generic;

	/// <summary>
	/// 
	/// </summary>
	internal class iOSBridgeImpl : IBridge
	{
		void IBridge.InitSDK(IBridgeListener initListener)
		{
			InitCallback._initListener = initListener;
			fb_sdkInitialize(InitCallback.OnSuccess, InitCallback.OnCancel, InitCallback.OnError);
		}

		void IBridge.SetAdvertiserTrackingEnabled(bool _enabled)
		{
			fb_setAdvertiserTrackingEnabled(_enabled);
		}

		bool IBridge.IsInstalled()
		{
			return false;
		}

		void IBridge.RetrieveLoginStatus(IBridgeListener loginListener)
		{
			string accessToken = fb_getAccessToken();
			if (!string.IsNullOrEmpty(accessToken))
			{
				loginListener?.OnSuccess(accessToken);
				return;
			}
			
			LogInWithReadPermissions(loginListener);
		}

		void IBridge.Login(IBridgeListener loginListener)
		{
			LogInWithReadPermissions(loginListener);
		}

		void IBridge.Logout()
		{
			fb_logout();
		}

		void IBridge.ShareLink(string linkUrl, IBridgeListener shareListener)
		{
			ShareCallback._shareListener = shareListener;
			fb_shareLink(linkUrl, ShareCallback.OnSuccess, ShareCallback.OnCancel, ShareCallback.OnError);
		}

		void IBridge.ShareVideo(string videoUrl, IBridgeListener shareListener)
		{
			ShareCallback._shareListener = shareListener;
			fb_shareVideo(videoUrl, ShareCallback.OnSuccess, ShareCallback.OnCancel, ShareCallback.OnError);
		}

		void IBridge.ShareImage(string imagePath, IBridgeListener shareListener)
		{
			ShareCallback._shareListener = shareListener;
			fb_shareImage(imagePath, ShareCallback.OnSuccess, ShareCallback.OnCancel, ShareCallback.OnError);
		}

		void IBridge.ShareImage(byte[] imageData, IBridgeListener shareListener)
		{
			try
			{
				ShareCallback._shareListener = shareListener;
				int length = imageData.Length;
				IntPtr buffer = Marshal.AllocHGlobal(length);
				Marshal.Copy(imageData, 0, buffer, length);
				fb_shareImageWithDatas(buffer, length, ShareCallback.OnSuccess, ShareCallback.OnCancel, ShareCallback.OnError);
			}
			catch (Exception e)
			{
				string err = e.Message;
				Debug.LogError("字节流转指针解析错误：" + err);
				ShareCallback._shareListener = null;
				shareListener?.OnError(-1, err);
			}
		}

		void IBridge.FBLogPurchase(float priceAmount, string priceCurrency, string packageName)
		{
			var iapParameters = new Dictionary<string, string>();
			iapParameters["BillingID"] = packageName;
			fb_logPurchase("logPurchase", priceAmount, priceCurrency, JsonConvert.SerializeObject(iapParameters));
		}

		void IBridge.FBLogEvent(string logEvent, float valueToSum, Dictionary<string, string> pars)
		{
			fb_logEventWithSum(logEvent, valueToSum, JsonConvert.SerializeObject(pars));
		}

		void IBridge.FBLogEvent(string logEvent, Dictionary<string, string> pars)
		{
			fb_logEvent(logEvent, JsonConvert.SerializeObject(pars));
		}
		
		private void LogInWithReadPermissions(IBridgeListener loginListener)
		{
			LoginCallback._loginListener = loginListener;
			fb_logInWithReadPermissions(LoginCallback.OnSuccess, LoginCallback.OnCancel, LoginCallback.OnError);
		}

		[DllImport("__Internal")]
		private static extern void fb_sdkInitialize(U3DBridgeCallback_Success onSuccess, U3DBridgeCallback_Cancel onCancel, U3DBridgeCallback_Error onError);

		[DllImport("__Internal")]
		private static extern void fb_logInWithReadPermissions(U3DBridgeCallback_Success onSuccess, U3DBridgeCallback_Cancel onCancel, U3DBridgeCallback_Error onError);

		[DllImport("__Internal")]
		private static extern string fb_getAccessToken();

		[DllImport("__Internal")]
		private static extern void fb_logout();

		[DllImport("__Internal")]
		private static extern void fb_logPurchase(string logName, double priceAmount, string priceCurrency, string extra);

		[DllImport("__Internal")]
		private static extern void fb_logEventWithSum(string logName, double valueToSum, string extra);

		[DllImport("__Internal")]
		private static extern void fb_logEvent(string logName, string extra);

		[DllImport("__Internal")]
		private static extern void fb_shareImage(string imagePath, U3DBridgeCallback_Success onSuccess, U3DBridgeCallback_Cancel onCancel, U3DBridgeCallback_Error onError);

		[DllImport("__Internal")]
		private static extern void fb_shareImageWithDatas(IntPtr imageDatas, int length, U3DBridgeCallback_Success onSuccess, U3DBridgeCallback_Cancel onCancel, U3DBridgeCallback_Error onError);

		[DllImport("__Internal")]
		private static extern void fb_shareLink(string uri, U3DBridgeCallback_Success onSuccess, U3DBridgeCallback_Cancel onCancel, U3DBridgeCallback_Error onError);

		[DllImport("__Internal")]
		private static extern void fb_shareVideo(string uri, U3DBridgeCallback_Success onSuccess, U3DBridgeCallback_Cancel onCancel, U3DBridgeCallback_Error onError);

		[DllImport("__Internal")]
		private static extern void fb_setAdvertiserTrackingEnabled(bool enabled);

		private static class InitCallback
		{
			/// <summary>
			/// 支付回调监听
			/// </summary>
			public static IBridgeListener _initListener;

			/// <summary>
			/// 支付成功回调桥接函数
			/// </summary>
			/// <param name="result"></param>
			[MonoPInvokeCallback(typeof(U3DBridgeCallback_Success))]
			public static void OnSuccess(string result)
			{
				_initListener?.OnSuccess(result);
			}

			/// <summary>
			/// 支付用户取消回调桥接函数
			/// </summary>
			[MonoPInvokeCallback(typeof(U3DBridgeCallback_Cancel))]
			public static void OnCancel()
			{
				_initListener?.OnCancel();
			}

			/// <summary>
			/// 支付错误回调桥接函数
			/// </summary>
			/// <param name="errCode"></param>
			/// <param name="errMsg"></param>
			[MonoPInvokeCallback(typeof(U3DBridgeCallback_Error))]
			public static void OnError(int errCode, string errMsg)
			{
				_initListener?.OnError(errCode, errMsg);
			}
		}
		
		private static class ShareCallback
		{
			/// <summary>
			/// 分享回调监听
			/// </summary>
			public static IBridgeListener _shareListener;

			/// <summary>
			/// 支付成功回调桥接函数
			/// </summary>
			/// <param name="result"></param>
			[MonoPInvokeCallback(typeof(U3DBridgeCallback_Success))]
			public static void OnSuccess(string result)
			{
				_shareListener?.OnSuccess(result);
			}

			/// <summary>
			/// 支付用户取消回调桥接函数
			/// </summary>
			[MonoPInvokeCallback(typeof(U3DBridgeCallback_Cancel))]
			public static void OnCancel()
			{
				_shareListener?.OnCancel();
			}

			/// <summary>
			/// 支付错误回调桥接函数
			/// </summary>
			/// <param name="errCode"></param>
			/// <param name="errMsg"></param>
			[MonoPInvokeCallback(typeof(U3DBridgeCallback_Error))]
			public static void OnError(int errCode, string errMsg)
			{
				_shareListener?.OnError(errCode, errMsg);
			}
		}
		
		private static class LoginCallback
		{
			/// <summary>
			/// 权限回调监听
			/// </summary>
			public static IBridgeListener _loginListener;

			/// <summary>
			/// 支付成功回调桥接函数
			/// </summary>
			/// <param name="result"></param>
			[MonoPInvokeCallback(typeof(U3DBridgeCallback_Success))]
			public static void OnSuccess(string result)
			{
				_loginListener?.OnSuccess(result);
			}

			/// <summary>
			/// 支付用户取消回调桥接函数
			/// </summary>
			[MonoPInvokeCallback(typeof(U3DBridgeCallback_Cancel))]
			public static void OnCancel()
			{
				_loginListener?.OnCancel();
			}

			/// <summary>
			/// 支付错误回调桥接函数
			/// </summary>
			/// <param name="errCode"></param>
			/// <param name="errMsg"></param>
			[MonoPInvokeCallback(typeof(U3DBridgeCallback_Error))]
			public static void OnError(int errCode, string errMsg)
			{
				_loginListener?.OnError(errCode, errMsg);
			}
		}
	}
}
#endif