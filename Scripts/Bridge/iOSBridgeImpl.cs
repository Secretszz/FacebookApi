// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		iOSBridgeImpl.cs
//
// Author Name:		Bridge
//
// Create Time:		2024/09/03 10:53:45
// *******************************************

namespace Bridge.FacebookApi
{
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
		void IBridge.InitSDK(IInitListener initListener)
		{
			_initListener = initListener;
			fb_sdkInitialize(InitOnSuccess, InitOnError);
		}

		void IBridge.SetAdvertiserTrackingEnabled(bool _enabled)
		{
			fb_setAdvertiserTrackingEnabled(_enabled);
		}

		bool IBridge.IsInstalled()
		{
			return false;
		}

		void IBridge.RetrieveLoginStatus(ILoginListener loginListener)
		{
			if (fb_isLogged())
			{
				loginListener?.OnSuccess();
				return;
			}
			
			LogInWithReadPermissions(loginListener);
		}

		void IBridge.Login(ILoginListener loginListener)
		{
			LogInWithReadPermissions(loginListener);
		}

		void IBridge.Logout()
		{
			fb_logout();
		}

		void IBridge.ShareLink(string linkUrl, IShareListener shareListener)
		{
			_shareListener = shareListener;
			fb_shareLink(linkUrl, ShareOnSuccess, ShareOnCancel, ShareOnError);
		}

		void IBridge.ShareVideo(string videoUrl, IShareListener shareListener)
		{
			_shareListener = shareListener;
			fb_shareVideo(videoUrl, ShareOnSuccess, ShareOnCancel, ShareOnError);
		}

		void IBridge.ShareImage(string imagePath, IShareListener shareListener)
		{
			_shareListener = shareListener;
			fb_shareImage(imagePath, ShareOnSuccess, ShareOnCancel, ShareOnError);
		}

		void IBridge.ShareImage(byte[] imageData, IShareListener shareListener)
		{
			try
			{
				_shareListener = shareListener;
				int length = imageData.Length;
				IntPtr buffer = Marshal.AllocHGlobal(length);
				Marshal.Copy(imageData, 0, buffer, length);
				fb_shareImageWithDatas(buffer, length, ShareOnSuccess, ShareOnCancel, ShareOnError);
			}
			catch (Exception e)
			{
				string err = e.Message;
				Debug.LogError("字节流转指针解析错误：" + err);
				_shareListener = null;
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
			fb_logEvent(logEvent, valueToSum, JsonConvert.SerializeObject(pars));
		}

		void IBridge.FBLogEvent(string logEvent, Dictionary<string, string> pars)
		{
			fb_logEvent(logEvent, JsonConvert.SerializeObject(pars));
		}
		
		private void LogInWithReadPermissions(ILoginListener loginListener)
		{
			_loginListener = loginListener;
			fb_logInWithReadPermissions(LoginOnSuccess, LoginOnCancel, LoginOnError);
		}

		[DllImport("__Internal")]
		private static extern void fb_sdkInitialize(FBOnSuccess onSuccess, FBOnError onError);

		[DllImport("__Internal")]
		private static extern void fb_logInWithReadPermissions(FBOnSuccess onSuccess, FBOnCancel onCancel, FBOnError onError);

		[DllImport("__Internal")]
		private static extern bool fb_isLogged();

		[DllImport("__Internal")]
		private static extern void fb_logout();

		[DllImport("__Internal")]
		private static extern void fb_logPurchase(string logName, double priceAmount, string priceCurrency, string extra);

		[DllImport("__Internal")]
		private static extern void fb_logEvent(string logName, double valueToSum, string extra);

		[DllImport("__Internal")]
		private static extern void fb_logEvent(string logName, string extra);

		[DllImport("__Internal")]
		private static extern void fb_shareImage(string imagePath, FBOnSuccess onSuccess, FBOnCancel onCancel, FBOnError onError);

		[DllImport("__Internal")]
		private static extern void fb_shareImageWithDatas(IntPtr imageDatas, int length, FBOnSuccess onSuccess, FBOnCancel onCancel, FBOnError onError);

		[DllImport("__Internal")]
		private static extern void fb_shareLink(string uri, FBOnSuccess onSuccess, FBOnCancel onCancel, FBOnError onError);

		[DllImport("__Internal")]
		private static extern void fb_shareVideo(string uri, FBOnSuccess onSuccess, FBOnCancel onCancel, FBOnError onError);

		[DllImport("__Internal")]
		private static extern void fb_setAdvertiserTrackingEnabled(bool enabled);

		/// <summary>
		/// iOS桥接成功回调事件
		/// </summary>
		private delegate void FBOnSuccess();

		/// <summary>
		/// iOS桥接取消回调事件
		/// </summary>
		private delegate void FBOnCancel();

		/// <summary>
		/// iOS桥接错误回调事件
		/// </summary>
		private delegate void FBOnError(int errCode, string errMsg);

		/// <summary>
		/// Unity分享回调事件
		/// </summary>
		private static IInitListener _initListener;

		/// <summary>
		/// Unity登录回调事件
		/// </summary>
		private static ILoginListener _loginListener;

		/// <summary>
		/// Unity登录回调事件
		/// </summary>
		private static IShareListener _shareListener;

		/// <summary>
		/// iOS桥接分享回调事件
		/// </summary>
		[AOT.MonoPInvokeCallback(typeof(FBOnSuccess))]
		private static void InitOnSuccess()
		{
			_initListener?.OnSuccess();
		}

		/// <summary>
		/// iOS桥接分享回调事件
		/// </summary>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		[AOT.MonoPInvokeCallback(typeof(FBOnError))]
		private static void InitOnError(int errCode, string errMsg)
		{
			_initListener?.OnError(errCode, errMsg);
		}

		/// <summary>
		/// iOS桥接分享回调事件
		/// </summary>
		[AOT.MonoPInvokeCallback(typeof(FBOnSuccess))]
		private static void LoginOnSuccess()
		{
			_loginListener?.OnSuccess();
		}

		/// <summary>
		/// iOS桥接分享回调事件
		/// </summary>
		[AOT.MonoPInvokeCallback(typeof(FBOnSuccess))]
		private static void LoginOnCancel()
		{
			_loginListener?.OnCancel();
		}

		/// <summary>
		/// iOS桥接分享回调事件
		/// </summary>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		[AOT.MonoPInvokeCallback(typeof(FBOnError))]
		private static void LoginOnError(int errCode, string errMsg)
		{
			_loginListener?.OnError(errCode, errMsg);
		}

		/// <summary>
		/// iOS桥接分享回调事件
		/// </summary>
		[AOT.MonoPInvokeCallback(typeof(FBOnSuccess))]
		private static void ShareOnSuccess()
		{
			_shareListener?.OnSuccess();
		}

		/// <summary>
		/// iOS桥接分享回调事件
		/// </summary>
		[AOT.MonoPInvokeCallback(typeof(FBOnSuccess))]
		private static void ShareOnCancel()
		{
			_shareListener?.OnCancel();
		}

		/// <summary>
		/// iOS桥接分享回调事件
		/// </summary>
		/// <param name="errCode"></param>
		/// <param name="errMsg"></param>
		[AOT.MonoPInvokeCallback(typeof(FBOnError))]
		private static void ShareOnError(int errCode, string errMsg)
		{
			_shareListener?.OnError(errCode, errMsg);
		}
	}
}