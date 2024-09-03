
// *******************************************
// Company Name:	深圳市晴天互娱科技有限公司
//
// File Name:		IBridge.cs
//
// Author Name:		Bridge
//
// Create Time:		2024/09/03 10:08:57
// *******************************************

namespace Bridge.FacebookApi
{
	using System.Collections.Generic;

	/// <summary>
	/// 
	/// </summary>
	internal interface IBridge
	{
		/// <summary>
		/// 初始化Facebook SDK
		/// </summary>
		/// <param name="initListener"></param>
		void InitSDK(IInitListener initListener);

		/// <summary>
		/// 设置iOS的ATT设置
		/// </summary>
		/// <param name="_enabled"></param>
		void SetAdvertiserTrackingEnabled(bool _enabled);

		/// <summary>
		/// 是否安装了facebook
		/// </summary>
		/// <returns></returns>
		bool IsInstalled();

		/// <summary>
		/// 检查自动登录状态
		/// </summary>
		/// <param name="loginListener"></param>
		void RetrieveLoginStatus(ILoginListener loginListener);

		/// <summary>
		/// 登录
		/// </summary>
		/// <param name="loginListener"></param>
		void Login(ILoginListener loginListener);

		/// <summary>
		/// Facebook SDK 登出
		/// </summary>
		void Logout();

		/// <summary>
		/// 分享链接
		/// </summary>
		/// <param name="linkUrl">链接地址</param>
		/// <param name="shareListener">拉起分享窗口事件</param>
		void ShareLink(string linkUrl, IShareListener shareListener);

		/// <summary>
		/// 分享视频
		/// </summary>
		/// <param name="videoUrl">视频地址</param>
		/// <param name="shareListener">拉起分享窗口事件</param>
		void ShareVideo(string videoUrl, IShareListener shareListener);

		/// <summary>
		/// 分享图片
		/// </summary>
		/// <param name="imagePath">图片本地地址</param>
		/// <param name="shareListener">拉起分享窗口事件</param>
		void ShareImage(string imagePath, IShareListener shareListener);

		/// <summary>
		/// 分享图片
		/// </summary>
		/// <param name="imageData">图片数据</param>
		/// <param name="shareListener">拉起分享窗口事件</param>
		void ShareImage(byte[] imageData, IShareListener shareListener);

		/// <summary>
		/// 充值成功的日志事件
		/// </summary>
		/// <param name="priceAmount"></param>
		/// <param name="priceCurrency">用户消费时，所使用的货币。这是一个包含3个字母的ISO的string结构数据。priceCurrency is a string containing the 3-letter ISO code for the currency that the user spent
		///  http://en.wikipedia.org/wiki/ISO_4217 </param>
		/// <param name="packageName">packageName is a string containing your SKU code for the thing they bought.</param>
		void FBLogPurchase(float priceAmount, string priceCurrency, string packageName);

		/// <summary>
		/// 自定义事件
		/// </summary>
		/// <param name="logEvent"></param>
		/// <param name="valueToSum"></param>
		/// <param name="pars"></param>
		void FBLogEvent(string logEvent, float valueToSum, Dictionary<string, string> pars);

		/// <summary>
		/// 自定义事件
		/// </summary>
		/// <param name="logEvent"></param>
		/// <param name="pars"></param>
		void FBLogEvent(string logEvent, Dictionary<string, string> pars);
	}
}