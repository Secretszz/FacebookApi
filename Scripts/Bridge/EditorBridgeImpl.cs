// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		EditorBridgeImpl.cs
//
// Author Name:		Bridge
//
// Create Time:		2024/09/03 10:51:48
// *******************************************

namespace Bridge.FacebookApi
{
	using System.Collections.Generic;

	/// <summary>
	/// 
	/// </summary>
	internal class EditorBridgeImpl : IBridge
	{
		void IBridge.InitSDK(IInitListener callback)
		{
			callback?.OnSuccess();
		}

		void IBridge.SetAdvertiserTrackingEnabled(bool _enabled)
		{
		}

		bool IBridge.IsInstalled()
		{
			return false;
		}

		void IBridge.RetrieveLoginStatus(ILoginListener callback)
		{
			callback?.OnSuccess();
		}

		void IBridge.Login(ILoginListener callback)
		{
			callback?.OnSuccess();
		}

		void IBridge.Logout()
		{
		}

		void IBridge.ShareLink(string linkUrl, IShareListener shareListener)
		{
			shareListener?.OnSuccess();
		}

		void IBridge.ShareVideo(string videoUrl, IShareListener shareListener)
		{
			shareListener?.OnSuccess();
		}

		void IBridge.ShareImage(string imagePath, IShareListener shareListener)
		{
			shareListener?.OnSuccess();
		}

		void IBridge.ShareImage(byte[] imageData, IShareListener shareListener)
		{
			shareListener?.OnSuccess();
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