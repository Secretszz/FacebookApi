// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		EditorBridgeImpl.cs
//
// Author Name:		Bridge
//
// Create Time:		2024/09/03 10:51:48
// *******************************************

#if UNITY_EDITOR

namespace Bridge.FacebookApi
{
	using Common;
	using System.Collections.Generic;

	/// <summary>
	/// 
	/// </summary>
	internal class EditorBridgeImpl : IBridge
	{
		void IBridge.InitSDK(IBridgeListener callback)
		{
			callback?.OnSuccess("");
		}

		void IBridge.SetAdvertiserTrackingEnabled(bool _enabled)
		{
		}

		bool IBridge.IsInstalled()
		{
			return false;
		}

		void IBridge.RetrieveLoginStatus(IBridgeListener callback)
		{
			callback?.OnSuccess("");
		}

		void IBridge.Login(IBridgeListener callback)
		{
			callback?.OnSuccess("");
		}

		void IBridge.Logout()
		{
		}

		void IBridge.ShareLink(string linkUrl, IBridgeListener shareListener)
		{
			shareListener?.OnSuccess("");
		}

		void IBridge.ShareVideo(string videoUrl, IBridgeListener shareListener)
		{
			shareListener?.OnSuccess("");
		}

		void IBridge.ShareImage(string imagePath, IBridgeListener shareListener)
		{
			shareListener?.OnSuccess("");
		}

		void IBridge.ShareImage(byte[] imageData, IBridgeListener shareListener)
		{
			shareListener?.OnSuccess("");
		}

		void IBridge.FBLogPurchase(float priceAmount, string priceCurrency, string packageName)
		{
		}

		void IBridge.FBLogEvent(string logEvent, float valueToSum, Dictionary<string, string> pars)
		{
		}

		void IBridge.FBLogEvent(string logEvent, Dictionary<string, string> pars)
		{
		}
	}
}

#endif