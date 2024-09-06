// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		FacebookApi.cs
//
// Author Name:		Bridge
//
// Create Time:		2024/09/03 11:16:21
// *******************************************

namespace Bridge.FacebookApi
{
	using Common;
	using System.Collections.Generic;

	/// <summary>
	/// 
	/// </summary>
	public static class FacebookApi
	{
		private static IBridge _bridge;

		/// <summary>
		/// SDK桥接文件
		/// </summary>
		private static IBridge bridgeImpl
		{
			get
			{
				if (_bridge == null)
				{
#if UNITY_IOS && !UNITY_EDITOR
					_bridge = new iOSBridgeImpl();
#elif UNITY_ANDROID && !UNITY_EDITOR
					_bridge = new AndroidBridgeImpl();
#else
					_bridge = new EditorBridgeImpl();
#endif
				}

				return _bridge;
			}
		}

		/// <summary>
		/// 初始化Facebook SDK
		/// </summary>
		/// <param name="initListener"></param>
		public static void InitSDK(IInitListener initListener)
		{
			bridgeImpl.InitSDK(initListener);
		}

		/// <summary>
		/// 设置iOS的ATT设置
		/// </summary>
		/// <param name="_enabled"></param>
		public static void SetAdvertiserTrackingEnabled(bool _enabled)
		{
			bridgeImpl.SetAdvertiserTrackingEnabled(_enabled);
		}

		/// <summary>
		/// 是否安装了facebook
		/// </summary>
		/// <returns></returns>
		public static bool IsInstalled()
		{
			return bridgeImpl.IsInstalled();
		}

		/// <summary>
		/// 检查自动登录状态
		/// </summary>
		/// <param name="loginListener"></param>
		public static void RetrieveLoginStatus(ILoginListener loginListener)
		{
			bridgeImpl.RetrieveLoginStatus(loginListener);
		}

		/// <summary>
		/// 登录
		/// </summary>
		/// <param name="loginListener"></param>
		public static void Login(ILoginListener loginListener)
		{
			bridgeImpl.Login(loginListener);
		}

		/// <summary>
		/// Facebook SDK 登出
		/// </summary>
		public static void Logout()
		{
			bridgeImpl.Logout();
		}

		/// <summary>
		/// 分享链接
		/// </summary>
		/// <param name="linkUrl">链接地址</param>
		/// <param name="shareListener">拉起分享窗口事件</param>
		public static void ShareLink(string linkUrl, IShareListener shareListener)
		{
			bridgeImpl.ShareLink(linkUrl, shareListener);
		}

		/// <summary>
		/// 分享视频
		/// </summary>
		/// <param name="videoUrl">视频地址</param>
		/// <param name="shareListener">拉起分享窗口事件</param>
		public static void ShareVideo(string videoUrl, IShareListener shareListener)
		{
			bridgeImpl.ShareVideo(videoUrl, shareListener);
		}

		/// <summary>
		/// 分享图片
		/// </summary>
		/// <param name="imagePath">图片本地地址</param>
		/// <param name="shareListener">拉起分享窗口事件</param>
		public static void ShareImage(string imagePath, IShareListener shareListener)
		{
			bridgeImpl.ShareImage(imagePath, shareListener);
		}

		/// <summary>
		/// 分享图片
		/// </summary>
		/// <param name="imageData">图片数据</param>
		/// <param name="shareListener">拉起分享窗口事件</param>
		public static void ShareImage(byte[] imageData, IShareListener shareListener)
		{
			bridgeImpl.ShareImage(imageData, shareListener);
		}

		/// <summary>
		/// 充值成功的日志事件
		/// </summary>
		/// <param name="priceAmount"></param>
		/// <param name="priceCurrency">用户消费时，所使用的货币。这是一个包含3个字母的ISO的string结构数据。priceCurrency is a string containing the 3-letter ISO code for the currency that the user spent
		///  http://en.wikipedia.org/wiki/ISO_4217 </param>
		/// <param name="packageName">packageName is a string containing your SKU code for the thing they bought.</param>
		public static void FBLogPurchase(float priceAmount, string priceCurrency, string packageName)
		{
			bridgeImpl.FBLogPurchase(priceAmount, priceCurrency, packageName);
		}

		/// <summary>
		/// 自定义事件
		/// </summary>
		/// <param name="logEvent"></param>
		/// <param name="valueToSum"></param>
		/// <param name="pars"></param>
		public static void FBLogEvent(string logEvent, float valueToSum, Dictionary<string, string> pars)
		{
			bridgeImpl.FBLogEvent(logEvent, valueToSum, pars);
		}

		/// <summary>
		/// 自定义事件
		/// </summary>
		/// <param name="logEvent"></param>
		/// <param name="pars"></param>
		public static void FBLogEvent(string logEvent, Dictionary<string, string> pars)
		{
			bridgeImpl.FBLogEvent(logEvent, pars);
		}
	}
}